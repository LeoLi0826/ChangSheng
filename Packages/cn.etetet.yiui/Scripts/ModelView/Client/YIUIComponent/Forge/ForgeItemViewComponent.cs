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
    public partial class ForgeItemViewComponent: Entity, IDynamicEvent<EventForgeItemReFresh>
            ,IDynamicEvent<EventForgeReadyReFresh>
            , IDynamicEvent<ForgeItemWindowFresh>, IDynamicEvent<EventElementReFresh>
            , IDynamicEvent<EventScoreReFresh> , IDynamicEvent<EventScoreTotalReFresh> 
            , IDynamicEvent<ForgeWindowAllFresh>  
    {
        public int ScoreTotal;
        public int ScoreMult;

        public int ReadyScoreTotal;
        
        public EntityRef<YIUILoopScrollChild> m_ComForgeScrollRect;
        public YIUILoopScrollChild ComForgeScrollRect => m_ComForgeScrollRect;
        
        public EntityRef<YIUILoopScrollChild> m_ComForgeReadyScrollRect;
        public YIUILoopScrollChild ComForgeReadyScrollRect => m_ComForgeReadyScrollRect;

        
        public List<EntityRef<Item>> m_ForgeDataList = new List<EntityRef<Item>>();
        public List<EntityRef<Item>> m_ForgeReadyDataList = new List<EntityRef<Item>>();
    }
    //刷新所有
    public struct ForgeWindowAllFresh
    {
        //public Item itemTemp;
    }
    
    
 
    
    //刷新输入格子
    public struct EventForgeInputItemReFresh
    {
        public int A;
    }
    
    //刷新合成栏格子
    public struct EventForgeReFresh
    {
        public int A;
    }
    

    public struct EventScoreReFresh
    {
        public int ScoreMult;
        public int ScoreAdd;
    }
    
    public struct EventScoreTotalReFresh
    {
        public int ScoreMult;
        public int ScoreAdd;
    }
    
 
    
    //检查是否可以放置
    public struct EventForgeInputCheck
    {
        public long id;
    }
    
}