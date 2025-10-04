using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{

    /// <summary>
    /// 由YIUI工具自动创建 请勿修改
    /// </summary>
    [YIUI(EUICodeType.Common)]
    [ComponentOf(typeof(YIUIChild))]
    public partial class ItemPrefabComponent : Entity, IDestroy, IAwake, IYIUIBind, IYIUIInitialize
    {
        public const string PkgName = "Bag";
        public const string ResName = "ItemPrefab";

        public EntityRef<YIUIChild> u_UIBase;
        public YIUIChild UIBase => u_UIBase;
        public UnityEngine.RectTransform u_ComItemImg;
        public YIUIFramework.UIDataValueInt u_DataItemState;
        public YIUIFramework.UIDataValueBool u_DataSelect;
        public YIUIFramework.UIDataValueString u_DataIcon;
        public YIUIFramework.UIDataValueInt u_DataCount;
        public YIUIFramework.UIDataValueInt u_DataItemType;
        public UIEventP0 u_EventSelect;
        public UIEventHandleP0 u_EventSelectHandle;
        public const string OnEventSelectInvoke = "ItemPrefabComponent.OnEventSelectInvoke";

    }
}