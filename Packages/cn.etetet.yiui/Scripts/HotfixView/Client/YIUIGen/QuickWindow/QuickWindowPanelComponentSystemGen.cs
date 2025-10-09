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
    [EntitySystemOf(typeof(QuickWindowPanelComponent))]
    public static partial class QuickWindowPanelComponentSystem
    {
        [EntitySystem]
        private static void Awake(this QuickWindowPanelComponent self)
        {
        }

        [EntitySystem]
        private static void YIUIBind(this QuickWindowPanelComponent self)
        {
            self.UIBind();
        }

        private static void UIBind(this QuickWindowPanelComponent self)
        {
            self.u_UIBase = self.GetParent<YIUIChild>();
            self.u_UIWindow = self.UIBase.GetComponent<YIUIWindowComponent>();
            self.u_UIPanel = self.UIBase.GetComponent<YIUIPanelComponent>();
            self.UIWindow.WindowOption = EWindowOption.None;
            self.UIPanel.Layer = EPanelLayer.Popup;
            self.UIPanel.PanelOption = EPanelOption.TimeCache;
            self.UIPanel.StackOption = EPanelStackOption.Visible;
            self.UIPanel.Priority = 0;
            self.UIPanel.CachePanelTime = 10;

            self.u_ComLoopScrollQuick = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopHorizontalScrollRect>("u_ComLoopScrollQuick");
            self.u_ComLoopScrollFunction = self.UIBase.ComponentTable.FindComponent<UnityEngine.UI.LoopVerticalScrollRect>("u_ComLoopScrollFunction");
            self.u_DataName = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataName");
            self.u_DataMessage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataMessage");
            self.u_DataImage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataImage");
            self.u_DataLevel = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataLevel");
            self.u_DataScore = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataScore");
            self.u_DataLife = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueFloat>("u_DataLife");
            self.u_DataManaPrecent = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueFloat>("u_DataManaPrecent");
            self.u_DataManaMax = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataManaMax");
            self.u_DataHuo = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataHuo");
            self.u_DataShui = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataShui");
            self.u_DataBing = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataBing");
            self.u_DataLei = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLei");
            self.u_DataFeng = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataFeng");
            self.u_DataElement = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueString>("u_DataElement");
            self.u_DataCalamityDate = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCalamityDate");
            self.u_DataCalamityDamage = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataCalamityDamage");
            self.u_DataManaValue = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataManaValue");
            self.u_DataInfoTipsState = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueBool>("u_DataInfoTipsState");
            self.u_DataLifeValue = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLifeValue");
            self.u_DataLifeMax = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataLifeMax");
            self.u_DataGoldCoin = self.UIBase.DataTable.FindDataValue<YIUIFramework.UIDataValueInt>("u_DataGoldCoin");

        }
    }
}
