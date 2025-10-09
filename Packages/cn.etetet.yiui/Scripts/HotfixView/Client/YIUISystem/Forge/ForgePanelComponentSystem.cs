using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.7
    /// Desc
    /// </summary>
    [FriendOf(typeof(ForgePanelComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    [FriendOfAttribute(typeof(ET.Client.ItemPrefabComponent))]
    public static partial class ForgePanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this ForgePanelComponent self)
        {
            //锻造输入
            self.m_LoopInputScroll = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComForgeInputLoop, typeof(ItemPrefabComponent), "u_EventSelect");
            //锻造输出
            self.m_LoopOutScroll = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComForgeOutLoop, typeof(ItemPrefabComponent), "u_EventSelect");
        }

        [EntitySystem]
        private static void Destroy(this ForgePanelComponent self)
        {

        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this ForgePanelComponent self)
        {


            await ETTask.CompletedTask;
            return true;
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this ForgePanelComponent self, EForgePanelViewEnum viewEnum)
        {
            self.u_DataView.SetValue((int)viewEnum);

            await self.UIPanel.OpenViewAsync(viewEnum.ToString());
            return true;
        }

        #region YIUIEvent开始

        [YIUIInvoke(ForgePanelComponent.OnEventForgeInvoke)]
        private static async ETTask OnEventForgeInvoke(this ForgePanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventTestGetInvoke)]
        private static async ETTask OnEventTestGetInvoke(this ForgePanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventTestFreshInvoke)]
        private static async ETTask OnEventTestFreshInvoke(this ForgePanelComponent self)
        {
            //await self.Fiber().UIEvent(new EventBagItemReFresh());
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventOpenOtherActionInvoke)]
        private static async ETTask OnEventOpenOtherActionInvoke(this ForgePanelComponent self)
        {
            Debug.Log("我打开了其他界面");
            await self.UIPanel.OpenViewAsync(EForgePanelViewEnum.ForgeOtherView.ToString());//viewEnum.ToString());
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventOpenItemActionInvoke)]
        private static async ETTask OnEventOpenItemActionInvoke(this ForgePanelComponent self)
        {
            Debug.Log("我打开了物品界面");
            await self.UIPanel.OpenViewAsync(EForgePanelViewEnum.ForgeItemView.ToString());//viewEnum.ToString());

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventOpenToolActionInvoke)]
        private static async ETTask OnEventOpenToolActionInvoke(this ForgePanelComponent self)
        {
            Debug.Log("我打开了道具界面");
            await self.UIPanel.OpenViewAsync(EForgePanelViewEnum.ForgeToolView.ToString());//viewEnum.ToString());

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventHomeActionInvoke)]
        private static async ETTask OnEventHomeActionInvoke(this ForgePanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(ForgePanelComponent.OnEventBackActionInvoke)]
        private static async ETTask OnEventBackActionInvoke(this ForgePanelComponent self)
        {
            //开启快捷栏的信息框
            await self.DynamicEvent(new EventQuickItemForgeState() {State = 0});
            self.UIPanel.Close();
            await ETTask.CompletedTask;
        }
        #endregion YIUIEvent结束


        //锻造输入 刷新物品信息界面
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgePanelComponent self, EventForgeInputReFresh message)
        {
            //await ETTask.CompletedTask;
            Debug.Log("锻造输入 刷新事件～！！");

            //清空数据
            //self.m_ItemDataList.Clear();

            //无限循环列表初始化 调用
            self.RefreshItem(self.m_ItemDataInputList, 1);
            await ETTask.CompletedTask;
        }

        //锻造输出 刷新物品信息界面
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgePanelComponent self, EventForgeOutReFresh message)
        {
           
            await ETTask.CompletedTask;
        }


        //刷新物品界面
        public static void RefreshItem(this ForgePanelComponent self, List<EntityRef<Item>> List, int type)
        {
            if (List.Count == 0)
            {
                Debug.Log("锻造系统：没有数据 不刷新背包物品");
                return;
            }

            //刷新数据
            Debug.Log("开始刷新锻造物品");

            //
            switch (type)
            {
                case 1:
                    self.LoopInputScroll.SetDataRefresh(List).NoContext();
                    break;
                case 2:
                    self.LoopOutScroll.SetDataRefresh(List).NoContext();
                    break;
            }

        }

        [EntitySystem]
        private static void YIUILoopRenderer(this ForgePanelComponent self, ItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.ResetItem(data);
            item.SelectItem(select);
        }

        [EntitySystem]
        private static void YIUILoopOnClick(this ForgePanelComponent self, ItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            switch (select)
            {
               
            }
        }
    }

}