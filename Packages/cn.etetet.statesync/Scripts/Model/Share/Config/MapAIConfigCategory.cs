using System.Collections.Generic;

namespace ET
{
    public partial class MapAIConfigCategory
    {
        public Dictionary<int, List<MapAIConfig>> spawnMapObjectConfigDic = new Dictionary<int, List<MapAIConfig>>();


        public List<MapAIConfig> GetListByMapVertexType(int mapVertexType)
        {
            if (spawnMapObjectConfigDic.TryGetValue(mapVertexType, out List<MapAIConfig> list))
            {
                return list;
            }

            return null;
        }
        
        public override void EndInit()
        {
            foreach (MapAIConfig config in this.DataList)
            {
                if (spawnMapObjectConfigDic.TryGetValue(config.MapVertexType, out List<MapAIConfig> list))
                {
                    list.Add(config);
                }
                else
                {
                    list = new List<MapAIConfig>();
                    list.Add(config);
                    spawnMapObjectConfigDic.Add(config.MapVertexType, list);
                }
            }
        }
    }
}

