using System.Collections.Generic;

namespace ET
{
    [FriendOfAttribute(typeof(ET.ItemContainer))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemContainerHelper
    {
        /// <summary>
        /// 移除指定数量的物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器</param>
        /// <param name="itemId">物品ID</param>
        /// <param name="count">数量</param>
        /// <param name="itemReason">操作原因</param>
        /// <param name="changeList">变更物品列表</param>
        /// <returns>错误码，0表示成功</returns>
        public static int RemoveItem(ItemContainerComponent itemContainerComponent, ItemContainerType type, long itemId, int count)
        {
            ItemContainer itemContainer = itemContainerComponent.GetContainer(type);
            if (itemContainer == null)
            {
                Log.Error($"[RemoveItem] 容器不存在 containerType:{type} itemId:{itemId}");
                return -1;
            }

            if (!itemContainer.Items.Contains(itemId))
            {
                Log.Warning($"[RemoveItem] 物品不在容器中 containerType:{type} itemId:{itemId}");
                return -1;
            }

            Item item = itemContainerComponent.GetItem(itemId);
            if (item == null)
            {
                Log.Warning($"[RemoveItem] 找不到物品实例 itemId:{itemId}");
                return -1;
            }

            return RemoveItem(itemContainerComponent, itemContainer, item, count);
        }

        public static int RemoveItem(ItemContainerComponent itemContainerComponent, ItemContainer itemContainer, long itemId, int count)
        {
            if (!itemContainer.Items.Contains(itemId))
            {
                Log.Warning($"[RemoveItem] 物品不在容器中 containerId:{itemContainer.Id} itemId:{itemId}");
                return -1;
            }

            Item item = itemContainerComponent.GetItem(itemId);
            if (item == null)
            {
                Log.Warning($"[RemoveItem] 找不到物品实例 itemId:{itemId}");
                return -1;
            }

            return RemoveItem(itemContainerComponent, itemContainer, item, count);
        }

        public static int RemoveItem(ItemContainerComponent itemContainerComponent, Item item, int count)
        {
            if (item == null)
            {
                Log.Warning($"[RemoveItem] 物品实例为空");
                return -1;
            }

            ItemContainer itemContainer = itemContainerComponent.GetContainer(item.ContainerType);
            if (itemContainer == null)
            {
                Log.Warning($"[RemoveItem] 找不到物品容器 containerType:{item.ContainerType} itemId:{item.Id}");
                return -1;
            }

            if (!itemContainer.Items.Contains(item.Id))
            {
                Log.Warning($"[RemoveItem] 物品不在容器中 containerType:{item.ContainerType} itemId:{item.Id}");
                return -1;
            }

            return RemoveItem(itemContainerComponent, itemContainer, item, count);
        }

        /// <summary>
        /// 核心删除方法 - 从容器中移除指定数量的物品
        /// </summary>
        /// <param name="itemContainerComponent">物品容器组件</param>
        /// <param name="itemContainer">目标容器</param>
        /// <param name="item">物品实例</param>
        /// <param name="count">删除数量</param>
        /// <param name="itemReason">删除原因</param>
        /// <param name="updateItems">更新消息对象</param>
        /// <returns>操作结果错误码</returns>
        public static int RemoveItem(ItemContainerComponent itemContainerComponent, ItemContainer itemContainer, Item item, int count)
        {
            if (item == null || item.IsDisposed)
            {
                Log.Warning($"[RemoveItem] 物品实例无效");
                return -1;
            }

            if (count <= 0)
            {
                Log.Warning($"[RemoveItem] 删除数量无效 itemId:{item.Id} count:{count}");
                return -1;
            }

            // 获取单位信息用于日志
            Unit unit = itemContainerComponent.Parent.GetParent<Unit>();
            int originalCount = item.Count;

            // 数量不足时自动调整为实际数量
            if (item.Count < count)
            {
                Log.Warning($"[RemoveItem] 物品数量不足，调整删除数量 unit:{unit?.Id} itemId:{item.Id} itemCount:{item.Count} requestCount:{count}");
                count = item.Count;
            }

            // 减少物品数量
            item.Count -= count;



            // 物品数量为0时从容器中移除并销毁
            // bool itemDestroyed = false;
            if (item.Count <= 0)
            {
                itemContainer.ItemLeaveContainer(item);
                item.Dispose();
                // itemDestroyed = true;
            }

            return -1;
        }
    }
}

