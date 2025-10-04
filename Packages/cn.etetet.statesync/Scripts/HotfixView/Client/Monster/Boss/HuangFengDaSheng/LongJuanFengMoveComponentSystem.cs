using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(LongJuanFengMoveComponent))]
    [FriendOfAttribute(typeof(ET.Client.LongJuanFengMoveComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    public static partial class LongJuanFengMoveComponentSystem
    {
        // 比普通怪移动更快的速度系数
        private const float MoveSpeedFactor = 1f; // 普通怪是0.2，这里提升到0.6

        [EntitySystem]
        private static void Awake(this ET.Client.LongJuanFengMoveComponent self, Vector3 pos)
        {
            self.InitPosition = pos;
            // 稍大一点的活动半径
            self.RandomRadius = 10f; 
            // 追击距离
            //self.ChaseStopThreshold = 1f;
            self.IsFinishRandom = true;
            self.AttackFlag = false;

            Unit unit = self.GetParent<Unit>();
            GameObject go = unit.GetComponent<GameObjectComponent>().GameObject;
            self.Seeker = go.GetComponentInChildren<Seeker>();
            self.Colliders = new Collider[8];
            if (self.Seeker != null)
            {
                self.Seeker.pathCallback += self.OnCallFunc;
            }

            // 启动随机移动
            self.RandomStartPath();
        }

        [EntitySystem]
        private static void Update(this ET.Client.LongJuanFengMoveComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            GameObject go = unit.GetComponent<GameObjectComponent>().GameObject;
            if (go == null || !go.activeSelf)
                return;

            // 检查与黄风大圣的距离，如果过远则朝黄风大圣方向移动
            if ((Unit)self.BossUnit != null)
            {
                Unit bossUnit = self.BossUnit;
                if (bossUnit != null && !bossUnit.IsDisposed)
                {
                    GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                    if (bossGO != null)
                    {
                        float distanceToBoss = Vector3.Distance(go.transform.position, bossGO.transform.position);
                        if (distanceToBoss > self.MaxDistanceFromBoss)
                        {
                            // 距离过远，朝黄风大圣方向移动
                            self.MoveTowardsBoss(bossGO.transform.position);
                            return;
                        }
                    }
                }
            }

            // 仅随机移动：无追击/返回逻辑
            if (self.Path == null && self.IsFinishRandom)
            {
                self.RandomStartPath();
                return;
            }

            self.SeekFunc();
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.LongJuanFengMoveComponent self)
        {
            if (self.Seeker != null)
            {
                self.Seeker.pathCallback -= self.OnCallFunc;
                self.Seeker.CancelCurrentPathRequest();
            }
            self.Path = null;
            self.Seeker = null;
            self.Colliders = null;
            self.BossUnit = null;
        }

        // 朝黄风大圣方向移动
        public static void MoveTowardsBoss(this LongJuanFengMoveComponent self, Vector3 bossPosition)
        {
            if (!self.IsFinishRandom)
                return;

            self.IsFinishRandom = false;
            Unit unit = self.GetParent<Unit>();
            Transform transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;

            // 计算朝向黄风大圣的方向，但保持在合理距离内
            Vector3 directionToBoss = (bossPosition - transform.position).normalized;
            Vector3 targetPosition = bossPosition - directionToBoss * (self.MaxDistanceFromBoss * 0.8f); // 保持在80%的最大距离内
            
            // 确保目标点在可行走区域内
            GraphNode node = AstarPath.active.GetNearest(targetPosition).node;
            if (node != null && node.Walkable)
            {
                targetPosition = (Vector3)node.position;
            }
            else
            {
                // 如果目标点不可行走，使用当前位置附近的可行走点
                targetPosition = self.GetRandomWalkablePoint();
            }

            // 最终检查：确保目标点确实在合理范围内
            float finalDistanceToBoss = Vector3.Distance(targetPosition, bossPosition);
            if (finalDistanceToBoss > self.MaxDistanceFromBoss)
            {
                // 如果还是超出范围，强制使用黄风大圣附近的安全点
                targetPosition = bossPosition + Vector3.forward * (self.MaxDistanceFromBoss * 0.5f);
                GraphNode safeNode = AstarPath.active.GetNearest(targetPosition).node;
                if (safeNode != null && safeNode.Walkable)
                {
                    targetPosition = (Vector3)safeNode.position;
                }
                else
                {
                    // 最后的回退：直接使用黄风大圣位置
                    targetPosition = bossPosition;
                }
            }

            if (self.Seeker != null)
            {
                self.Seeker.StartPath(transform.position, targetPosition);
            }
        }

        public static void RandomStartPath(this LongJuanFengMoveComponent self)
        {
            if (!self.IsFinishRandom)
                return;

            self.IsFinishRandom = false;
            Unit unit = self.GetParent<Unit>();
            Transform transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;

            Vector3 randomPoint = self.GetRandomWalkablePoint();
            if (self.Seeker != null)
            {
                self.Seeker.StartPath(transform.position, randomPoint);
            }
        }

        public static Vector3 GetRandomWalkablePoint(this LongJuanFengMoveComponent self)
        {
            // 如果有黄风大圣引用，优先在黄风大圣附近生成随机点
            Vector3 centerPosition = self.InitPosition;
            if ((Unit)self.BossUnit != null)
            {
                Unit bossUnit = self.BossUnit;
                if (bossUnit != null && !bossUnit.IsDisposed)
                {
                    GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                    if (bossGO != null)
                    {
                        centerPosition = bossGO.transform.position;
                    }
                }
            }

            for (int i = 0; i < 30; i++)
            {
                // 基于个人偏移的扰动，避免多个龙卷风向同一点收敛
                Vector3 baseOffset = self.PersonalOffset;
                if (baseOffset == Vector3.zero)
                {
                    baseOffset = (self.InitPosition - centerPosition);
                    baseOffset.y = 0;
                    if (baseOffset == Vector3.zero)
                    {
                        baseOffset = UnityEngine.Random.insideUnitSphere;
                    }
                    baseOffset = baseOffset.normalized * self.OrbitRadius;
                }

                Vector3 randomPoint = centerPosition + baseOffset + UnityEngine.Random.insideUnitSphere * self.RandomRadius;//(self.RandomRadius * 0.5f);
                randomPoint.y = centerPosition.y;
                
                // 检查生成的随机点是否在黄风大圣的合理距离内
                if ((Unit)self.BossUnit != null)
                {
                    Unit bossUnit = self.BossUnit;
                    if (bossUnit != null && !bossUnit.IsDisposed)
                    {
                        GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                        if (bossGO != null)
                        {
                            float distanceToBoss = Vector3.Distance(randomPoint, bossGO.transform.position);
                            // 如果距离黄风大圣太远，跳过这个点，重新生成
                            if (distanceToBoss > self.MaxDistanceFromBoss)
                            {
                                continue;
                            }
                        }
                    }
                }
                
                GraphNode node = AstarPath.active.GetNearest(randomPoint).node;
                if (node != null && node.Walkable)
                {
                    return (Vector3)node.position;
                }
            }
            
            // 如果30次尝试都失败，返回一个在合理范围内的安全点
            if ((Unit)self.BossUnit != null)
            {
                Unit bossUnit = self.BossUnit;
                if (bossUnit != null && !bossUnit.IsDisposed)
                {
                    GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                    if (bossGO != null)
                    {
                        // 返回黄风大圣附近的一个安全点
                        Vector3 safePoint = bossGO.transform.position + Vector3.forward * (self.MaxDistanceFromBoss * 0.5f);
                        GraphNode safeNode = AstarPath.active.GetNearest(safePoint).node;
                        if (safeNode != null && safeNode.Walkable)
                        {
                            return (Vector3)safeNode.position;
                        }
                    }
                }
            }
            
            return centerPosition;
        }

        public static void SeekFunc(this LongJuanFengMoveComponent self)
        {
            if (self.Path == null)
                return;

            Unit unit = self.GetParent<Unit>();
            Transform transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;

            // 检查当前路径的目标点是否在合理范围内
            if ((Unit)self.BossUnit != null)
            {
                Unit bossUnit = self.BossUnit;
                if (bossUnit != null && !bossUnit.IsDisposed)
                {
                    GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                    if (bossGO != null && self.Path.vectorPath.Count > 0)
                    {
                        // 检查路径的终点是否在合理范围内
                        Vector3 pathEndPoint = self.Path.vectorPath[self.Path.vectorPath.Count - 1];
                        float distanceToBoss = Vector3.Distance(pathEndPoint, bossGO.transform.position);
                        if (distanceToBoss > self.MaxDistanceFromBoss)
                        {
                            // 目标点超出范围，直接朝黄风大圣方向移动回到合理范围
                            Debug.Log($"龙卷风：目标点距离黄风大圣{distanceToBoss:F2}超出范围{self.MaxDistanceFromBoss}，朝黄风大圣方向移动");
                            self.CurrentPoint = 0;
                            if (self.Seeker != null)
                            {
                                self.Seeker.CancelCurrentPathRequest();
                            }
                            self.Path = null;
                            self.IsFinishRandom = true;
                            self.MoveTowardsBoss(bossGO.transform.position);
                            return;
                        }
                    }
                }
            }

            if (self.CurrentPoint >= self.Path.vectorPath.Count || self.AttackFlag)
            {
                self.CurrentPoint = 0;
                if (self.Seeker != null)
                {
                    self.Seeker.CancelCurrentPathRequest();
                }
                self.Path = null;
                self.IsFinishRandom = true;
                self.RandomStartPath();
                return;
            }

            Vector3 dir = (self.Path.vectorPath[self.CurrentPoint] - transform.position).normalized;
            float speed = MoveSpeedFactor;
            // 不再追击玩家，仅随机移动
            dir *= speed * Time.deltaTime;

            // 可选翻转：仅沿Y轴翻转，符合2.5D表现
            if (dir.x != 0)
            {
                float targetYRotation = dir.x > 0 ? 0f : 180f;
                if (transform.childCount > 0)
                {
                    Vector3 euler = transform.GetChild(0).localEulerAngles;
                    euler.y = targetYRotation;
                    transform.GetChild(0).localEulerAngles = euler;
                }
            }

            transform.Translate(dir, Space.World);

            Vector3 offset = transform.position - self.Path.vectorPath[self.CurrentPoint];
            if (offset.sqrMagnitude < 0.01f)
            {
                self.CurrentPoint++;
            }
        }

        public static void OnCallFunc(this LongJuanFengMoveComponent self, Path p)
        {
            if (!p.error)
            {
                self.Path = p;
            }
        }

        public static void StopMove(this LongJuanFengMoveComponent self)
        {
            if (self.Seeker != null)
            {
                self.Seeker.CancelCurrentPathRequest();
            }
            self.Path = null;
            self.CurrentPoint = 0;
            self.IsFinishRandom = true;
            self.TargetUnit = null;
            // 追击逻辑已移除
        }

        // 追击逻辑已移除
    }
}