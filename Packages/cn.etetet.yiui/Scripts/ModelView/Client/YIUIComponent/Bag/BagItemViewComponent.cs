using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.8
    /// Desc
    /// </summary>
    public partial class BagItemViewComponent: Entity, IDynamicEvent<EventBagItemReFresh>
            ,IDynamicEvent<BagItemViewFresh>
    {
        public EntityRef<YIUILoopScrollChild> m_ItemListScroll;
        
        public YIUILoopScrollChild ItemListScroll => m_ItemListScroll;
        

        public List<EntityRef<Item>> m_ItemDataList=new List<EntityRef<Item>>();
    }
    
    public struct EventBagItemReFresh
    {
        public int A;
    }

    
}