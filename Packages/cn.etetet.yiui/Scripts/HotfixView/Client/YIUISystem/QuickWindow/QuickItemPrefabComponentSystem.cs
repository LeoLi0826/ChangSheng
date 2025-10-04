using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2025.9.3
    /// Desc
    /// </summary>
    [FriendOf(typeof(QuickItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class QuickItemPrefabComponentSystem
    {
      
        [EntitySystem]
        private static void YIUIInitialize(this QuickItemPrefabComponent self)
        {
            self.u_ComItemStateUIDragItem.Entity = self;
            self.u_ComItemStateUIDragItem.Canvas = self.YIUIMgr().UICanvas;
            self.u_ComItemStateUIDragItem.CanReceive = true;
        }

        [EntitySystem]
        private static void Destroy(this QuickItemPrefabComponent self)
        {

        }

        #region YIUIEvent开始

        //鼠标移入显示物品详情
        private static void OnEventPointerEnterAction(this QuickItemPrefabComponent self)
        {
            Item item = self.ItemDataInfo;
            if (item == null) return;
            TipsHelper.OpenSync<ItemTipsViewComponent>(self.Scene(),new ItemTipsExtraData()
            {
                Item = item,
                Position = self.UIBase.OwnerRectTransform.position,
            });
        }

        //点击打开物品详情（保留原有功能）
        public static void OnEventSelectAction(this QuickItemPrefabComponent self)
        {
            Item item = self.ItemDataInfo;
            if (item == null) return;
            TipsHelper.OpenSync<ItemTipsViewComponent>(self.Scene(),new ItemTipsExtraData()
            {
                Item = item,
                Position = self.UIBase.OwnerRectTransform.position,
            });
        }

        #endregion YIUIEvent结束


        //刷新物品
        public static void ResetItem(this QuickItemPrefabComponent self, Item info)
        {
            self.u_ComItemStateUIDragItem.CanDrag = info != null;
            //这里的格子的顺序可能有问题
            self.ItemDataInfo = info;
            self.u_DataSlotState.SetValue(info != null ? 1 : 0);
            if (info != null)
            {
                ItemConfig itemConfig = info.config;

                //Log.Debug("快捷物品名字为：" + info.Name + "物品个数为：" + info.Count + "物品状态为：" + info.SlotState + " 物品图标为：" + info.ItemIcon + " 物品ID为：" + info.ItemId);
                //这个是是否显示数字下标

                self.u_DataIcon.SetValue(itemConfig.Icon);

                self.u_DataCount.SetValue(info.Count);
                self.u_DataItemType.SetValue(itemConfig.Type);
            }
            else
            {
                self.u_DataIcon.SetValue("");
                self.u_DataCount.SetValue(0);
            }
        }

        public static void ResetItemFunction(this QuickItemPrefabComponent self, Item info)
        {
            self.u_ComItemStateUIDragItem.CanDrag = info != null;
            //这里的格子的顺序可能有问题
            self.ItemDataInfo = info;
            self.u_DataSlotState.SetValue(info != null ? 1 : 0);
            if (info != null)
            {
                ItemConfig itemConfig = info.config;

                //Log.Debug("快捷物品名字为：" + info.Name + "物品个数为：" + info.Count + "物品状态为：" + info.SlotState + " 物品图标为：" + info.ItemIcon + " 物品ID为：" + info.ItemId);
                //这个是是否显示数字下标
                self.u_DataSlotState.SetValue(info.Count);

                self.u_DataIcon.SetValue(itemConfig.Icon);

                self.u_DataCount.SetValue(info.Count);
                self.u_DataItemType.SetValue(itemConfig.Type);
            }
            else
            {
                self.u_DataIcon.SetValue("");
                self.u_DataSlotState.SetValue(0);
            }
        }

        public static void SelectItem(this QuickItemPrefabComponent self, bool value, int type = 1, Item data = null)
        {

            switch (type)
            {
                //正常情况 即背包打开
                case 1:
                    // self.u_DataSelect.SetValue(value);
                    break;

                //合成界面打开 需发送回数据给合成界面刷新
                case 2:
                    if (data == null)
                    {
                        Debug.Log("我发送信息给合成界面 本物体的信息为 为空");
                        return;
                    }
                    else
                    {
                        ItemConfig itemConfig = data.config;
                        Debug.Log("我发送信息给合成界面 本物体的信息为： " + itemConfig.Icon);

                    }

                    self.DynamicEvent(new EventSelectItemFresh() { bagItemData = data }).NoContext();
                    break;
            }
        }

        public static void SelectItemFunction(this QuickItemPrefabComponent self, bool value, int type = 1, Item data = null)
        {
            switch (type)
            {
                //正常情况 即背包打开
                case 1:
                    // self.u_DataSelect.SetValue(value);
                    break;

                //合成界面打开 需发送回数据给合成界面刷新
                case 2:
                    if (data == null)
                    {
                        Debug.Log("我发送信息给合成界面 本物体的信息为 为空");
                        return;
                    }
                    else
                    {
                        ItemConfig itemConfig = data.config;
                        Debug.Log("我发送信息给合成界面 本物体的信息为： " + itemConfig.Icon);

                    }

                    self.DynamicEvent(new EventSelectItemFresh() { bagItemData = data }).NoContext();
                    break;
            }
        }



        // 合成台状态
        [EntitySystem]
        private static async ETTask DynamicEvent(this ET.Client.QuickItemPrefabComponent self, ET.Client.EventQuickItemForgeState param1)
        {
            Debug.Log("我刷新了合成台状态！ " + param1.State);

            await ETTask.CompletedTask;
        }
     
        public static void BagInfoShowSelect(this QuickItemPrefabComponent self, Item data)
        {


        }
    }
}
