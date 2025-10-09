using System.Collections.Generic;

namespace ET
{
    
    /// <summary>
    /// 元素反应类型枚举
    /// </summary>
    public enum ElementReactionType
    {
        None = 0,           // 无反应
        
        // 基础反应 (Base Reactions) - 提供基础分数加成
        Evaporation = 1,    // 蒸发反应 (火+水)
        Freeze = 2,         // 冻结反应 (水+冰)
        Superconduct = 3,   // 超导反应 (雷+冰)
        Storm = 4,          // 风暴反应 (风+水)
        FerventWind = 5,    // 焚风反应 (风+火)
        
        // 聚变反应 (Fusion Reactions) - 提供倍乘加成
        Melt = 6,           // 融化反应 (火+冰)
        ElectroCharged = 7, // 导电反应 (雷+水)
        Blizzard = 8,       // 暴雪反应 (风+冰)
        Overload = 9,       // 超载反应 (火+雷)
        Vaporize = 10,      // 挥发反应 (风+水)
    }

    /// <summary>
    /// 元素反应配置类
    /// </summary>
    [EnableClass]
    public class ElementReactionConfig
    {
        public ElementReactionType ReactionType;    // 反应类型
        public string ReactionName;                 // 反应名称
        public int Ride;                          // 倍乘加成
        public int Multiplier;                    // 反应倍率加成
        public int BaseScore;                       // 反应基础分加成
        public ItemElementAttr Element1;            // 参与反应的元素1
        public ItemElementAttr Element2;            // 参与反应的元素2

        public ElementReactionConfig(ElementReactionType reactionType, string reactionName,
        int multiplier, int baseScore,int ride, ItemElementAttr element1, ItemElementAttr element2)
        {
            ReactionType = reactionType;
            ReactionName = reactionName;
            Multiplier = multiplier;
            Ride = ride;
            BaseScore = baseScore;
            Element1 = element1;
            Element2 = element2;
        }
    }

    
    [CodeProcess]
    public class ElementReactionConfigCategory: Singleton<ElementReactionConfigCategory>,ISingletonAwake
    {
        private Dictionary<ElementReactionType, ElementReactionConfig> configMap = new();
         /// <summary>
        /// 元素反应配置表（配置双向，确保所有元素组合都能正确反应）
        /// </summary>
        private Dictionary<(ItemElementAttr, ItemElementAttr), ElementReactionConfig> ReactionConfigs =
            new Dictionary<(ItemElementAttr, ItemElementAttr), ElementReactionConfig>
        {
            // ========== 基础反应 (Base Reactions) - 提供基础分数加成 +100 ==========
            // 倍率（因为后面要除以10000） 基础灵气分 倍乘
            // 蒸发反应：火+水 和 水+火
            { (ItemElementAttr.Huo, ItemElementAttr.Shui), new ElementReactionConfig(ElementReactionType.Evaporation, "蒸发",0*10000,0,1, ItemElementAttr.Huo, ItemElementAttr.Shui) },

            // 冻结反应：水+冰 和 冰+水
            { (ItemElementAttr.Shui, ItemElementAttr.Bing), new ElementReactionConfig(ElementReactionType.Freeze, "冻结", 0*10000,0,1, ItemElementAttr.Shui, ItemElementAttr.Bing) },

            // 超导反应：雷+冰 和 冰+雷
            { (ItemElementAttr.Lei, ItemElementAttr.Bing), new ElementReactionConfig(ElementReactionType.Superconduct, "超导", 0*10000,0,1, ItemElementAttr.Lei, ItemElementAttr.Bing) },

            // 风暴反应：风+雷 和 雷+风
            { (ItemElementAttr.Feng, ItemElementAttr.Lei), new ElementReactionConfig(ElementReactionType.Storm, "风暴", 0*10000,0,1, ItemElementAttr.Feng, ItemElementAttr.Lei) },

            // 焚风反应：风+火 和 火+风
            { (ItemElementAttr.Feng, ItemElementAttr.Huo), new ElementReactionConfig(ElementReactionType.FerventWind, "焚风",0*10000,0,1,ItemElementAttr.Feng, ItemElementAttr.Huo) },

            // ========== 聚变反应 (Fusion Reactions) - 提供倍乘加成 *2 ==========
            
            // 融化反应：火+冰 和 冰+火
            { (ItemElementAttr.Huo, ItemElementAttr.Bing), new ElementReactionConfig(ElementReactionType.Melt, "融化",  0*10000 , 100 , 0 , ItemElementAttr.Huo, ItemElementAttr.Bing) },

            // 导电反应：雷+水 和 水+雷
            { (ItemElementAttr.Lei, ItemElementAttr.Shui), new ElementReactionConfig(ElementReactionType.ElectroCharged, "导电", 0*10000 , 100 , 0 , ItemElementAttr.Lei, ItemElementAttr.Shui) },

            // 暴雪反应：风+冰 和 冰+风
            { (ItemElementAttr.Feng, ItemElementAttr.Bing), new ElementReactionConfig(ElementReactionType.Blizzard, "暴雪", 0*10000 , 100 , 0 , ItemElementAttr.Feng, ItemElementAttr.Bing) },

            // 超载反应：火+雷 和 雷+火
            { (ItemElementAttr.Huo, ItemElementAttr.Lei), new ElementReactionConfig(ElementReactionType.Overload, "超载", 0*10000 , 100 , 0 , ItemElementAttr.Huo, ItemElementAttr.Lei) },

            // 挥发反应：风+水 和 水+风
            { (ItemElementAttr.Feng, ItemElementAttr.Shui), new ElementReactionConfig(ElementReactionType.Vaporize, "挥发", 0*10000 , 100 , 0 , ItemElementAttr.Feng, ItemElementAttr.Shui) },
        };
         
