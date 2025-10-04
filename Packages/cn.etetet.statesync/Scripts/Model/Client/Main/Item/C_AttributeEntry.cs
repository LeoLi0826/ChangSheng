namespace ET
{
    //1
    public enum EntryType
    {
        Common  = 1, //普通词条
        Special = 2, //特殊词条
    }

    [ChildOf()]

    public class C_AttributeEntry : Entity,IAwake, IDestroy
    {
        /// <summary>
        /// 词条数值属性类型
        /// </summary>
        public int Key;
        
        /// <summary>
        /// 词条数值属性值
        /// </summary>
        public long Value;
        
        /// <summary>
        /// 词条类型
        /// </summary>
        public EntryType Type;       
    }
}