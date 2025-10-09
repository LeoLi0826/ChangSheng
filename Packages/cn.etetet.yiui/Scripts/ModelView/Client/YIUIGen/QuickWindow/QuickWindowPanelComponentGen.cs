using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{

    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.Panel, EPanelLayer.Popup)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class QuickWindowPanelComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "QuickWindow";
        public const string ResName = "QuickWindowPanel";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIPanelComponent> u_UIPanel;
        public YIUIPanelComponent UIPanel => u_UIPanel;
        public UnityEngine.UI.LoopHorizontalScrollRect u_ComLoopScrollQuick;
        public UnityEngine.UI.LoopVerticalScrollRect u_ComLoopScrollFunction;
        public YIUIFramework.UIDataValueString u_DataName;
        public YIUIFramework.UIDataValueString u_DataMessage;
        public YIUIFramework.UIDataValueString u_DataImage;
        public YIUIFramework.UIDataValueString u_DataLevel;
        public YIUIFramework.UIDataValueInt u_DataScore;
        public YIUIFramework.UIDataValueFloat u_DataLife;
        public YIUIFramework.UIDataValueFloat u_DataManaPrecent;
        public YIUIFramework.UIDataValueInt u_DataManaMax;
        public YIUIFramework.UIDataValueInt u_DataHuo;
        public YIUIFramework.UIDataValueInt u_DataShui;
        public YIUIFramework.UIDataValueInt u_DataBing;
        public YIUIFramework.UIDataValueInt u_DataLei;
        public YIUIFramework.UIDataValueInt u_DataFeng;
        public YIUIFramework.UIDataValueString u_DataElement;
        public YIUIFramework.UIDataValueInt u_DataCalamityDate;
        public YIUIFramework.UIDataValueInt u_DataCalamityDamage;
        public YIUIFramework.UIDataValueInt u_DataManaValue;
        public YIUIFramework.UIDataValueBool u_DataInfoTipsState;
        public YIUIFramework.UIDataValueInt u_DataLifeValue;
        public YIUIFramework.UIDataValueInt u_DataLifeMax;
        public YIUIFramework.UIDataValueInt u_DataGoldCoin;

    }
}