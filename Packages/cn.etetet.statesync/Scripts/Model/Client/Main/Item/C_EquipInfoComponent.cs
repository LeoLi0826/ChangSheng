using System;
using System.Collections.Generic;


namespace ET
{
    [ComponentOf(typeof(Item))]
    public class C_EquipInfoComponent : Entity,IAwake,IDestroy
    {
        public bool IsInited = false;
        
        /// <summary>
        /// 装备评分
        /// </summary>
        public int Score = 0;
        
        /// <summary>
        /// 装备词条列表
        /// </summary>

        public List<EntityRef<C_AttributeEntry>> EntryList = new List<EntityRef<C_AttributeEntry>>();
    }
}