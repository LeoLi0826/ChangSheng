using System.Collections.Generic;

namespace ET
{

    [EnableClass]
    public class ItemElementAttrData
    {
        public ItemElementAttr ItemElementAttr = ItemElementAttr.None;
        public int Value;
    }
    public partial class ItemConfigCategory
    {
        public Dictionary<int,ItemElementAttrData> elements = new Dictionary<int, ItemElementAttrData>();

        public ItemElementAttrData GetElement(int configId)
        {
            if (this.elements.TryGetValue(configId, out var element))
            {
                return element;
            }

            return null;
        }

        public override void EndInit()
        {
            foreach (ItemConfig itemConfig in this.DataList)
            {
                ItemElementAttrData data = new ItemElementAttrData();
                if (itemConfig.Huo > 0)
                {
                    data.ItemElementAttr = ItemElementAttr.Huo;
                    data.Value = itemConfig.Huo;
                }

                if (itemConfig.Bing > 0)
                {
                    data.ItemElementAttr = ItemElementAttr.Bing;
                    data.Value = itemConfig.Bing;
                }

                if (itemConfig.Feng > 0)
                {
                    data.ItemElementAttr = ItemElementAttr.Feng;
                    data.Value = itemConfig.Feng;
                }
                if (itemConfig.Lei > 0)
                {
                    data.ItemElementAttr = ItemElementAttr.Lei;
                    data.Value = itemConfig.Lei;
                } 
                
                if (itemConfig.Shui > 0)
                {
                    data.ItemElementAttr = ItemElementAttr.Shui;
                    data.Value = itemConfig.Shui;
                }
                elements.Add(itemConfig.Id,data);
            }
        }
    }
}

