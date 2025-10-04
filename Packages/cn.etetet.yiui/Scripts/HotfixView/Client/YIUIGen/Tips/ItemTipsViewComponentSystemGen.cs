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
    [FriendOf(typeof(YIUIWindowComponent))]
    [FriendOf(typeof(YIUIViewComponent))]
    [EntitySystemOf(typeof(ItemTipsViewComponent))]
    public static partial class ItemTipsViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ItemTipsViewComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this ItemTipsViewComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this ItemTipsViewComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIView = self.UIBase.GetComponent<YIUIViewComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIView.ViewWindowType = EViewWindowType.View;
            self.UIView.StackOption = EViewStackOption.None;

            self.u_DataHuo = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataHuo");
            self.u_DataShui = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataShui");
            self.u_DataBing = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataBing");
            self.u_DataLei = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLei");
            self.u_DataFeng = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataFeng");
            self.u_DataIcon = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataIcon");
            self.u_DataName = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataName");
            self.u_DataRarity = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataRarity");
            self.u_DataCollection = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCollection");
            self.u_DataElement = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataElement");
            self.u_DataMessage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataMessage");
            self.u_EventClose = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventClose");
            self.u_EventCloseHandle = self.u_EventClose.Add(self,ItemTipsViewComponent.OnEventCloseInvoke);

        }
    }
}
