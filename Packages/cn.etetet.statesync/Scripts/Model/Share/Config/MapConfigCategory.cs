using System.Collections.Generic;

namespace ET
{
    public partial class MapConfigCategory
    {
        public Dictionary<int, List<MapConfig>> spawnMapObjectConfigDic = new Dictionary<int, List<MapConfig>>();


        public List<MapConfig> GetListByMapVertexType(int mapVertexType)
        {
            if (spawnMapObjectConfigDic.TryGetValue(mapVertexType, out List<MapConfig> list))
            {
                return list;
            }

            return null;
        }
        
        public override void EndInit()
        {
            foreach (MapConfig config in this.DataList)
            {
                if (spawnMapObjectConfigDic.TryGetValue(config.MapVertexType, out List<MapConfig> list))
                {
                    list.Add(config);
                }
                else
                {
                    list = new List<MapConfig>();
                    list.Add(config);
                    spawnMapObjectConfigDic.Add(config.MapVertexType, list);
                }
            }
        }
    }
}