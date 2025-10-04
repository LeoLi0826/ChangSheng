namespace ET
{

    public struct StartGame
    {
        
    }
    
    public struct SceneChangeStart
    {
    }
    
    public struct SceneChangeFinish
    {
    }
    
    public struct AfterCreateClientScene
    {
    }
    
    public struct AfterCreateCurrentScene
    {
    }

    public struct AppStartInitFinish
    {
    }

    public struct EnterMapFinish
    {
    }

    public struct AfterUnitCreateGetView
    {
        public Unit Unit;
    }
    
    public struct AfterUnitCreate
    {
        public Unit Unit;
    }
    
    public struct ChangeAIAnimation
    {
        public UnitActionType changeType;
        public bool changeLoop;
        public long unitId;
        public Unit changeUnit;
    }
    
    public struct ChangeAnimation
    {
        public UnitActionType changeType;
        public bool changeLoop;
        public long unitId;
        public Unit changeUnit;
    }
    
    public struct AttackEvent
    {
        public Unit Unit;
    }
    
    // 战意条
    public struct ZYAttackEvent
    {
        public Unit Unit;
        public int State;
    }

    // 战意条
    public struct ZYAttackInitEvent
    {
        public Unit Unit;
        public int State;
    }
    
    // 具体的动画事件类型
    public struct AttackStartEvent
    {
        public Unit Unit;
    }
    
    public struct AttackFinishEvent
    {
        public Unit Unit;
    }
    
    public struct AttackUnitStart
    {
        public Unit AttackUnit;
        public Unit TargetUnit;
        public long Damage;
    }
    
    // 动画事件系统
    public struct AnimationEvent
    {
        public Unit Unit;           // 触发事件的Unit
        public string EventName;    // 事件名称 (attack_start, attack_finish, attack等)
        public float EventTime;     // 事件触发时间
    }
    
    public struct PlayerUIDateRefresh
    {
        public NumericComponent date;
    }
    
    public struct EventThrowingCheckEnergy
    {
        public bool flag;
    }
    
    public struct NumericReFresh
    {
        public NumericComponent date;
    }
    
    public struct BagItemViewFresh
    {
        
    }
    
    public struct ManaMaxAddRefresh
    {
        public int date;
    }

    public struct ManaReduceRefresh
    {
        public int date;
    }
    
    public struct LifeRefresh
    {
    }
    
    public struct ManaAddRefresh
    {
        public int date;
    }
    
    public struct CalamityAdd
    {
        public int day;
    }

    public struct CalamityReduce
    {
        public int day;
    }
    
    public struct ForgeItemWindowFresh
    {
        //public Item itemTemp;
    }
    
    //刷新完成品格子
    public struct EventForgeReadyReFresh
    {
        public int A;
    }
    
    //刷新完成品格子
    public struct EventElementReFresh
    {
        public int A;
    }
    
    public struct EventQuickItemReFresh
    {
        public int A;
    }

    //刷新格子
    public struct EventForgeItemReFresh
    {
        public int A;
    }
}