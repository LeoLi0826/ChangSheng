using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 物品容器系统 - 管理单个容器的生命周期和操作
    /// </summary>
    [EntitySystemOf(typeof(ItemContainer))]
    [FriendOfAttribute(typeof(ET.ItemContainer))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemContainerSystem
    {
        /// <summary>
        /// 容器初始化
        /// </summary>
        [EntitySystem]
        private static void Awake(this ItemContainer self)
        {
            // 初始化容器的各种集合
            self.Items ??= new List<long>();
            self.ItemsByCell ??= new Dictionary<long, long>();

            Log.Debug($"[ItemContainerSystem.Awake] 物品容器初始化完成 containerId:{self.Id} containerType:{self.ContainerType}");
        }

        /// <summary>
        /// 容器销毁清理
        /// </summary>
        [EntitySystem]
        private static void Destroy(this ItemContainer self)
        {
            // 清理所有集合数据
            self.Items?.Clear();
            self.ItemsByCell?.Clear();
            self.ItemsByConfigId?.Clear();
            self.ItemsByType?.Clear();

            Log.Debug($"[ItemContainerSystem.Destroy] 物品容器销毁完成 containerId:{self.Id} containerType:{self.ContainerType}");
        }

        #region 容器状态查询方法

        /// <summary>
        /// 检查容器是否已满
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <returns>true表示已满</returns>
        public static bool IsFull(this ItemContainer self)
        {
            ItemContainerConfig config = ItemContainerConfigCategory.Instance.Get(self.ContainerType);
            if (config == null)
            {
                Log.Error($"[ItemContainerSystem.IsFull] 容器配置不存在 containerId:{self.Id} containerType:{self.ContainerType}");
                return true; // 配置不存在时认为已满，防止异常操作
            }

            return self.Items.Count >= config.CellCount;
        }

        /// <summary>
        /// 检查容器是否包含指定物品
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>true表示包含该物品</returns>
        public static bool HasItem(this ItemContainer self, long itemId)
        {
            return self.Items != null && self.Items.Contains(itemId);
        }

        /// <summary>
        /// 检查指定格子是否有物品
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="cellId">格子ID</param>
        /// <param name="itemId">输出的物品ID</param>
        /// <returns>true表示该格子有物品</returns>
        public static bool HasItemByCell(this ItemContainer self, long cellId, out long itemId)
        {
            itemId = 0;
            return self.ItemsByCell != null && self.ItemsByCell.TryGetValue(cellId, out itemId);
        }

        #endregion

        #region 物品查询方法

        /// <summary>
        /// 获取指定配置ID的所有物品ID列表
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="configId">物品配置ID</param>
        /// <param name="itemIds">输出的物品ID列表</param>
        /// <returns>true表示找到物品</returns>
        public static bool GetItemsByConfigId(this ItemContainer self, int configId, out List<long> itemIds)
        {
            itemIds = null;
            return self.ItemsByConfigId != null && self.ItemsByConfigId.TryGetValue(configId, out itemIds);
        }

        /// <summary>
        /// 获取指定类型的所有物品ID列表
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="itemType">物品类型</param>
        /// <param name="itemIds">输出的物品ID列表</param>
        /// <returns>true表示找到物品</returns>
        public static bool GetItemsByType(this ItemContainer self, ItemType itemType, out List<long> itemIds)
        {
            itemIds = null;
            return self.ItemsByType != null && self.ItemsByType.TryGetValue(itemType, out itemIds);
        }

        /// <summary>
        /// 通过格子ID获取物品ID
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="cellId">格子ID</param>
        /// <returns>物品ID，不存在则返回0</returns>
        public static long GetItemByCellId(this ItemContainer self, int cellId)
        {
            if (self.ItemsByCell != null && self.ItemsByCell.TryGetValue(cellId, out long itemId))
            {
                return itemId;
            }

            return 0;
        }

        #endregion

        #region 物品进出容器方法

        /// <summary>
        /// 物品加入容器
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="item">物品实例</param>
        /// <param name="skipBaseAdd">是否跳过基础添加（用于反序列化时）</param>
        public static void ItemJoinContainer(this ItemContainer self, Item item, bool skipBaseAdd = false)
        {
            if (item == null)
            {
                Log.Error($"[ItemContainerSystem.ItemJoinContainer] 物品为空 containerId:{self.Id}");
                return;
            }

            ItemConfig config = ItemConfigCategory.Instance.Get(item.ConfigId);
            if (config == null)
            {
                Log.Error($"[ItemContainerSystem.ItemJoinContainer] 物品配置不存在 containerId:{self.Id} itemId:{item.Id} configId:{item.ConfigId}");
                return;
            }

            // 添加到基础物品列表
            if (!skipBaseAdd)
            {
                if (self.Items.Contains(item.Id))
                {
                    Log.Warning($"[ItemContainerSystem.ItemJoinContainer] 物品已存在于容器中 containerId:{self.Id} itemId:{item.Id}");
                    return;
                }

                self.Items.Add(item.Id);
            }

            // 更新物品的容器归属
            item.ContainerType = self.ContainerType;

            // 添加到格子索引
            if (item.index != 0)
            {
                if (self.ItemsByCell.ContainsKey(item.index))
                {
                    Log.Warning(
                        $"[ItemContainerSystem.ItemJoinContainer] 格子已被占用 containerId:{self.Id} cellId:{item.index} existingItemId:{self.ItemsByCell[item.index]} newItemId:{item.Id}");
                }

                self.ItemsByCell[item.index] = item.Id;
            }

            // 添加到配置ID索引
            self.ItemsByConfigId.Add(config.Id, item.Id);

            // 添加到类型索引
            self.ItemsByType.Add((ItemType)config.Type, item.Id);

            Log.Debug($"[ItemContainerSystem.ItemJoinContainer] 物品加入容器成功 containerId:{self.Id} itemId:{item.Id} configId:{item.ConfigId}");
        }

        /// <summary>
        /// 物品离开容器
        /// </summary>
        /// <param name="self">容器实例</param>
        /// <param name="item">物品实例</param>
        public static void ItemLeaveContainer(this ItemContainer self, Item item)
        {
            if (item == null)
            {
                Log.Error($"[ItemContainerSystem.ItemLeaveContainer] 物品为空 containerId:{self.Id}");
                return;
            }

            ItemConfig config = ItemConfigCategory.Instance.Get(item.ConfigId);
            if (config == null)
            {
                Log.Error($"[ItemContainerSystem.ItemLeaveContainer] 物品配置不存在 containerId:{self.Id} itemId:{item.Id} configId:{item.ConfigId}");
                return;
            }

            if (!self.Items.Contains(item.Id))
            {
                Log.Warning($"[ItemContainerSystem.ItemLeaveContainer] 物品不在容器中 containerId:{self.Id} itemId:{item.Id}");
                return;
            }

            // 清除物品的容器归属
            item.ContainerType = ItemContainerType.None;

            // 从基础物品列表中移除
            self.Items.Remove(item.Id);

            // 从格子索引中移除
            if (item.index != 0)
            {
                self.ItemsByCell.Remove(item.index);
                item.index = 0;
            }

            // 从配置ID索引中移除
            if (self.ItemsByConfigId.TryGetValue(item.ConfigId, out var configIdList))
            {
                configIdList.Remove(item.Id);
            }

            // 从类型索引中移除
            if (self.ItemsByType.TryGetValue((ItemType)config.Type, out var typeList))
            {
                typeList.Remove(item.Id);
            }

            Log.Debug($"[ItemContainerSystem.ItemLeaveContainer] 物品离开容器成功 containerId:{self.Id} itemId:{item.Id} configId:{item.ConfigId}");
        }

        #endregion
    }
}