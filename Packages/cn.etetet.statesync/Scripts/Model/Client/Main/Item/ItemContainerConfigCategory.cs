using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public class ItemContainerConfig
    {
        public ItemContainerType ContainerType;
        public int CellCountMax;
        public int CellCount;
        public bool CanCell;
    }
    public class ItemContainerConfigCategory : Singleton<ItemContainerConfigCategory>,ISingletonAwake
    {
        private Dictionary<ItemContainerType, ItemContainerConfig> dic = new Dictionary<ItemContainerType, ItemContainerConfig>();
        public void Awake()
        {
            this.dic.Add(ItemContainerType.Backpack, new ItemContainerConfig()
            {
                ContainerType = ItemContainerType.Backpack,
                CellCountMax = 50,
                CellCount = 20,
                CanCell = false,
            });
            this.dic.Add(ItemContainerType.Fast, new ItemContainerConfig()
            {
                ContainerType = ItemContainerType.Backpack,
                CellCountMax = 10,
                CellCount = 10,
                CanCell = false,
            });
            this.dic.Add(ItemContainerType.Forge,new ItemContainerConfig()
            {
                ContainerType = ItemContainerType.Forge,
                CellCountMax = 6,
                CellCount = 6,
                CanCell = false,
            });
            this.dic.Add(ItemContainerType.ForgeReady,new ItemContainerConfig()
            {
                ContainerType = ItemContainerType.ForgeReady,
                CellCountMax = 20,
                CellCount = 20,
                CanCell = false,
            });
        }

        public ItemContainerConfig Get(ItemContainerType containerType)
        {
            if (this.dic.TryGetValue(containerType, out var ItemContainerConfig))
            {
                return ItemContainerConfig;
            }

            return null;
        }
    }
}

