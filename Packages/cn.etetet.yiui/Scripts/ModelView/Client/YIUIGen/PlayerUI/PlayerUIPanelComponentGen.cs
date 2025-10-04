using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{

    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.Panel, EPanelLayer.Panel)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class PlayerUIPanelComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "PlayerUI";
        public const string ResName = "PlayerUIPanel";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIPanelComponent> u_UIPanel;
        public YIUIPanelComponent UIPanel => u_UIPanel;
        public UnityEngine.EventSystems.EventTrigger u_ComJoystickEventTrigger;
        public UnityEngine.RectTransform u_ComHandleImgTransform;
        public YIUIFramework.UIDataValueInt u_DataLevel;
        public YIUIFramework.UIDataValueInt u_DataGold;
        public YIUIFramework.UIDataValueInt u_DataExp;
        public YIUIFramework.UIDataValueInt u_DataDiamond;
        public YIUIFramework.UIDataValueInt u_DataButtonState;
        public UITaskEventP0 u_EventDrag;
        public UITaskEventHandleP0 u_EventDragHandle;
        public const string OnEventDragInvoke = "PlayerUIPanelComponent.OnEventDragInvoke";
        public UITaskEventP0 u_EventBag;
        public UITaskEventHandleP0 u_EventBagHandle;
        public const string OnEventBagInvoke = "PlayerUIPanelComponent.OnEventBagInvoke";
        public UITaskEventP0 u_EventShop;
        public UITaskEventHandleP0 u_EventShopHandle;
        public const string OnEventShopInvoke = "PlayerUIPanelComponent.OnEventShopInvoke";
        public UITaskEventP0 u_EventButtonInteraction;
        public UITaskEventHandleP0 u_EventButtonInteractionHandle;
        public const string OnEventButtonInteractionInvoke = "PlayerUIPanelComponent.OnEventButtonInteractionInvoke";
        public UITaskEventP0 u_EventHome;
        public UITaskEventHandleP0 u_EventHomeHandle;
        public const string OnEventHomeInvoke = "PlayerUIPanelComponent.OnEventHomeInvoke";
        public UITaskEventP0 u_EventDataTest;
        public UITaskEventHandleP0 u_EventDataTestHandle;
        public const string OnEventDataTestInvoke = "PlayerUIPanelComponent.OnEventDataTestInvoke";
        public UITaskEventP0 u_EventHandBook;
        public UITaskEventHandleP0 u_EventHandBookHandle;
        public const string OnEventHandBookInvoke = "PlayerUIPanelComponent.OnEventHandBookInvoke";
        public UITaskEventP0 u_EventTestOpenModalTipPanel;
        public UITaskEventHandleP0 u_EventTestOpenModalTipPanelHandle;
        public const string OnEventTestOpenModalTipPanelInvoke = "PlayerUIPanelComponent.OnEventTestOpenModalTipPanelInvoke";
        public UITaskEventP0 u_EventForgeAction;
        public UITaskEventHandleP0 u_EventForgeActionHandle;
        public const string OnEventForgeActionInvoke = "PlayerUIPanelComponent.OnEventForgeActionInvoke";

    }
}