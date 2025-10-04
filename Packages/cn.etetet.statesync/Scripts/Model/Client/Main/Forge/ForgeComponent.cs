using System.Collections.Generic;

namespace ET.Client
{
    public struct ElementReaction
    {
        public string Name;
        public int Value;
    }
   
    [ComponentOf(typeof(Scene))]
    public class ForgeComponent : Entity,IAwake,IDestroy
    {
        /// <summary>
        /// 可以合成次数
        /// </summary>
        public int ForgeNum = 10;

        /// <summary>
        /// 灵气值
        /// </summary>
        public int Score = 0;
        
        /// <summary>
        /// 最终灵气值
        /// </summary>
        public int FinalScore = 0;
        
        /// <summary>
        /// 现在有的属性
        /// </summary>
        public Dictionary<ItemElementAttr, int> Element = new();

        /// <summary>
        /// 当前反应类型
        /// </summary>
        public ElementReactionConfig ElementReactionConfig;

        /// <summary>
        /// 当前反应名称
        /// </summary>
        public string CurrentReactionName = "";

        /// <summary>
        /// 当前反应倍乘加成
        /// </summary>
        public float CurrentReactionMultiplier = 0f;

        /// <summary>
        /// 当前反应灵气加成
        /// </summary>
        public int CurrentReactionScore = 0;

        /// <summary>
        /// 最终灵气值（用于UI显示）
        /// </summary>
        //public int FinalSpiritValue = 0;
    }
}
