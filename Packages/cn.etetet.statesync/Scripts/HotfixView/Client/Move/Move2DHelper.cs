
using UnityEngine;

namespace ET.Client
{
    public static class Move2DHelper
    {
     /// <summary>
        /// 寻路并移动到目标点
        /// </summary>
        /// <remarks>
        /// 可以多次连续调用，新的调用会自动取消并覆盖上一次的寻路和移动。
        /// </remarks>
        /// <param name="unit">要移动的Unit</param>
        /// <param name="target">目标位置</param>
        /// <param name="turnTime">转向时间(未使用)</param>
        public static async ETTask FindPathMoveToAsync(this Unit unit, Vector3 target, int turnTime = 100)
        {
            // 检查 Unit 是否有效
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            
            float speed = numericComponent.GetAsFloat(NumericType.Speed);
            if (speed < 0.01)
            {
                return;
            }

            // 使用对象池的列表来存储路径点，避免GC
            using var list = ListComponent<Vector3>.Create();

            var pathfindingComponent = unit.GetComponent<Pathfinding2DComponent>();
            if (pathfindingComponent == null)
            {
                return;
            }

            await pathfindingComponent.Find(unit.Position, target, list);
            // 如果路径点少于2个（例如起点和终点重合），则不移动
            if (list.Count < 2)
            {
                return;
            }

            // 再次检查 Unit 是否仍然有效（异步操作后可能被销毁）
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            Move2DComponent moveComponent = unit.GetComponent<Move2DComponent>();
            if (moveComponent == null || moveComponent.IsDisposed)
            {
                return;
            }

            await moveComponent.MoveToAsync(list, speed, turnTime);
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="unit">要停止的Unit</param>
        /// <param name="error">错误码(0表示正常停止)</param>
        public static void Stop(this Unit unit, int error)
        {
            unit.GetComponent<MoveComponent>().Stop(error == 0);
        }
    }
}

