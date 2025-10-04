using System.Collections.Generic;

namespace ET
{
    [ChildOf(typeof(ItemContainerComponent))]
    public class ItemContainer : Entity,IAwake,IDestroy
    {
        public ItemContainerType ContainerType;
        /// <summary>
        /// 当前已经使用的格子的数量
        /// </summary>
        public int CurrentCellCount;
        
        /// <summary>
        /// 容器内的物品,这里只需要记住id就行
        /// </summary>
        public List<long> Items = new List<long>();
        
        /// <summary>
        /// 容器内的物品，按照格子进行存储
        /// </summary>
        public Dictionary<long, long> ItemsByCell = new Dictionary<long,long>();
        /// <summary>
        /// 容器内的物品，按照物品配置ID进行分组
        /// </summary>
        public readonly UnOrderMultiMap<int, long> ItemsByConfigId = new UnOrderMultiMap<int, long>();
        /// <summary>
        /// 容器内的物品，按照物品类型进行分组
        /// </summary>
        public readonly UnOrderMultiMap<ItemType,long> ItemsByType = new UnOrderMultiMap<ItemType, long>();
    }
}