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
    public partial class ItemTipsViewComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize, IYIUIOpen
    {
        public const string PkgName = "Tips";
        public const string ResName = "ItemTipsView";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public EntityRef<YIUIWindowComponent> u_UIWindow;
        public YIUIWindowComponent UIWindow => u_UIWindow;
        public EntityRef<YIUIViewComponent> u_UIView;
        public YIUIViewComponent UIView => u_UIView;
        public YIUIFramework.UIDataValueInt u_DataHuo;
        public YIUIFramework.UIDataValueInt u_DataShui;
        public YIUIFramework.UIDataValueInt u_DataBing;
        public YIUIFramework.UIDataValueInt u_DataLei;
        public YIUIFramework.UIDataValueInt u_DataFeng;
        public YIUIFramework.UIDataValueString u_DataIcon;
        public YIUIFramework.UIDataValueString u_DataName;
        public YIUIFramework.UIDataValueInt u_DataRarity;
        public YIUIFramework.UIDataValueInt u_DataCollection;
        public YIUIFramework.UIDataValueInt u_DataElement;
        public YIUIFramework.UIDataValueString u_DataMessage;
        public UITaskEventP0 u_EventClose;
        public UITaskEventHandleP0 u_EventCloseHandle;
        public const string OnEventCloseInvoke = "ItemTipsViewComponent.OnEventCloseInvoke";

    }
}