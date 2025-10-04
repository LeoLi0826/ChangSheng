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
    public partial class ItemPrefabComponent: Entity 
    {
        //物品的数据
        public EntityRef<Item> ItemDataInfo;

        public int currentIndex;
    }
    
    public struct EventSelectItemFresh
    {
        public EntityRef<Item> bagItemData;
    }
}