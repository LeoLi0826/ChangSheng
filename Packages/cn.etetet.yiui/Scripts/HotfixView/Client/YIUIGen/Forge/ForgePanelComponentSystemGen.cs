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
    [FriendOf(typeof(YIUIPanelComponent))]
    [EntitySystemOf(typeof(ForgePanelComponent))]
    public static partial class ForgePanelComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ForgePanelComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this ForgePanelComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this ForgePanelComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIPanel = self.UIBase.GetComponent<YIUIPanelComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIPanel.Layer = EPanelLayer.Popup;
            self.UIPanel.PanelOption = EPanelOption.TimeCache;
            self.UIPanel.StackOption = EPanelStackOption.Visible;
            self.UIPanel.Priority = 0;
            self.UIPanel.CachePanelTime = 10;

            self.u_ComForgeInputLoop = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopVerticalScrollRect>("u_ComForgeInputLoop");
            self.u_ComForgeOutLoop = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopVerticalScrollRect>("u_ComForgeOutLoop");
            self.u_DataView = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataView");
            self.u_DataEnergy = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataEnergy");
            self.u_DataGold = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataGold");
            self.u_DataDiamond = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataDiamond");
            self.u_EventBackAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventBackAction");
            self.u_EventBackActionHandle = self.u_EventBackAction.Add(self,ForgePanelComponent.OnEventBackActionInvoke);
            self.u_EventHomeAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventHomeAction");
            self.u_EventHomeActionHandle = self.u_EventHomeAction.Add(self,ForgePanelComponent.OnEventHomeActionInvoke);
            self.u_EventOpenToolAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenToolAction");
            self.u_EventOpenToolActionHandle = self.u_EventOpenToolAction.Add(self,ForgePanelComponent.OnEventOpenToolActionInvoke);
            self.u_EventOpenItemAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenItemAction");
            self.u_EventOpenItemActionHandle = self.u_EventOpenItemAction.Add(self,ForgePanelComponent.OnEventOpenItemActionInvoke);
            self.u_EventOpenOtherAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenOtherAction");
            self.u_EventOpenOtherActionHandle = self.u_EventOpenOtherAction.Add(self,ForgePanelComponent.OnEventOpenOtherActionInvoke);
            self.u_EventTestFresh = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventTestFresh");
            self.u_EventTestFreshHandle = self.u_EventTestFresh.Add(self,ForgePanelComponent.OnEventTestFreshInvoke);
            self.u_EventTestGet = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventTestGet");
            self.u_EventTestGetHandle = self.u_EventTestGet.Add(self,ForgePanelComponent.OnEventTestGetInvoke);
            self.u_EventForge = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventForge");
            self.u_EventForgeHandle = self.u_EventForge.Add(self,ForgePanelComponent.OnEventForgeInvoke);

        }
    }
}
