using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 物品容器组件系统 - 管理物品容器组件的生命周期
    /// </summary>
    [EntitySystemOf(typeof(ItemContainerComponent))]
    [FriendOfAttribute(typeof(ET.ItemContainerComponent))]
    [FriendOfAttribute(typeof(ET.ItemContainer))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemContainerComponentSystem
    {
        /// <summary>
        /// 组件初始化
        /// </summary>
        [EntitySystem]
        private static void Awake(this ItemContainerComponent self)
        {
            // 初始化容器列表和CD列表
            self.ContainerList ??= new Dictionary<ItemContainerType, long>();

            Log.Debug($"[ItemContainerComponentSystem.Awake] 物品容器组件初始化完成 entity:{self.Id}");
        }

        /// <summary>
        /// 组件销毁清理
        /// </summary>
        [EntitySystem]
        private static void Destroy(this ItemContainerComponent self)
        {
            // 清理所有容器数据
            self.ContainerList?.Clear();

            Log.Debug($"[ItemContainerComponentSystem.Destroy] 物品容器组件销毁完成 entity:{self.Id}");
        }

        /// <summary>
        /// 重建容器内物品的索引关系
        /// </summary>
        /// <param name="self">物品容器组件</param>
        /// <param name="itemContainer">目标容器</param>
        private static void RebuildContainerItemIndex(ItemContainerComponent self, ItemContainer itemContainer)
        {
            if (itemContainer.Items == null)
            {
                return;
            }

            foreach (long itemId in itemContainer.Items)
            {
                var item = self.GetChild<Item>(itemId);
                if (item == null)
                {
                    Log.Warning($"[ItemContainerComponentSystem.RebuildContainerItemIndex] 物品不存在 containerId:{itemContainer.Id} itemId:{itemId}");
                    continue;
                }

                // 重建物品在容器中的索引关系
                itemContainer.ItemJoinContainer(item, true);
            }
        }

        #region 容器管理方法

        /// <summary>
        /// 获取指定类型的容器，如果不存在则创建
        /// </summary>
        /// <param name="self">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <returns>容器实例</returns>
        public static ItemContainer GetContainer(this ItemContainerComponent self, ItemContainerType containerType)
        {
            // 尝试从已有容器中获取
            if (self.ContainerList.TryGetValue(containerType, out var itemContainerId))
            {
                var existingContainer = self.GetChild<ItemContainer>(itemContainerId);
                if (existingContainer != null)
                {
                    return existingContainer;
                }

                // 容器ID存在但实体不存在，清理无效数据
                Log.Warning(
                    $"[ItemContainerComponentSystem.GetContainer] 发现无效容器ID entity:{self.Id} containerType:{containerType} containerId:{itemContainerId}");
                self.ContainerList.Remove(containerType);
            }

            // 创建新容器
            return CreateContainer(self, containerType);
        }

        /// <summary>
        /// 创建新的容器
        /// </summary>
        /// <param name="self">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <returns>新创建的容器实例</returns>
        private static ItemContainer CreateContainer(ItemContainerComponent self, ItemContainerType containerType)
        {
            // 获取容器配置
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(containerType);
            if (itemContainerConfig == null)
            {
                Log.Error($"[ItemContainerComponentSystem.CreateContainer] 容器配置不存在 entity:{self.Id} containerType:{containerType}");
                return null;
            }

            // 创建容器实例
            ItemContainer itemContainer = self.AddChild<ItemContainer>();
            itemContainer.ContainerType = containerType;
            itemContainer.CurrentCellCount = itemContainerConfig.CellCount;

            // 注册到容器列表
            self.ContainerList[containerType] = itemContainer.Id;

            Log.Debug(
                $"[ItemContainerComponentSystem.CreateContainer] 创建新容器 entity:{self.Id} containerType:{containerType} containerId:{itemContainer.Id}");
            return itemContainer;
        }

        #endregion

        #region 物品管理方法

        /// <summary>
        /// 通过ID获取物品实例
        /// </summary>
        /// <param name="self">物品容器组件</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例</returns>
        public static Item GetItem(this ItemContainerComponent self, long itemId)
        {
            return self.GetChild<Item>(itemId);
        }

        /// <summary>
        /// 创建新的物品实例
        /// </summary>
        /// <param name="self">物品容器组件</param>
        /// <param name="itemConfig">物品配置</param>
        /// <param name="count">物品数量</param>
        /// <returns>新创建的物品实例</returns>
        public static Item AddItem(this ItemContainerComponent self, ItemConfig itemConfig, int count)
        {
            if (itemConfig == null)
            {
                Log.Error($"[ItemContainerComponentSystem.AddItem] 物品配置为空 entity:{self.Id}");
                return null;
            }

            if (count <= 0)
            {
                Log.Error($"[ItemContainerComponentSystem.AddItem] 物品数量无效 entity:{self.Id} configId:{itemConfig.Id} count:{count}");
                return null;
            }

            // 创建物品实例
            Item item = self.AddChild<Item, int>(itemConfig.Id);
            item.Count = count;

            // 根据物品类型添加对应组件
            InitializeItemComponents(item, itemConfig);

            Log.Debug($"[ItemContainerComponentSystem.AddItem] 创建新物品 entity:{self.Id} itemId:{item.Id} configId:{itemConfig.Id} count:{count}");
            return item;
        }

        /// <summary>
        /// 根据物品类型初始化对应的组件
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <param name="itemConfig">物品配置</param>
        private static void InitializeItemComponents(Item item, ItemConfig itemConfig)
        {
            switch ((ItemType)itemConfig.Type)
            {
                case ItemType.Weapon:
                    // item.AddComponent<EquipComponent>();
                    break;

                    // 可以在这里添加其他物品类型的组件初始化
                    // case ItemType.Consumables:
                    //     break;
                    // case ItemType.Material:
                    //     break;
            }
        }

        #endregion
    }
}

