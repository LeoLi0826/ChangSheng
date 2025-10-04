using System.Collections.Generic;

namespace ET
{
    public static partial class ItemContainerHelper
    {
        public static int AddItem(ItemContainerComponent itemContainerComponent, ItemContainerType type, int configId, int count)
        {
            ItemContainer itemContainer = itemContainerComponent.GetContainer(type);
            if (itemContainer == null)
            {
                return -1;
            }
            return AddItem(itemContainerComponent, itemContainer, configId, count);
        }
        
        public static int AddItem(ItemContainerComponent itemContainerComponent, ItemContainer itemContainer, int configId, int count)
        {
            ItemContainerConfig containerConfig = ItemContainerConfigCategory.Instance.Get(itemContainer.ContainerType);
            if (containerConfig == null)
            {
                Log.Error($"[AddItem] 找不到容器配置 containerType:{itemContainer.ContainerType}");
                return -1;
            }
            if (containerConfig.CanCell)
            {
                Log.Error($"[AddItem] 容器类型错误，该方法只适用于普通容器 containerType:{itemContainer.ContainerType}");
                return -1;
            }
            
            if (itemContainer.IsFull())
            {
                return -1;
            }
            
            if (count <= 0)
            {
                Log.Error($"[AddItem] 添加物品数量必须大于0 configId:{configId} count:{count}");
                return -1;
            }

            // 获取物品配置
            ItemConfig itemConfig = ItemConfigCategory.Instance.Get(configId);
            if (itemConfig == null)
            {
                Log.Error($"[AddItem] 找不到物品配置 configId:{configId}");
                return -1;
            }

            bool isSuperposed = true;
            if ((ItemType)itemConfig.Type == ItemType.Weapon)
            {
                //如果是装备那么只能数量为1
                isSuperposed = false;
                count = 1;
            }

            // 获取所属单位
            Unit unit = itemContainerComponent.Parent.GetParent<Unit>();

            // 处理可堆叠物品
            if (isSuperposed)
            {
                // bool isUnlimited = true; // 新增：标记是否无限堆叠

                // 尝试堆叠到现有物品上
                if (itemContainer.ItemsByConfigId.TryGetValue(itemConfig.Id, out var itemList))
                {
                    foreach (long haveId in itemList)
                    {
                        Item haveItem = itemContainerComponent.GetChild<Item>(haveId);
            
                        if (haveItem == null || haveItem.IsDisposed) continue;

                        // 调整可堆叠量计算：无限堆叠时可以添加全部剩余数量
                        int canAdd = count;
                        if (canAdd <= 0) continue;

                        int add = canAdd > count ? count : canAdd;
                        haveItem.Count += add;
                        count -= add;

                        if (count == 0) break;
                    }
                }

                // 剩余数量创建新物品（调整堆叠上限逻辑）
                while (count > 0)
                {
                    if (itemContainer.IsFull())
                    {
                        Log.Warning($"[AddItem] 容器已满，无法添加剩余物品 unit:{unit?.Id} configId:{configId} remainCount:{count}");
                        return -1;
                    }

                    // 无限堆叠时，新物品直接添加全部剩余数量（无需限制superposedMax）
                    int add = count;
                    Item item = itemContainerComponent.AddItem(itemConfig, add);
                    itemContainer.ItemJoinContainer(item);
                    count -= add;
                }
            }
            else // 不可堆叠物品
            {
                int addedCount = 0; // 记录已成功添加的数量
                List<Item> createdItems = new List<Item>(); // 记录创建的物品，用于回滚

                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (itemContainer.IsFull()) // 每次添加前检查容器是否已满
                        {
                            Log.Warning($"[AddItem] 容器已满，仅添加了{addedCount}/{count}个物品 unit:{unit?.Id} configId:{configId}");
                            return addedCount > 0 ? -1 : -2;
                        }

                        // 创建数量为1的不可堆叠物品实例
                        Item item = itemContainerComponent.AddItem(itemConfig, 1);
                        if (item == null)
                        {
                            Log.Error($"[AddItem] 创建物品失败 unit:{unit?.Id} configId:{configId} index:{i}");
                            break;
                        }

                        // 加入容器
                        itemContainer.ItemJoinContainer(item);
                        createdItems.Add(item);
                        addedCount++;
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error($"[AddItem] 添加不可堆叠物品异常 unit:{unit?.Id} configId:{configId} error:{ex}");

                    // 异常时回滚已创建的物品
                    foreach (var createdItem in createdItems)
                    {
                        if (createdItem != null && !createdItem.IsDisposed)
                        {
                            itemContainer.ItemLeaveContainer(createdItem);
                            createdItem.Dispose();
                        }
                    }

                    return -1;
                }
            }
            return 0;
        }
    }
}

