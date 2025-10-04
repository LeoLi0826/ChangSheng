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
    public enum EForgePanelViewEnum
    {
        ForgeItemView = 1,
        ForgeToolView = 2,
        ForgeOtherView = 3,
    }
    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.Panel, EPanelLayer.Popup)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class ForgePanelComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "Forge";
        public const string ResName = "ForgePanel";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIPanelComponent> u_UIPanel;
        public YIUIPanelComponent UIPanel => u_UIPanel;
        public UnityEngine.UI.LoopVerticalScrollRect u_ComForgeInputLoop;
        public UnityEngine.UI.LoopVerticalScrollRect u_ComForgeOutLoop;
        public YIUIFramework.UIDataValueInt u_DataView;
        public YIUIFramework.UIDataValueInt u_DataEnergy;
        public YIUIFramework.UIDataValueInt u_DataGold;
        public YIUIFramework.UIDataValueInt u_DataDiamond;
        public UITaskEventP0 u_EventBackAction;
        public UITaskEventHandleP0 u_EventBackActionHandle;
        public const string OnEventBackActionInvoke = "ForgePanelComponent.OnEventBackActionInvoke";
        public UITaskEventP0 u_EventHomeAction;
        public UITaskEventHandleP0 u_EventHomeActionHandle;
        public const string OnEventHomeActionInvoke = "ForgePanelComponent.OnEventHomeActionInvoke";
        public UITaskEventP0 u_EventOpenToolAction;
        public UITaskEventHandleP0 u_EventOpenToolActionHandle;
        public const string OnEventOpenToolActionInvoke = "ForgePanelComponent.OnEventOpenToolActionInvoke";
        public UITaskEventP0 u_EventOpenItemAction;
        public UITaskEventHandleP0 u_EventOpenItemActionHandle;
        public const string OnEventOpenItemActionInvoke = "ForgePanelComponent.OnEventOpenItemActionInvoke";
        public UITaskEventP0 u_EventOpenOtherAction;
        public UITaskEventHandleP0 u_EventOpenOtherActionHandle;
        public const string OnEventOpenOtherActionInvoke = "ForgePanelComponent.OnEventOpenOtherActionInvoke";
        public UITaskEventP0 u_EventTestFresh;
        public UITaskEventHandleP0 u_EventTestFreshHandle;
        public const string OnEventTestFreshInvoke = "ForgePanelComponent.OnEventTestFreshInvoke";
        public UITaskEventP0 u_EventTestGet;
        public UITaskEventHandleP0 u_EventTestGetHandle;
        public const string OnEventTestGetInvoke = "ForgePanelComponent.OnEventTestGetInvoke";
        public UITaskEventP0 u_EventForge;
        public UITaskEventHandleP0 u_EventForgeHandle;
        public const string OnEventForgeInvoke = "ForgePanelComponent.OnEventForgeInvoke";

    }
}