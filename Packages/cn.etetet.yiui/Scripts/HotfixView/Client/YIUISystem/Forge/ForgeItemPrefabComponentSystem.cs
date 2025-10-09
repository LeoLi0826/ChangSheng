using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2025.1.15
    /// Desc
    /// </summary>
    /// 合成台的格子
    [FriendOf(typeof(ForgeItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ForgeItemPrefabComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this ForgeItemPrefabComponent self)
        {
            self.u_ComItemStateUIDragItem.Entity = self;
            self.u_ComItemStateUIDragItem.Canvas = self.YIUIMgr().UICanvas;
            self.u_ComItemStateUIDragItem.CanReceive = true;
        }

        [EntitySystem]
        private static void Destroy(this ForgeItemPrefabComponent self)
        {
        }

        #region YIUIEvent开始

        private static void OnEventSelectAction(this ForgeItemPrefabComponent self)
        {
            Item item = self.ItemDataInfo;
            if(item == null)return;
            TipsHelper.OpenSync<ItemTipsViewComponent>(self.Scene(),new ItemTipsExtraData()
            {
                Item = item,
                Position = self.UIBase.OwnerRectTransform.position,
            });
        }
        #endregion YIUIEvent结束


        //刷新物品
        public static void ResetItem(this ForgeItemPrefabComponent self, Item info)
        {
            self.u_ComItemStateUIDragItem.CanDrag = info != null;
            //这里的格子的顺序可能有问题
            self.u_DataSlotState.SetValue(info != null?1:0);
            
            if (info != null)
            {
                self.ItemDataInfo = info;
                ItemConfig itemConfig = info.config;

                self.u_DataIcon.SetValue(itemConfig.Icon);

                self.u_DataCount.SetValue(info.Count);
                self.u_DataItemType.SetValue(itemConfig.Type);
            }
            else
            {
                self.ItemDataInfo = default;
            }
        }
    }
}