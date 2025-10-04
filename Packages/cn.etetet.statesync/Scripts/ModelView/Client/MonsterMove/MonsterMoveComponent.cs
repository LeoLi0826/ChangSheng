using Pathfinding;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class MonsterMoveComponent : Entity, IAwake<Vector3>, IUpdate, IDestroy
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

    }
}