using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 物品容器辅助类 - 提供容器和物品的查询、操作方法
    /// </summary>
    public static partial class ItemContainerHelper
    {
        #region 物品查询方法

        /// <summary>
        /// 通过物品ID获取物品实例
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例，不存在则返回null</returns>
        public static Item GetItemById(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, long itemId)
        {
            if (itemContainerComponent == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemById] itemContainerComponent is null");
                return null;
            }

            var container = itemContainerComponent.GetContainer(containerType);
            return GetItemById(itemContainerComponent, container, itemId);
        }

        /// <summary>
        /// 通过物品ID获取物品实例（容器版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品实例，不存在则返回null</returns>
        public static Item GetItemById(ItemContainerComponent itemContainerComponent, ItemContainer container, long itemId)
        {
            if (container == null || !container.HasItem(itemId))
            {
                return null;
            }

            return itemContainerComponent.GetChild<Item>(itemId);
        }

        /// <summary>
        /// 通过格子ID获取物品实例
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="cellId">格子ID</param>
        /// <returns>物品实例，不存在则返回null</returns>
        public static Item GetItemByCell(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, long cellId)
        {
            if (itemContainerComponent == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemByCell] itemContainerComponent is null");
                return null;
            }

            var container = itemContainerComponent.GetContainer(containerType);
            return GetItemByCell(itemContainerComponent, container, cellId);
        }

        /// <summary>
        /// 通过格子ID获取物品实例（容器版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="cellId">格子ID</param>
        /// <returns>物品实例，不存在则返回null</returns>
        public static Item GetItemByCell(ItemContainerComponent itemContainerComponent, ItemContainer container, long cellId)
        {
            if (container == null || !container.HasItemByCell(cellId, out var itemId))
            {
                return null;
            }

            return itemContainerComponent.GetItem(itemId);
        }
        
        /// <summary>
        /// 获取所有物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="configId">物品配置ID</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItems(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, List<EntityRef<Item>> items)
        {
            if (itemContainerComponent == null || items == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemsByConfigId] Invalid parameters");
                return;
            }

            var container = itemContainerComponent.GetContainer(containerType);
            GetItems(itemContainerComponent, container, items);
        }

        /// <summary>
        /// 获取所有物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="configId">物品配置ID</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItems(ItemContainerComponent itemContainerComponent, ItemContainer container, List<EntityRef<Item>> items)
        {
            foreach (long itemId in container.Items)
            {
                Item item = itemContainerComponent.GetItem(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// 获取指定配置ID的所有物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="configId">物品配置ID</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItemsByConfigId(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, int configId, List<Item> items)
        {
            if (itemContainerComponent == null || items == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemsByConfigId] Invalid parameters");
                return;
            }

            var container = itemContainerComponent.GetContainer(containerType);
            GetItemsByConfigId(itemContainerComponent, container, configId, items);
        }

        /// <summary>
        /// 获取指定配置ID的所有物品（容器版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="configId">物品配置ID</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItemsByConfigId(ItemContainerComponent itemContainerComponent, ItemContainer container, int configId, List<Item> items)
        {
            if (container == null || !container.GetItemsByConfigId(configId, out var itemIds))
            {
                return;
            }

            foreach (long itemId in itemIds)
            {
                Item item = itemContainerComponent.GetItem(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// 获取指定类型的所有物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItemsByType(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, ItemType itemType, List<Item> items)
        {
            if (itemContainerComponent == null || items == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemsByType] Invalid parameters");
                return;
            }

            var container = itemContainerComponent.GetContainer(containerType);
            GetItemsByType(itemContainerComponent, container, itemType, items);
        }

        /// <summary>
        /// 获取指定类型的所有物品（容器版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="items">输出的物品列表</param>
        public static void GetItemsByType(ItemContainerComponent itemContainerComponent, ItemContainer container, ItemType itemType, List<Item> items)
        {
            if (container == null || !container.GetItemsByType(itemType, out var itemIds))
            {
                return;
            }

            foreach (long itemId in itemIds)
            {
                Item item = itemContainerComponent.GetItem(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        #endregion

        #region 物品数量统计方法

        /// <summary>
        /// 获取指定配置ID物品的总数量
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="containerType">容器类型</param>
        /// <param name="configId">物品配置ID</param>
        /// <returns>物品总数量</returns>
        public static int GetItemCount(ItemContainerComponent itemContainerComponent, ItemContainerType containerType, int configId)
        {
            if (itemContainerComponent == null)
            {
                Log.Error($"[ItemContainerHelper.GetItemCount] itemContainerComponent is null");
                return 0;
            }

            ItemContainer itemContainer = itemContainerComponent.GetContainer(containerType);
            return GetItemCount(itemContainerComponent, itemContainer, configId);
        }

        /// <summary>
        /// 获取指定配置ID物品的总数量（容器版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="container">目标容器</param>
        /// <param name="configId">物品配置ID</param>
        /// <returns>物品总数量</returns>
        public static int GetItemCount(ItemContainerComponent itemContainerComponent, ItemContainer container, int configId)
        {
            if (container == null || !container.ItemsByConfigId.TryGetValue(configId, out var itemList))
            {
                return 0;
            }

            int totalCount = 0;
            foreach (long itemId in itemList)
            {
                Item item = itemContainerComponent.GetItem(itemId);
                if (item != null)
                {
                    totalCount += item.Count;
                }
            }

            return totalCount;
        }

        #endregion

        #region 物品移动方法

        /// <summary>
        /// 将物品移动到指定容器
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="itemId">要移动的物品ID</param>
        /// <param name="targetContainerType">目标容器类型</param>
        /// <returns>移动结果：0成功，-1失败</returns>
        public static int MoveItem(ItemContainerComponent itemContainerComponent, long itemId,
            ItemContainerType targetContainerType)
        {
            if (itemContainerComponent == null)
            {
                Log.Error($"[ItemContainerHelper.MoveItem] itemContainerComponent is null");
                return -1;
            }

            // 获取物品实例
            Item item = itemContainerComponent.GetItem(itemId);
            if (item == null)
            {
                Log.Error($"[ItemContainerHelper.MoveItem] 找不到物品实例 itemId:{itemId}");
                return -1;
            }

            return MoveItem(itemContainerComponent, item, targetContainerType);
        }

        /// <summary>
        /// 将物品移动到指定容器（物品实例版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="item">要移动的物品实例</param>
        /// <param name="targetContainerType">目标容器类型</param>
        /// <returns>移动结果：0成功，-1失败</returns>
        public static int MoveItem(ItemContainerComponent itemContainerComponent, Item item,
            ItemContainerType targetContainerType)
        {
            if (item == null)
            {
                Log.Error($"[ItemContainerHelper.MoveItem] 物品实例为空");
                return -1;
            }

            // 从物品实例获取当前所在的源容器
            ItemContainer sourceContainer = itemContainerComponent.GetContainer(item.ContainerType);
            if (sourceContainer == null)
            {
                Log.Error($"[ItemContainerHelper.MoveItem] 源容器不存在 sourceContainerType:{item.ContainerType} itemId:{item.Id}");
                return -1;
            }

            // 获取目标容器
            ItemContainer targetContainer = itemContainerComponent.GetContainer(targetContainerType);
            if (targetContainer == null)
            {
                Log.Error($"[ItemContainerHelper.MoveItem] 目标容器不存在 targetContainerType:{targetContainerType}");
                return -1;
            }

            // 如果源容器和目标容器相同，无需移动
            if (sourceContainer == targetContainer)
            {
                Log.Debug($"[ItemContainerHelper.MoveItem] 物品已在目标容器中 itemId:{item.Id} containerType:{targetContainerType}");
                return 0;
            }

            // 检查目标容器是否已满
            if (targetContainer.IsFull())
            {
                Log.Warning($"[ItemContainerHelper.MoveItem] 目标容器已满 targetContainerType:{targetContainerType}");
                return -1;
            }

            // 从源容器中移除物品
            sourceContainer.ItemLeaveContainer(item);

            // 将物品加入目标容器
            targetContainer.ItemJoinContainer(item);

            Log.Debug($"[ItemContainerHelper.MoveItem] 物品移动成功 itemId:{item.Id} from:{sourceContainer.ContainerType} to:{targetContainer.ContainerType}");

            return 0;
        }



        /// <summary>
        /// 交换同一容器中两个物品的位置
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="itemId1">第一个物品ID</param>
        /// <param name="itemId2">第二个物品ID</param>
        /// <returns>交换结果：0成功，-1失败</returns>
        public static int SwapItems(ItemContainerComponent itemContainerComponent, long itemId1, long itemId2)
        {
            if (itemContainerComponent == null)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] itemContainerComponent is null");
                return -1;
            }

            // 获取两个物品实例
            Item item1 = itemContainerComponent.GetItem(itemId1);
            Item item2 = itemContainerComponent.GetItem(itemId2);

            if (item1 == null)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 找不到第一个物品 itemId:{itemId1}");
                return -1;
            }

            if (item2 == null)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 找不到第二个物品 itemId:{itemId2}");
                return -1;
            }

            return SwapItems(itemContainerComponent, item1, item2);
        }

        /// <summary>
        /// 交换同一容器中两个物品的位置（物品实例版本）
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="item1">第一个物品实例</param>
        /// <param name="item2">第二个物品实例</param>
        /// <returns>交换结果：0成功，-1失败</returns>
        public static int SwapItems(ItemContainerComponent itemContainerComponent, Item item1, Item item2)
        {
            if (item1 == null || item2 == null)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 物品实例为空 item1:{item1?.Id} item2:{item2?.Id}");
                return -1;
            }

            // 检查两个物品是否在同一个容器中
            if (item1.ContainerType != item2.ContainerType)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 两个物品不在同一容器中 item1:{item1.Id}({item1.ContainerType}) item2:{item2.Id}({item2.ContainerType})");
                return -1;
            }

            // 获取容器
            ItemContainer container = itemContainerComponent.GetContainer(item1.ContainerType);
            if (container == null)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 容器不存在 containerType:{item1.ContainerType}");
                return -1;
            }

            // 在容器的物品列表中找到两个物品的位置
            int index1 = container.Items.IndexOf(item1.Id);
            int index2 = container.Items.IndexOf(item2.Id);

            if (index1 == -1)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 物品1不在容器的列表中 itemId:{item1.Id}");
                return -1;
            }

            if (index2 == -1)
            {
                Log.Error($"[ItemContainerHelper.SwapItems] 物品2不在容器的列表中 itemId:{item2.Id}");
                return -1;
            }

            // 在容器列表中交换两个物品的位置
            container.Items[index1] = item2.Id;
            container.Items[index2] = item1.Id;

            Log.Debug($"[ItemContainerHelper.SwapItems] 物品交换成功 " +
                     $"item1:{item1.Id} 和 item2:{item2.Id} 在容器 {container.ContainerType} 中交换了位置");

            return 0;
        }

        #endregion
    }
}

