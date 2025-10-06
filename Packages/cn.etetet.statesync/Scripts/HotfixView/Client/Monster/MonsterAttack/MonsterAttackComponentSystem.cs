using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(MonsterAttackComponent))]
    [FriendOfAttribute(typeof(ET.Client.MonsterAttackComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    public static partial class MonsterAttackComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MonsterAttackComponent self)
        {
            self.IsCd = false;
            
            // 订阅动画事件
            self.SubscribeAnimationEvents();
        }
        [EntitySystem]
        private static void Destroy(this ET.Client.MonsterAttackComponent self)
        {
            self.Target = default;
            TimerComponent timerComponent = self.Root().GetComponent<TimerComponent>();
            timerComponent.Remove(ref self.TimerId);
        }
        public static void Attack(this MonsterAttackComponent self, Unit unit)
        {
            self.AttackBefore(unit);
            if (self.IsCd)
                return;
        }
        public static void AttackBefore(this MonsterAttackComponent self, Unit target)
        {
            self.Target = target;
            Unit unit = self.GetParent<Unit>();
            unit.SetMonsterActionType(MonsterActionType.Attack);
            
        }

        
        
        [EntitySystem]
        private static void Update(this ET.Client.MonsterAttackComponent self)
        {
            //没死 攻击
            if (self.IsCd)
            {
               // Debug.Log("怪物attack：444 ");
                return;
            }
            Unit unit = self.GetParent<Unit>();
            if (unit.GetMonsterActionType() != MonsterActionType.Attack)
                return;
            //Debug.Log("怪物attack：111 ");
            Unit target = self.Target;
            if (target == null || target.IsDisposed)
            {
                self.Target = default;
                unit.SetAIUnitActionType(UnitActionType.Idle);
                unit.SetMonsterActionType(MonsterActionType.Walk);
               // Debug.Log("怪物attack：222 ");
                return;
            }
            // NumericComponent numericComponent = target.GetComponent<NumericComponent>();
            // long hp = numericComponent[NumericType.Hp];
            // Debug.Log("怪物attack：HP "+hp);
            if (!target.IsAlive())
            {
                self.Target = default;
                unit.SetMonsterActionType(MonsterActionType.Walk);
                unit.SetAIUnitActionType(UnitActionType.Walk);
                Debug.Log("怪物attack：333 ");
                return;
            }

            //判断是否大于攻击距离
            var timerComponent = self.Root().GetComponent<TimerComponent>();
            if (!self.AttackFlag)
            {
                //超出范围
                self.Target = default;
                unit.SetAIUnitActionType(UnitActionType.Walk);
                unit.SetMonsterActionType(MonsterActionType.Walk);
                timerComponent.Remove(ref self.TimerId);
                // Debug.LogWarning("超出仇恨范围！！！");
                // Debug.LogWarning("玩家动画机状态"+unit.unitActionType.ToString());
                // Debug.LogWarning("怪物动画机状态"+unit.MonsterActionType.ToString());
                return;
            }

            self.IsCd = true;

            // 小怪物只有普通攻击
            Debug.Log("小怪物执行普通攻击");
            self.TimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + 2000, TimerInvokeType.AttackCdTimer, self);
            unit.SetAIUnitActionType(UnitActionType.Attack);
            
            Debug.LogError("attack");
        }

        // 订阅动画事件
        private static void SubscribeAnimationEvents(this MonsterAttackComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            Debug.Log($"小怪物[{unit.Id}]订阅动画事件");
        }

    }
    
    // 小怪物普通攻击计时器
    [Invoke(TimerInvokeType.AttackCdTimer)]
    [FriendOfAttribute(typeof(ET.Client.MonsterAttackComponent))]
    public class MonsterAttackComponent_AttackTimer : ATimer<MonsterAttackComponent>
    {
        protected override void Run(MonsterAttackComponent self)
        {
            if (self == null || self.IsDisposed)
                return;
            var unit = self.GetParent<Unit>();
            if (unit == null || unit.IsDisposed)
                return;
            self.IsCd = false;
            unit.SetAIUnitActionType(UnitActionType.Idle);
            unit.SetMonsterActionType(MonsterActionType.Attack);
        }
    }

    // 小怪物攻击事件处理器
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.MonsterAttackComponent))]
    public class MonsterAttackEvent_Handler : AEvent<Scene, AttackEvent>
    {
        protected override async ETTask Run(Scene scene, AttackEvent args)
        {
            Unit unit = args.Unit;

            // 只处理拥有MonsterAttackComponent的Unit
            MonsterAttackComponent attackComponent = unit.GetComponent<MonsterAttackComponent>();
            if (attackComponent == null)
                return;

            Debug.Log($"小怪物[{unit.Id}]处理attack事件");

            // 处理普通攻击的血量扣除逻辑
            HandleMonsterNormalAttack(unit, attackComponent);

            await ETTask.CompletedTask;
        }

        private static void HandleMonsterNormalAttack(Unit monsterUnit, MonsterAttackComponent attackComponent)
        {
            // 获取攻击目标(玩家)
            Unit target = attackComponent.Target;
            if (target == null || !target.IsAlive())
            {
                return;
            }

            // 获取怪物的攻击力
            int attackPower = (int)monsterUnit.GetComponent<NumericComponent>()[NumericType.Damage];

            // 统一扣血入口（含振刀判定）
            int power = attackPower == 0 ? 10 : attackPower;
            EventSystem.Instance.PublishAsync(monsterUnit.Root(), new AttackUnitStart()
            {
                AttackUnit = monsterUnit,
                TargetUnit = target,
                Damage = power
            }).NoContext();
        }
    }
}


