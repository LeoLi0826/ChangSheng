using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf]
    public class AttackComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public PlayerBehaviour playerBehaviour;
        public EntityRef<Unit> unit;
        public UnitConfig unitConfig;
        public Collider AttackCollider;
        public long unitId;

        public int AttackState;  // 0 表示没有攻击 1 表示玩家攻击 2表示怪物攻击
        public int PickState;

        public int AttackBuff;
        
        // 攻击防抖字典
        public Dictionary<string, float> LastAttackTime = new Dictionary<string, float>();
    }
}