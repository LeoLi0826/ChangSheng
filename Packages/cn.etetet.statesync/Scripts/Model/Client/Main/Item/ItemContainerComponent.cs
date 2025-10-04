using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class ItemContainerComponent : Entity,IAwake,IDestroy
    {
        
        /// <summary>
        /// 道具使用CD,int为道具配置ID
        /// </summary>
        public Dictionary<ItemContainerType, long> ContainerList = new Dictionary<ItemContainerType, long>();
        
    }
}