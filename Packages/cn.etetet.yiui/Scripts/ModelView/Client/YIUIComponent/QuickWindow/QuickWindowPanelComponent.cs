using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2025.3.19
    /// Desc
    /// </summary>
    public partial class QuickWindowPanelComponent: Entity,IUpdate , IDynamicEvent<EventQuickItemReFresh>
            , IDynamicEvent<EventQuickItemFunctionReFresh>, IDynamicEvent<EventFunctionCalReFresh>
            ,  IDynamicEvent<QuickTipsClose>, IDynamicEvent<ModalTipsClose>  
            , IDynamicEvent<ManaMaxAddRefresh> , IDynamicEvent<GameSettlementJudge> 
            , IDynamicEvent<CalamitySettlement> 
    
            , IDynamicEvent<QuickWindowFresh> //,IDynamicEvent<QuickWindowFresh>
            , IDynamicEvent<LifeRefresh>
            , IDynamicEvent<ManaAddRefresh> , IDynamicEvent<ManaReduceRefresh>
            , IDynamicEvent<CalamityAdd> , IDynamicEvent<CalamityReduce>

    {
        
        public EntityRef<YIUILoopScrollChild> m_LoopScrollQuick;
        
        public YIUILoopScrollChild LoopScrollQuick => m_LoopScrollQuick;

        public List<EntityRef<Item>> quickList = new List<EntityRef<Item>>();
        
        
        
        public EntityRef<QuickWindowPanelComponent>                         m_CommandQuickComponent;
        
        //这种写法代表只读
        public QuickWindowPanelComponent                                    CommandQuickComponent => m_CommandQuickComponent;
        
        public EntityRef<YIUILoopScrollChild> m_LoopScrollFunction;
        
        public YIUILoopScrollChild LoopScrollFunction => m_LoopScrollFunction;
        
        
        public List<EntityRef<Item>> functionList = new List<EntityRef<Item>>();

        
        public EntityRef<QuickWindowPanelComponent>                         m_CommandFunctionComponent;
        
        //这种写法代表只读
        public QuickWindowPanelComponent                                    CommandFunctionComponent => m_CommandFunctionComponent;
        
        
        //加成
        public int ScoreMult;
        public int ScoreAdd;
        
        //天劫
        public int CalamityDate;
        public int CalamityDamage;
        public bool CalamityFlag;
        
        
        public RectTransform Target;
    }
    
  
    public struct EventQuickItemFunctionReFresh
    {
        public int A;
    }
    
    public struct EventFunctionCalReFresh
    {
        //属性
        public int Element;
        public int Huo;
        public int Shui;
        public int Bing;
        public int Lei;
        public int Feng;
    }
    
    public struct QuickTipsClose
    {
    }
    public struct ModalTipsClose
    {
    }
    
    public struct GameSettlementJudge
    {
        public int ManaAdd;
    }
    
    //天劫结算
    public struct CalamitySettlement
    {
        //public Item itemTemp;
    }
    
    //快捷栏的刷新
    public struct QuickWindowFresh
    {
        //public Item itemTemp;
    }

}