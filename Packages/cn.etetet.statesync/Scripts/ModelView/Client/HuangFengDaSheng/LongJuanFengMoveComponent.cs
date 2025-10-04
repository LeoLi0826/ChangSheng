using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class LongJuanFengMoveComponent : Entity, IAwake<Vector3>, IUpdate, IDestroy
    {
        public Seeker Seeker;
        public Path Path;
        public int CurrentPoint;
        public float RandomRadius;
        public Vector3 InitPosition;
        public EntityRef<Unit> TargetUnit;
        public Collider[] Colliders;
        public bool IsFinishRandom;

        public bool AttackFlag;

        // 龙卷风与黄风大圣的距离限制
        public float MaxDistanceFromBoss = 7f;        // 最大允许距离黄风大圣的距离
        public EntityRef<Unit> BossUnit;              // 黄风大圣的引用

        // 防汇聚：每个龙卷风的个人偏移与分离配置
        public Vector3 PersonalOffset;                // 相对Boss的个人偏移（在生成时设置）
        public float OrbitRadius = 2.5f;              // 围绕Boss的基础半径
        public float SeparationRadius = 1.2f;         // 与其他龙卷风的最小间隔，避免汇聚
    }
}