using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.8
    /// Desc
    /// </summary>
    [FriendOf(typeof(BagItemViewComponent))]
    [FriendOf(typeof(Item))]
    public static partial class BagItemViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this BagItemViewComponent self)
        {

            self.m_ItemListScroll = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComLoopScrollVerticalGroupRect, typeof(ItemPrefabComponent), "u_EventSelect");
        }

        [EntitySystem]
        private static void Destroy(this BagItemViewComponent self)
        {

        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this BagItemViewComponent self)
        {
            await ETTask.CompletedTask;
            //刷新背包物品数据
            //self.RefreshItemList();
            // await self.Fiber().UIEvent(new EventReFresh());

            //初始化数据   也包括了 无限循环列表初始化
            await self.DynamicEvent(new EventBagItemReFresh());

            //无限循环列表初始化 调用
            // self.RefreshItem();



            return true;
        }

        #region YIUIEvent开始

        #endregion YIUIEvent结束



        [EntitySystem]
        private static void YIUILoopRenderer(this BagItemViewComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.ResetItem(data);

            item.SelectItem(select);
            if (select)
            {
            }
        }

        [EntitySystem]
        private static void YIUILoopOnClick(this BagItemViewComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.SelectItem(select);

            item.BagInfoShowSelect(self.m_ItemDataList[index]);
        }

        //刷新物品信息界面 切换场景时 
        [EntitySystem]
        private static async ETTask DynamicEvent(this BagItemViewComponent self, BagItemViewFresh message)
        {
            await self.DynamicEvent(new EventBagItemReFresh());

            //await ETTask.CompletedTask;
        }

        //刷新物品信息界面
        [EntitySystem]
        private static async ETTask DynamicEvent(this BagItemViewComponent self, EventBagItemReFresh message)
        {
            self.m_ItemDataList.Clear();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Backpack, self.m_ItemDataList);
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.Backpack);
            int addNum = itemContainerConfig.CellCountMax - self.m_ItemDataList.Count;
            for (int i = 0; i < addNum; i++)
            {
                self.m_ItemDataList.Add(null);
            }

            self.RefreshItem();
            await ETTask.CompletedTask;
        }



        //刷新物品界面
        public static void RefreshItem(this BagItemViewComponent self)
        {
            if (self.m_ItemDataList.Count == 0)
            {
                return;
            }

            //刷新数据

            self.ItemListScroll.SetDataRefresh(self.m_ItemDataList).NoContext();
        }
    }
}