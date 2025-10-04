using System;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class MonsterAttackComponent : Entity, IAwake, IDestroy, IUpdate
    {
        public bool IsCd;
        public long TimerId;
        public EntityRef<Unit> Target;
        public bool AttackFlag;
    }
}

