namespace ET
{
    [EntitySystemOf(typeof(Unit))]
    public static partial class UnitSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Unit self)
        {

        }
        [EntitySystem]
        private static void Awake(this Unit self, int configId)
        {
            self.ConfigId = configId;
        }

        public static UnitConfig Config(this Unit self)
        {
            return UnitConfigCategory.Instance.Get(self.ConfigId);
        }
        
        public static void SetUnitActionType(this Unit self, UnitActionType type, bool loop = true)
        {
            self.unitActionType = type;

            EventSystem.Instance.PublishAsync(self.Root(), new ChangeAnimation() { changeType = type, changeLoop = loop, changeUnit = self }).NoContext();

        }
        
        //AI敌人更新状态机
        public static void SetAIUnitActionType(this Unit self, UnitActionType type, bool loop = true)
        {
            if (self.unitActionType == type)
                return;
            self.unitActionType = type;

            EventSystem.Instance.Publish(self.Root(), new ChangeAIAnimation() { changeType = type, changeLoop = loop, changeUnit = self });
        }
        public static void SetMonsterActionType(this Unit self, MonsterActionType type, bool loop = true)
        {
            self.MonsterActionType = type;

            //EventSystem.Instance.PublishAsync(self.Root(), new ChangeAIAnimation(){ changeType=type ,changeLoop = loop, changeUnit=self}).Coroutine();
        }
        //get
        public static MonsterActionType GetMonsterActionType(this Unit self)
        {
            return self.MonsterActionType;
        }

        public static bool IsAlive(this Unit self)
        {
            if (self == null || self.IsDisposed)
                return false;
            NumericComponent numericComponent = self.GetComponent<NumericComponent>();
            long hp = numericComponent[NumericType.Hp];
            return hp > 0;
        }
    }
}