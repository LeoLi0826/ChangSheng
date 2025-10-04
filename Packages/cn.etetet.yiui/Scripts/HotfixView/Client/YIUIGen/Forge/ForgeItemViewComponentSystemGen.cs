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
    [FriendOf(typeof(YIUIViewComponent))]
    [EntitySystemOf(typeof(ForgeItemViewComponent))]
    public static partial class ForgeItemViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ForgeItemViewComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this ForgeItemViewComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this ForgeItemViewComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIView = self.UIBase.GetComponent<YIUIViewComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIView.ViewWindowType = EViewWindowType.View;
            self.UIView.StackOption = EViewStackOption.VisibleTween;

            self.u_ComForgeScrollRect = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopVerticalScrollRect>("u_ComForgeScrollRect");
            self.u_ComForgeReadyScrollRect = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopHorizontalScrollRect>("u_ComForgeReadyScrollRect");
            self.u_ComForgeLv_SkeSkeletonGraphic = self.UIBase.ComponentTable.FindComponent<Spine.Unity.SkeletonGraphic>("u_ComForgeLv_SkeSkeletonGraphic");
            self.u_ComForgeBgUIDragItem = self.UIBase.ComponentTable.FindComponent<ET.Client.UIDragItem>("u_ComForgeBgUIDragItem");
            self.u_DataForgeTime = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataForgeTime");
            self.u_DataHuo = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataHuo");
            self.u_DataShui = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataShui");
            self.u_DataBing = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataBing");
            self.u_DataLei = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLei");
            self.u_DataFeng = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataFeng");
            self.u_DataScoreBase = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataScoreBase");
            self.u_DataScoreTotal = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataScoreTotal");
            self.u_DataScoreMult = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataScoreMult");
            self.u_DataFanYing = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataFanYing");
            self.u_DataFanYinAdd = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataFanYinAdd");
            self.u_DataFanYinMult = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueFloat>("u_DataFanYinMult");
            self.u_DataLingQi = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLingQi");
            self.u_EventForgeStart = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventForgeStart");
            self.u_EventForgeStartHandle = self.u_EventForgeStart.Add(self,ForgeItemViewComponent.OnEventForgeStartInvoke);
            self.u_EventForgeFinish = self.UIBase.EventTable.FindEvent<UITaskEventP0>("u_EventForgeFinish");
            self.u_EventForgeFinishHandle = self.u_EventForgeFinish.Add(self,ForgeItemViewComponent.OnEventForgeFinishInvoke);

        }
    }
}
