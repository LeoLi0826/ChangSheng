using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 2D寻路组件系统，使用ETTask实现异步寻路，消除回调
    /// </summary>
    [EntitySystemOf(typeof(Pathfinding2DComponent))]
    public static partial class Pathfinding2DComponentSystem
    {
        [EntitySystem]
        private static void Awake(this Pathfinding2DComponent self, GameObject gameObject)
        {
            self.Seeker = gameObject.GetComponent<Seeker>();
            if (self.Seeker == null)
            {
                Log.Error($"GameObject {gameObject.name} 上没有找到Seeker组件");
            }
        }

        [EntitySystem]
        private static void Destroy(this Pathfinding2DComponent self)
        {
            // 取消当前寻路任务
            if (self.PathfindingTask != null && !self.PathfindingTask.IsCompleted)
            {
                self.PathfindingTask.SetResult(null);
            }
        }

        /// <summary>
        /// 兼容旧版本的Find方法
        /// </summary>
        /// <param name="self">寻路组件</param>
        /// <param name="start">起始位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="result">寻路结果路径点列表</param>
        /// <returns>寻路是否成功</returns>
        public static async ETTask<bool> Find(this Pathfinding2DComponent self, Vector3 start, Vector3 target, List<Vector3> result)
        {
            if (self.Seeker == null)
            {
                Log.Error("Seeker组件为空，无法进行寻路");
                return false;
            }

            // 取消上一次的寻路请求
            if (self.PathfindingTask != null && !self.PathfindingTask.IsCompleted)
            {
                self.PathfindingTask.SetResult(null);
            }

            if (!AstarPath.active.data.gridGraph.Linecast(start, target))
            {
                //直线
                result.Add(start);
                result.Add(target);
                return true;
            }
            // 创建新的ETTask任务
            self.PathfindingTask = ETTask<Path>.Create(true);

            // 开始寻路，使用回调设置ETTask结果
            self.Seeker.StartPath(start, target, path =>
            {
                // 确保任务还有效且未完成
                if (self.PathfindingTask != null && !self.PathfindingTask.IsCompleted)
                {
                    self.PathfindingTask.SetResult(path);
                }
            });

            // 等待寻路结果
            Path pathResult = await self.PathfindingTask;

            // 检查是否被取消
            if (pathResult == null)
            {
                return false;
            }

            // 检查是否有错误
            if (pathResult.error)
            {
                return false;
            }

            // 填充结果路径点
            result.Clear();
            if (pathResult.vectorPath != null && pathResult.vectorPath.Count > 0)
            {
                result.AddRange(pathResult.vectorPath);
                return true;
            }
            return false;
        }

        public static Vector2 FindRandomPointAroundCircle(this Pathfinding2DComponent self, Vector2 center, float radius)
        {
            Vector2 randomDirection = self.GetRandomInsideUnitCircle();
            if (randomDirection == Vector2.zero)
            {
                randomDirection = new Vector2(RandomGenerator.RandFloat01(), RandomGenerator.RandFloat01()).normalized;
            }
            float randomDistance = RandomGenerator.RandFloat01() * radius;
            Vector2 randomPoint = center + randomDirection.normalized * randomDistance;

            var nearestNodeInfo = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);
            if (nearestNodeInfo.node != null)
            {
                return (Vector2)nearestNodeInfo.position;
            }
            return center;
        }

        /// <summary>
        /// 获取单位圆内的随机点
        /// </summary>
        private static Vector2 GetRandomInsideUnitCircle(this Pathfinding2DComponent self)
        {
            float angle = RandomGenerator.RandFloat01() * 2 * Mathf.PI;
            float radius = Mathf.Sqrt(RandomGenerator.RandFloat01());
            return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        /// <summary>
        /// 在指定环形区域内查找随机点
        /// </summary>
        /// <param name="self">寻路组件</param>
        /// <param name="pos">中心位置</param>
        /// <param name="minRadius">最小半径</param>
        /// <param name="maxRadius">最大半径</param>
        /// <returns>随机点位置</returns>
        public static Vector2 FindRandomPointAroundRing(this Pathfinding2DComponent self, Vector2 pos, float minRadius, float maxRadius)
        {
            Vector2 randomDirection = self.GetRandomInsideUnitCircle();
            if (randomDirection == Vector2.zero)
            {
                randomDirection = new Vector2(RandomGenerator.RandFloat01(), RandomGenerator.RandFloat01()).normalized;
            }

            float randomDistance = minRadius + RandomGenerator.RandFloat01() * (maxRadius - minRadius);
            Vector2 randomPoint = pos + randomDirection.normalized * randomDistance;

            var nearestNodeInfo = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);
            if (nearestNodeInfo.node != null)
            {
                return (Vector2)nearestNodeInfo.position;
            }
            return pos;
        }

        /// <summary>
        /// 查找最近的可行走点
        /// </summary>
        /// <param name="self">寻路组件</param>
        /// <param name="pos">目标位置</param>
        /// <returns>最近的可行走点</returns>
        public static Vector2 RecastFindNearestPoint(this Pathfinding2DComponent self, Vector2 pos)
        {
            var nearestNodeInfo = AstarPath.active.GetNearest(pos, NNConstraint.Default);
            if (nearestNodeInfo.node != null)
            {
                return (Vector2)nearestNodeInfo.position;
            }
            return pos;
        }
    }
}