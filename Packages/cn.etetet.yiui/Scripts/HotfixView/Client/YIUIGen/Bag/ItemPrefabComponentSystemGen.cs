using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [FriendOf(typeof(YIUIChild))]
    [EntitySystemOf(typeof(ItemPrefabComponent))]
    public static partial class ItemPrefabComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ItemPrefabComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this ItemPrefabComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this ItemPrefabComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();

            self.u_ComItemImg = self.UIBase.ComponentTable.FindComponent<UnityEngine.RectTransform>("u_ComItemImg");
            self.u_DataItemState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataItemState");
            self.u_DataSelect = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueBool>("u_DataSelect");
            self.u_DataIcon = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataIcon");
            self.u_DataCount = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCount");
            self.u_DataItemType = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataItemType");
            self.u_EventSelect = self.UIBase.EventTable.FindEvent<UIEventP0>("u_EventSelect");
            self.u_EventSelectHandle = self.u_EventSelect.Add(self,ItemPrefabComponent.OnEventSelectInvoke);

        }
    }
}
