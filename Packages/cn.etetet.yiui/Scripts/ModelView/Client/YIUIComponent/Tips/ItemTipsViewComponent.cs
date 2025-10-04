using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2025.9.8
    /// Desc
    /// </summary>
    public partial class ItemTipsViewComponent: Entity, IYIUIOpen<ParamVo>, IYIUIOpenTween, IYIUICloseTween,IDynamicEvent<ItemTipsClose>
    {
        public ItemTipsExtraData ExtraData;

    }
    
    public struct ItemTipsClose
    {
    
    }
    
    //额外参数
    [EnableClass]
    public class ItemTipsExtraData
    {
        /// <summary>
        /// 道具
        /// </summary>
        public EntityRef<Item> Item;
        
        /// <summary>
        /// 道具所在位子
        /// </summary>
        public Vector2 Position;
    }
}