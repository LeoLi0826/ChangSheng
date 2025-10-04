using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2025.1.9
    /// Desc
    /// </summary>
    public partial class ForgePanelComponent: Entity, IAwake, IDestroy,IYIUIOpen<EForgePanelViewEnum> ,
            IDynamicEvent<EventForgeInputReFresh>,IDynamicEvent<EventForgeOutReFresh>
    {
        
        public EntityRef<YIUILoopScrollChild> m_LoopInputScroll;
        public YIUILoopScrollChild LoopInputScroll => m_LoopInputScroll;
        
        public EntityRef<YIUILoopScrollChild> m_LoopOutScroll;
        public YIUILoopScrollChild LoopOutScroll => m_LoopOutScroll;

        public List<EntityRef<Item>> m_ItemDataInputList  =  new List<EntityRef<Item>>();
        public List<EntityRef<Item>> m_ItemDataOutList  =  new List<EntityRef<Item>>();
    }
    public struct EventForgeInputReFresh
    {
        public int A;
    }
    
    public struct EventForgeOutReFresh
    {
        public int A;
    }
   
    
}