using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.8
    /// Desc
    /// </summary>
    [FriendOf(typeof(ItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemPrefabComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this ItemPrefabComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this ItemPrefabComponent self)
        {
        }

        

        public static void ResetItem(this ItemPrefabComponent self, Item info)
        {
            //这里的格子的顺序可能有问题
            self.ItemDataInfo = info;
            ItemConfig itemConfig = info.config;
            //这个是是否显示数字下标
            self.u_DataItemState.SetValue(info.Count);

            self.u_DataIcon.SetValue(itemConfig.Icon);

            self.u_DataCount.SetValue(info.Count);
            self.u_DataItemType.SetValue(itemConfig.Type);
        }

        #region YIUIEvent开始

        private static void OnEventSelectAction(this ItemPrefabComponent self)
        {

        }

        #endregion YIUIEvent结束
        
        #region 鼠标相关事件
        public static void SelectItem(this ItemPrefabComponent self, bool value, int type = 1, Item data = null)
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
            self.u_DataSelect.SetValue(value);
        }

        #endregion

    }
}