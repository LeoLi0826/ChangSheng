using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2025.3.21
    /// Desc
    /// </summary>
    public partial class QuickItemPrefabComponent: Entity, IDynamicEvent<EventQuickItemForgeState>
    {
        //物品的数据
        public EntityRef<Item> ItemDataInfo;


        public bool IsEquipBox = false;
    }
    
    public struct EventQuickItemForgeState
    {
        public int State;
    }
   
}