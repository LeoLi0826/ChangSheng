using System.Collections.Generic;

namespace ET
{
    public partial class TalismanConfigCategory
    {
        public void GetSatisfyElements(Dictionary<ItemElementAttr,int> elements,List<TalismanConfig> configs,List<int> weights)
        {
            foreach (var config in this.GetAll().Values)
            {
                elements.TryGetValue(ItemElementAttr.Huo, out int huo);
                if (config.Huo > huo)
                {
                    continue;
                }

                elements.TryGetValue(ItemElementAttr.Bing, out int bing);
                if (config.Bing > bing)
                {
                    continue;
                }

                elements.TryGetValue(ItemElementAttr.Feng, out int feng);
                if (config.Feng > feng)
                {
                    continue;
                }
                
                elements.TryGetValue(ItemElementAttr.Lei,out int lei);
                if (config.Lei > lei)
                {
                    continue;
                }

                elements.TryGetValue(ItemElementAttr.Shui, out int shui);
                if (config.Shui > shui)
                {
                    continue;
                }
                configs.Add(config);
                weights.Add(config.Weight);
            }
        }
    }
}