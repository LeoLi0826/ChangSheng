namespace ET
{
    /// <summary>
    /// 物品项类型
    /// </summary>
    public enum ItemType
    {
        Weapon   = 0, //武器
        Consumable = 1, //消耗品
        Prop     = 2, //道具
        Fish    = 3, //鱼
        LuckyCharm = 4, //护符 幸运符
        Rod = 5, //鱼竿
        Bait = 6, //鱼饵
        Armor    = 7, //防具
        Ring     = 8, //戒指
        Gem      = 9, //宝石
    }

    /// <summary>
    /// 物品操作指令
    /// </summary>
    public enum ItemOp
    {
        Add = 0,  //增加物品
        Remove = 1 //移除物品
    }


    /// <summary>
    /// 物品容器类型
    /// </summary>
    public enum ItemContainerType
    {
        None = 0,  //无
        Backpack = 1, //背包
        Fast = 2, //快捷
        Forge = 3, //合成台
        ForgeReady = 4,//合成台预计
    }

    public enum ItemElementAttr
    {
        None = 0,  //无
        Huo = 1,  // 原Jin (金 → 火)
        Shui = 2, // 原Mu (木 → 水)
        Bing = 3, // 原Shui (水 → 冰)
        Lei = 4,  // 原Huo (火 → 雷)
        Feng = 5, // 原Tu (土 → 风)
    }

    /// <summary>
    /// 物品旋转角度
    /// </summary>
    public enum ItemDir
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }
    
}