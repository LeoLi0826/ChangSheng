using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf]
    public class MonsterCollisionComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public UnitBehaviour UnitBehaviour;
        public UnitConfig unitConfig;
        
        public PlayerBehaviour playerBehaviour;
        public EntityRef<Unit> unit;
        public Collider AttackCollider;
        public EntityRef<MonsterMoveComponent> MonsterMove;
        public long unitId;

        public int AttackState;  // 0 表示没有攻击 1 表示玩家攻击 2表示怪物攻击
        public int PickState;
    }
}