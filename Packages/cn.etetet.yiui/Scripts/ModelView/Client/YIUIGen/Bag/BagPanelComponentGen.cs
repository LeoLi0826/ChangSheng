using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// 当前Panel所有可用view枚举
    /// </summary>
    public enum EBagPanelViewEnum
    {
        BagItemView = 1,
        BagToolView = 2,
        BagOtherView = 3,
    }
    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.Panel, EPanelLayer.Popup)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class BagPanelComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "Bag";
        public const string ResName = "BagPanel";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIPanelComponent> u_UIPanel;
        public YIUIPanelComponent UIPanel => u_UIPanel;
        public UnityEngine.RectTransform u_ComItemDescRectTransform;
        public YIUIFramework.UIDataValueInt u_DataState;
        public YIUIFramework.UIDataValueInt u_DataView;
        public YIUIFramework.UIDataValueInt u_DataCoin;
        public YIUIFramework.UIDataValueString u_DataInfo;
        public YIUIFramework.UIDataValueInt u_DataScore;
        public YIUIFramework.UIDataValueString u_DataIcon;
        public YIUIFramework.UIDataValueInt u_DataCollectState;
        public YIUIFramework.UIDataValueString u_DataName;
        public YIUIFramework.UIDataValueString u_DataMessage;
        public YIUIFramework.UIDataValueInt u_DataMu;
        public YIUIFramework.UIDataValueString u_DataLevel;
        public YIUIFramework.UIDataValueInt u_DataJin;
        public YIUIFramework.UIDataValueBool u_DataInfoState;
        public YIUIFramework.UIDataValueString u_DataImage;
        public YIUIFramework.UIDataValueInt u_DataHuo;
        public YIUIFramework.UIDataValueString u_DataElement;
        public YIUIFramework.UIDataValueInt u_DataShui;
        public YIUIFramework.UIDataValueInt u_DataTu;
        public UITaskEventP0 u_EventOpenItemAction;
        public UITaskEventHandleP0 u_EventOpenItemActionHandle;
        public const string OnEventOpenItemActionInvoke = "BagPanelComponent.OnEventOpenItemActionInvoke";
        public UITaskEventP0 u_EventOpenOtherAction;
        public UITaskEventHandleP0 u_EventOpenOtherActionHandle;
        public const string OnEventOpenOtherActionInvoke = "BagPanelComponent.OnEventOpenOtherActionInvoke";
        public UITaskEventP0 u_EventOpenToolAction;
        public UITaskEventHandleP0 u_EventOpenToolActionHandle;
        public const string OnEventOpenToolActionInvoke = "BagPanelComponent.OnEventOpenToolActionInvoke";
        public UITaskEventP0 u_EventBackAction;
        public UITaskEventHandleP0 u_EventBackActionHandle;
        public const string OnEventBackActionInvoke = "BagPanelComponent.OnEventBackActionInvoke";
        public UITaskEventP0 u_EventHomeAction;
        public UITaskEventHandleP0 u_EventHomeActionHandle;
        public const string OnEventHomeActionInvoke = "BagPanelComponent.OnEventHomeActionInvoke";
        public UITaskEventP0 u_EventTestGet;
        public UITaskEventHandleP0 u_EventTestGetHandle;
        public const string OnEventTestGetInvoke = "BagPanelComponent.OnEventTestGetInvoke";
        public UITaskEventP0 u_EventTestFresh;
        public UITaskEventHandleP0 u_EventTestFreshHandle;
        public const string OnEventTestFreshInvoke = "BagPanelComponent.OnEventTestFreshInvoke";
        public UITaskEventP0 u_EventSell;
        public UITaskEventHandleP0 u_EventSellHandle;
        public const string OnEventSellInvoke = "BagPanelComponent.OnEventSellInvoke";
        public UITaskEventP0 u_EventCollect;
        public UITaskEventHandleP0 u_EventCollectHandle;
        public const string OnEventCollectInvoke = "BagPanelComponent.OnEventCollectInvoke";
        public UITaskEventP0 u_EventObtain;
        public UITaskEventHandleP0 u_EventObtainHandle;
        public const string OnEventObtainInvoke = "BagPanelComponent.OnEventObtainInvoke";

    }
}