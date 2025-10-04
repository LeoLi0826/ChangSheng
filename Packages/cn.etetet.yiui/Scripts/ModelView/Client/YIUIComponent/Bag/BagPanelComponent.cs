using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.7
    /// Desc
    /// </summary>
    public partial class BagPanelComponent: Entity, IAwake, IDestroy,IYIUIOpen<EBagPanelViewEnum>
    {
        public EntityRef<Item> ItemInfo;
        public RectTransform Target;
    }
}