using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(MonsterMoveComponent))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MonsterAttackComponent))]
    public static partial class MonsterMoveComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MonsterMoveComponent self, Vector3 pos)
        {
            //Debug.Log("随机寻路 ： 找到玩家0");
            self.InitPosition = pos;
            self.RandomRadius = 3;
            self.IsFinishRandom = true;
            self.AttackFlag = false;
            var unit = self.GetParent<Unit>();

            // 确保动画组件存在
            if (unit.GetComponent<AISkeletonAnimationComponent>() == null)
            {
                //Log.Warning("怪物移动组件初始化时，动画组件不存在");
                unit.AddComponent<AISkeletonAnimationComponent>();
            }

            var gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
            self.Seeker = gameObject.GetComponentInChildren<Seeker>();
            self.Colliders = new Collider[20];
            self.Seeker.pathCallback += self.OnCallFunc;

            // 设置默认朝向和动画状态
            unit.Direction = Direction.Front;
            //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 初始化时设置Walk动画状态");
            unit.SetAIUnitActionType(UnitActionType.Walk, true);

            // 初始化后开始随机移动
            self.RandomStartPath();
        }
        [EntitySystem]
        private static void Update(this ET.Client.MonsterMoveComponent self)
        {
            var unit = self.GetParent<Unit>();
            var monsterActionType = unit.GetMonsterActionType();
            var gameObject = unit.GetComponent<GameObjectComponent>().GameObject;

            if (!gameObject.activeSelf)
                return;

            // 如果不是行走状态，停止移动
            if (monsterActionType != MonsterActionType.Walk)
            {
                self.StopMove();
                return;
            }

            // 如果没有路径且不在寻路中，开始随机移动
            if (self.Path == null && self.IsFinishRandom)
            {
                self.RandomStartPath();
                return;
            }

            // 1. 先检查与出生点的距离，决定是否需要返回
            self.CheckTargetDistance();
            // 2. 追击：如果没有返回出生点，则寻找玩家目标
            self.FindTarget();
            // 3. 根据当前状态决定移动方向
            self.CheckMove();
            // 4. 执行实际的移动
            self.SeekFunc();
        }
        [EntitySystem]
        private static void Destroy(this ET.Client.MonsterMoveComponent self)
        {
            var unit = self.GetParent<Unit>();
            unit.RemoveComponent<AISkeletonAnimationComponent>();
            self.StopMove();
            self.Path = null;
            self.Seeker = null;
        }
        public static void CheckTargetDistance(this MonsterMoveComponent self)
        {
            var unit = self.GetParent<Unit>();
            var gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
            if (!gameObject.activeSelf)
                return;


            // 检查是否离开出生点太远
            float distanceToSpawn = Vector3.Distance(gameObject.transform.position, self.InitPosition);
            if (distanceToSpawn > self.RandomRadius * 2)
            {
                // Debug.Log("随机寻路 ： 放弃追击：出生位置："+self.InitPosition+"本体位置："+gameObject.transform.position);
                // 离开出生点太远，放弃追击
                self.TargetUnit = null;
                // 返回出生点
                self.IsFinishRandom = true;
                if (!self.IsFinishRandom)
                    return;
                self.IsFinishRandom = false;
                self.Seeker.StartPath(gameObject.transform.position, self.InitPosition);
                return;
            }

            // 如果当前有目标，检查与目标的距离
            if ((Unit)self.TargetUnit != null)
            {
                Unit targetUnit = (Unit)self.TargetUnit;
                var targetTransform = targetUnit.GetComponent<GameObjectComponent>().GameObject.transform;
                float distanceToTarget = Vector3.Distance(gameObject.transform.position, targetTransform.position);

                // 如果与目标的距离大于追击范围（这里使用 RandomRadius * 2），放弃追击
                if (distanceToTarget > self.RandomRadius * 2)
                {
                    self.TargetUnit = null;
                    // 返回出生点
                    self.Seeker.StartPath(gameObject.transform.position, self.InitPosition);
                    //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 放弃追击返回出生点，设置Walk动画状态");
                    unit.SetAIUnitActionType(UnitActionType.Walk, true);
                }
                return;
            }
        }
        public static void FindTarget(this MonsterMoveComponent self)
        {
            // 如果已经有目标，不需要寻找
            if ((Unit)self.TargetUnit != null)
                return;

            var unit = self.GetParent<Unit>();
            var gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
            if (!gameObject.activeSelf)
                return;

            Unit targetUnit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            GameObject o = targetUnit.GetComponent<GameObjectComponent>().GameObject;
            float distance = Vector3.Distance(gameObject.transform.position, o.transform.position);

            // 修改寻敌范围为2，这样在距离为1时会开始攻击，在距离为2时会开始追击
            if (distance < 2)//(self.AttackFlag)// 
            {
                //Debug.Log($"找到目标玩家，距离: {distance} flag:" +self.AttackFlag);
                self.TargetUnit = targetUnit;

                // 如果距离小于1，直接切换到攻击状态
                if (self.AttackFlag)//(distance < 1)
                {
                    var attackComponent = unit.GetComponent<MonsterAttackComponent>();
                    if (attackComponent != null && !attackComponent.IsCd)
                    {
                        attackComponent.AttackBefore(targetUnit);
                    }
                }
            }
        }
        public static void CheckMove(this MonsterMoveComponent self)
        {
            var unit = self.GetParent<Unit>();
            var transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;

            if ((Unit)self.TargetUnit == null)
            {
                if (!self.IsFinishRandom)
                    return;

                // Debug.Log("随机寻路 ： 随机移动0");
                self.IsFinishRandom = false;
                // 检查是否需要返回出生点
                float distanceToSpawn = Vector3.Distance(transform.position, self.InitPosition);
                if (distanceToSpawn > self.RandomRadius)
                {
                    //    Debug.Log("随机移动： 1");
                    // 如果距离出生点太远，先返回出生点
                    self.Seeker.StartPath(transform.position, self.InitPosition);
                    //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 返回出生点，设置Walk动画状态");
                    unit.SetAIUnitActionType(UnitActionType.Walk, true);
                }
                else
                {
                    //    Debug.Log("随机移动： 2");
                    // 在出生点范围内随机移动
                    if (self.Path == null || self.CurrentPoint >= self.Path.vectorPath.Count)
                    {
                        self.RandomStartPath();
                        //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 在出生点范围内随机移动，设置Walk动画状态");
                        unit.SetAIUnitActionType(UnitActionType.Walk, true);
                    }
                }
            }
            else
            {
                // Debug.Log("随机寻路 ： 随机移动11");
                // 朝向目标单位移动
                Unit targetUnit = (Unit)self.TargetUnit;
                var targetTransform = targetUnit.GetComponent<GameObjectComponent>().GameObject.transform;
                Vector3 targetPosition = targetTransform.position;
                self.Seeker.StartPath(transform.position, targetPosition);
                //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 追击目标，设置Walk动画状态");
                unit.SetAIUnitActionType(UnitActionType.Walk, true);
            }
        }
        public static void RandomStartPath(this MonsterMoveComponent self)
        {
            if (!self.IsFinishRandom)
                return;

            //Debug.Log("开始随机移动");
            self.IsFinishRandom = false;  // 标记为正在寻路中
            var unit = self.GetParent<Unit>();
            var transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;
            // 获取一个在出生点范围内的随机点
            Vector3 randomPoint = self.GetRandomWalkablePoint();
            //Debug.Log($"随机寻路目标点: {randomPoint}");
            self.Seeker.StartPath(transform.position, randomPoint);
            //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 开始随机移动，设置Walk动画状态");
            unit.SetAIUnitActionType(UnitActionType.Walk, true);
        }
        public static Vector3 GetRandomWalkablePoint(this MonsterMoveComponent self)
        {
            for (int i = 0; i < 30; i++) // 最多尝试30次
            {
                // 使用InitPosition作为中心点生成随机位置
                Vector3 randomPoint = self.InitPosition + UnityEngine.Random.insideUnitSphere * self.RandomRadius;
                //Debug.Log($"出生点: {self.InitPosition}");
                randomPoint.y = self.InitPosition.y; // 保持在同一平面
                // 检查该点是否可行走
                GraphNode node = AstarPath.active.GetNearest(randomPoint).node;
                if (node.Walkable)
                {
                    //Debug.Log($"随机点: {randomPoint}");
                    return (Vector3)node.position;
                }
            }
            // Debug.Log($"找不到可行走点，返回当前位置: {self.InitPosition}");
            return self.InitPosition; // 如果找不到可行走点，返回当前位置
        }
        public static void SeekFunc(this MonsterMoveComponent self)
        {
            if (self.Path == null)
                return;


            var unit = self.GetParent<Unit>();
            var transform = unit.GetComponent<GameObjectComponent>().GameObject.transform;
            // 检查是否到达路径终点
            if (self.CurrentPoint >= self.Path.vectorPath.Count || self.AttackFlag)
            {
                self.CurrentPoint = 0;
                self.Seeker.CancelCurrentPathRequest();
                self.Path = null;  // 清除当前路径，让CheckMove重新计算
                self.IsFinishRandom = true;
                // 如果有目标，到达终点时尝试攻击
                if ((Unit)self.TargetUnit != null)
                {
                    // 使用统一攻击调用方法，支持所有类型的攻击组件
                    unit.CallAttack(self.TargetUnit);
                }
                else
                {
                    self.RandomStartPath();
                }
                return;
            }

            Vector3 dir = (self.Path.vectorPath[self.CurrentPoint] - transform.position).normalized;
            dir *= .2f * Time.deltaTime;

            // 根据移动方向调整朝向
            if (dir.x != 0)
            {
                // 使用Y轴旋转来实现翻转效果
                float targetYRotation = dir.x > 0 ? 0f : 180f;
                Vector3 currentRotation = transform.GetChild(0).localEulerAngles;
                currentRotation.y = targetYRotation;
                transform.GetChild(0).localEulerAngles = currentRotation;
            }
            transform.Translate(dir, Space.World);

            Vector3 offset = transform.position - self.Path.vectorPath[self.CurrentPoint];
            if (offset.sqrMagnitude < 0.01f)
            {
                self.CurrentPoint++;
            }
        }
        public static void OnCallFunc(this MonsterMoveComponent self, Path p)
        {
            if (!p.error)
            {
                self.Path = p;
                Unit unit = self.GetParent<Unit>();
                if (unit == null || unit.IsDisposed)
                    return;
                //Debug.Log($"[怪物移动] 单位ID:{unit.Id} 寻路完成，设置Walk动画状态");
                unit.SetMonsterActionType(MonsterActionType.Walk);
            }
        }

        public static void StopMove(this MonsterMoveComponent self)
        {
            if (self.Seeker != null)
            {
                self.Seeker.CancelCurrentPathRequest();
            }
            self.Path = null;
            self.CurrentPoint = 0;
            self.IsFinishRandom = true;  // 设置为true，这样可以开始新的随机移动
            self.TargetUnit = null;
        }
    }
}