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
    [EntitySystemOf(typeof(BagPanelComponent))]
    public static partial class BagPanelComponentSystem
    {
        [EntitySystem]
        private static void Awake(this BagPanelComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this BagPanelComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this BagPanelComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIPanel = self.UIBase.GetComponent<YIUIPanelComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIPanel.Layer = EPanelLayer.Popup;
            self.UIPanel.PanelOption = EPanelOption.TimeCache;
            self.UIPanel.StackOption = EPanelStackOption.VisibleTween;
            self.UIPanel.Priority = 0;
            self.UIPanel.CachePanelTime = 10;

            self.u_ComItemDescRectTransform = self.UIBase.ComponentTable.FindComponent<UnityEngine.RectTransform>("u_ComItemDescRectTransform");
            self.u_DataState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataState");
            self.u_DataView = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataView");
            self.u_DataCoin = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCoin");
            self.u_DataInfo = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataInfo");
            self.u_DataScore = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataScore");
            self.u_DataIcon = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataIcon");
            self.u_DataCollectState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCollectState");
            self.u_DataName = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataName");
            self.u_DataMessage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataMessage");
            self.u_DataMu = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataMu");
            self.u_DataLevel = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataLevel");
            self.u_DataJin = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataJin");
            self.u_DataInfoState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueBool>("u_DataInfoState");
            self.u_DataImage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataImage");
            self.u_DataHuo = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataHuo");
            self.u_DataElement = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataElement");
            self.u_DataShui = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataShui");
            self.u_DataTu = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataTu");
            self.u_EventOpenItemAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenItemAction");
            self.u_EventOpenItemActionHandle = self.u_EventOpenItemAction.Add(self,BagPanelComponent.OnEventOpenItemActionInvoke);
            self.u_EventOpenOtherAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenOtherAction");
            self.u_EventOpenOtherActionHandle = self.u_EventOpenOtherAction.Add(self,BagPanelComponent.OnEventOpenOtherActionInvoke);
            self.u_EventOpenToolAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventOpenToolAction");
            self.u_EventOpenToolActionHandle = self.u_EventOpenToolAction.Add(self,BagPanelComponent.OnEventOpenToolActionInvoke);
            self.u_EventBackAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventBackAction");
            self.u_EventBackActionHandle = self.u_EventBackAction.Add(self,BagPanelComponent.OnEventBackActionInvoke);
            self.u_EventHomeAction = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventHomeAction");
            self.u_EventHomeActionHandle = self.u_EventHomeAction.Add(self,BagPanelComponent.OnEventHomeActionInvoke);
            self.u_EventTestGet = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventTestGet");
            self.u_EventTestGetHandle = self.u_EventTestGet.Add(self,BagPanelComponent.OnEventTestGetInvoke);
            self.u_EventTestFresh = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventTestFresh");
            self.u_EventTestFreshHandle = self.u_EventTestFresh.Add(self,BagPanelComponent.OnEventTestFreshInvoke);
            self.u_EventSell = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventSell");
            self.u_EventSellHandle = self.u_EventSell.Add(self,BagPanelComponent.OnEventSellInvoke);
            self.u_EventCollect = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventCollect");
            self.u_EventCollectHandle = self.u_EventCollect.Add(self,BagPanelComponent.OnEventCollectInvoke);
            self.u_EventObtain = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventObtain");
            self.u_EventObtainHandle = self.u_EventObtain.Add(self,BagPanelComponent.OnEventObtainInvoke);

        }
    }
}
