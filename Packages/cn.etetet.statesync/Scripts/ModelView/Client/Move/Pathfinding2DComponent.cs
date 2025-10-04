using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 2D寻路组件，封装AStarPro的Seeker，使用ETTask消除回调
    /// </summary>
    [ComponentOf(typeof(Unit))]
    public class Pathfinding2DComponent : Entity, IAwake<GameObject>, IDestroy
    {
        /// <summary>
        /// AStarPro的Seeker组件
        /// </summary>
        public Seeker Seeker;

        /// <summary>
        /// 当前寻路任务
        /// </summary>
        public ETTask<Path> PathfindingTask;
    }
}

