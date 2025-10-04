using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{

    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.View)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class ForgeItemViewComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "Forge";
        public const string ResName = "ForgeItemView";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIViewComponent> u_UIView;
        public YIUIViewComponent UIView => u_UIView;
        public UnityEngine.UI.LoopVerticalScrollRect u_ComForgeScrollRect;
        public UnityEngine.UI.LoopHorizontalScrollRect u_ComForgeReadyScrollRect;
        public Spine.Unity.SkeletonGraphic u_ComForgeLv_SkeSkeletonGraphic;
        public ET.Client.UIDragItem u_ComForgeBgUIDragItem;
        public YIUIFramework.UIDataValueInt u_DataForgeTime;
        public YIUIFramework.UIDataValueInt u_DataHuo;
        public YIUIFramework.UIDataValueInt u_DataShui;
        public YIUIFramework.UIDataValueInt u_DataBing;
        public YIUIFramework.UIDataValueInt u_DataLei;
        public YIUIFramework.UIDataValueInt u_DataFeng;
        public YIUIFramework.UIDataValueInt u_DataScoreBase;
        public YIUIFramework.UIDataValueInt u_DataScoreTotal;
        public YIUIFramework.UIDataValueInt u_DataScoreMult;
        public YIUIFramework.UIDataValueString u_DataFanYing;
        public YIUIFramework.UIDataValueInt u_DataFanYinAdd;
        public YIUIFramework.UIDataValueFloat u_DataFanYinMult;
        public YIUIFramework.UIDataValueInt u_DataLingQi;
        public UITaskEventP0 u_EventForgeStart;
        public UITaskEventHandleP0 u_EventForgeStartHandle;
        public const string OnEventForgeStartInvoke = "ForgeItemViewComponent.OnEventForgeStartInvoke";
        public UITaskEventP0 u_EventForgeFinish;
        public UITaskEventHandleP0 u_EventForgeFinishHandle;
        public const string OnEventForgeFinishInvoke = "ForgeItemViewComponent.OnEventForgeFinishInvoke";

    }
}