        public void Awake()
        {
            foreach (var kv in this.ReactionConfigs)
            {
                configMap.Add(kv.Value.ReactionType,kv.Value);
            }
        }
        
        /// <summary>
        /// 根据两个元素属性获取反应配置信息
        /// </summary>
        /// <param name="element1">元素1</param>
        /// <param name="element2">元素2</param>
        /// <returns>反应配置，如果没有反应则返回null</returns>
        public ElementReactionConfig Get(ElementReactionType type)
        {
            if (this.configMap.TryGetValue(type, out var config))
            {
                return config;
            }

            return null;
        }
        
        /// <summary>
        /// 根据两个元素属性获取反应配置信息
        /// </summary>
        /// <param name="element1">元素1</param>
        /// <param name="element2">元素2</param>
        /// <returns>反应配置，如果没有反应则返回null</returns>
        public ElementReactionConfig Get(ItemElementAttr element1, ItemElementAttr element2)
        {
            // 检查是否是相同元素（相同元素无法反应）
            if (element1 == element2)
            {
                return null;
            }

            // 先查找正向配置
            var reactionKey = (element1, element2);
            if (ReactionConfigs.TryGetValue(reactionKey, out ElementReactionConfig config))
            {
                return config;
            }

            // 再查找反向配置
            var reverseKey = (element2, element1);
            if (ReactionConfigs.TryGetValue(reverseKey, out ElementReactionConfig reverseConfig))
            {
                return reverseConfig;
            }

            return null;
        }

        /// <summary>
        /// 检查两个元素是否可以反应
        /// </summary>
        /// <param name="element1">元素1</param>
        /// <param name="element2">元素2</param>
        /// <returns>是否可以反应</returns>
        public bool CanReact(ItemElementAttr element1, ItemElementAttr element2)
        {
            return Get(element1, element2) != null;
        }

        /// <summary>
        /// 获取所有可能的反应类型
        /// </summary>
        /// <returns>所有反应配置的列表</returns>
        public List<ElementReactionConfig> GetAllReactionConfigs()
        {
            var configs = new List<ElementReactionConfig>();
            var addedReactions = new HashSet<ElementReactionType>();

            foreach (var config in ReactionConfigs.Values)
            {
                // 避免重复添加相同的反应类型
                if (!addedReactions.Contains(config.ReactionType))
                {
                    configs.Add(config);
                    addedReactions.Add(config.ReactionType);
                }
            }

            return configs;
        }
    }
}

