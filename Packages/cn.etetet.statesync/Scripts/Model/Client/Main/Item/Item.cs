namespace ET
{
    [ChildOf(typeof(ItemContainerComponent))]
    public class Item : Entity, IAwake<int>, IDestroy,ISerializeToEntity
    {
        //物品配置ID
        public int ConfigId = 0;
        
        public int EquipPositon = 0;
        
        //Fish需要的参数
        public int state = 0;//状态：0 正常模式 1 添加模式 2空模式 3锁定模式
       
        // public string Name = "";
        // public string ItemIcon = "";
        public int Price = 0;
        public int Count = 1;
        public int Score = 0;
        
        /// <summary>
        /// 物品品质 白 蓝 紫 橙 红 等级越高 效果提升越好
        /// </summary>
        public int Quality = 0;
        public int Rarity = 0; //物品稀有度：0 普通 1 稀有 2 史诗 3 传说 4 神话
        
        public int LabelState = 0; //图鉴状态：0 从未获得过 1 已获得
        public int CollectState; //收藏状态：0 正常物品类目 1 可收藏
        
        public ItemType itemType; //物品类型

        public int index;

        public ItemContainerType ContainerType = ItemContainerType.None;
        
        //物品配置数据
        public ItemConfig config => ItemConfigCategory.Instance.Get(ConfigId);

    }
}