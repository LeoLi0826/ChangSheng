namespace ET
{
    public static partial class TimerInvokeType
    {
        public const int MapUpdate = PackageType.StateSync * 100 + 1;
        
        public const int EnemyAngry = PackageType.StateSync * 100 + 2;
        
        public const int AttackCdTimer = PackageType.StateSync * 100 + 3;
        
        public const int LocalStorageAutoSave = PackageType.StateSync * 100 + 4;
        public const int AttackCdTimer2 = PackageType.StateSync * 100 + 5;
        public const int AttackCdTimer3 = PackageType.StateSync * 100 + 6;
        public const int ZhendaoWindowTimer = PackageType.StateSync * 100 + 7;
        public const int PlayerAttackCdTimer = PackageType.StateSync * 100 + 8;
        
        
        public const int HuangFengNormalAttackTimer = PackageType.StateSync * 100 + 9;  // 普通攻击CD定时器
        public const int HuangFengChargeAttackTimer = PackageType.StateSync * 100 + 10;  // 蓄力攻击CD定时器
        public const int HuangFengWindUltimateTimer = PackageType.StateSync * 100 + 11;  // 大招定时器(预留)

        public const int BattleWillUpdateTimer = PackageType.StateSync * 100 + 12;
        public const int BattleRound = PackageType.StateSync * 100 + 13;

        public const int Move2DTimer = PackageType.StateSync * 100 + 14;
    }
}

