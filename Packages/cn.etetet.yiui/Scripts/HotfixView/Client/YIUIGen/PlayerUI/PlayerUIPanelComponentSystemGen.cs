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
    [EntitySystemOf(typeof(PlayerUIPanelComponent))]
    public static partial class PlayerUIPanelComponentSystem
    {
        [EntitySystem]
        private static void Awake(this PlayerUIPanelComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this PlayerUIPanelComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this PlayerUIPanelComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIPanel = self.UIBase.GetComponent<YIUIPanelComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIPanel.Layer = EPanelLayer.Panel;
            self.UIPanel.PanelOption = EPanelOption.TimeCache;
            self.UIPanel.StackOption = EPanelStackOption.VisibleTween;
            self.UIPanel.Priority = 0;
            self.UIPanel.CachePanelTime = 10;

            self.u_ComJoystickEventTrigger = self.UIBase.ComponentTable.FindComponent<UnityEngine.EventSystems.EventTrigger>("u_ComJoystickEventTrigger");
            self.u_ComHandleImgTransform = self.UIBase.ComponentTable.FindComponent<UnityEngine.RectTransform>("u_ComHandleImgTransform");
            self.u_DataLevel = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLevel");
            self.u_DataGold = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataGold");
            self.u_DataExp = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataExp");
            self.u_DataDiamond = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataDiamond");
            self.u_DataButtonState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataButtonState");
            self.u_EventDrag = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventDrag");
            self.u_EventDragHandle = self.u_EventDrag.Add(self,PlayerUIPanelComponent.OnEventDragInvoke);
            self.u_EventBag = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventBag");
            self.u_EventBagHandle = self.u_EventBag.Add(self,PlayerUIPanelComponent.OnEventBagInvoke);
            self.u_EventShop = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventShop");
            self.u_EventShopHandle = self.u_EventShop.Add(self,PlayerUIPanelComponent.OnEventShopInvoke);
            self.u_EventButtonInteraction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventButtonInteraction");
            self.u_EventButtonInteractionHandle = self.u_EventButtonInteraction.Add(self,PlayerUIPanelComponent.OnEventButtonInteractionInvoke);
            self.u_EventHome = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventHome");
            self.u_EventHomeHandle = self.u_EventHome.Add(self,PlayerUIPanelComponent.OnEventHomeInvoke);
            self.u_EventDataTest = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventDataTest");
            self.u_EventDataTestHandle = self.u_EventDataTest.Add(self,PlayerUIPanelComponent.OnEventDataTestInvoke);
            self.u_EventHandBook = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventHandBook");
            self.u_EventHandBookHandle = self.u_EventHandBook.Add(self,PlayerUIPanelComponent.OnEventHandBookInvoke);
            self.u_EventTestOpenModalTipPanel = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventTestOpenModalTipPanel");
            self.u_EventTestOpenModalTipPanelHandle = self.u_EventTestOpenModalTipPanel.Add(self,PlayerUIPanelComponent.OnEventTestOpenModalTipPanelInvoke);
            self.u_EventForgeAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventForgeAction");
            self.u_EventForgeActionHandle = self.u_EventForgeAction.Add(self,PlayerUIPanelComponent.OnEventForgeActionInvoke);

        }
    }
}
