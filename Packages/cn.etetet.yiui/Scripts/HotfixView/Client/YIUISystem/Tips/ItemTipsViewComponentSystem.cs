using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2025.9.8
    /// Desc
    /// </summary>
    [FriendOf(typeof(ItemTipsViewComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemTipsViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this ItemTipsViewComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this ItemTipsViewComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this ItemTipsViewComponent self)
        {
            await ETTask.CompletedTask;
            return true;
        }

        [EntitySystem]
        private static async ETTask YIUIOpenTween(this ItemTipsViewComponent self)
        {
            await WindowFadeAnim.In(self.UIBase.OwnerGameObject);
        }

        [EntitySystem]
        private static async ETTask YIUICloseTween(this ItemTipsViewComponent self)
        {
            await WindowFadeAnim.Out(self.UIBase.OwnerGameObject);
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this ItemTipsViewComponent self, ParamVo vo)
        {
            await ETTask.CompletedTask;
            self.ExtraData = vo.Get(0, new ItemTipsExtraData());
            Item item = self.ExtraData.Item;
            ItemConfig itemConfig = ItemConfigCategory.Instance.Get(item.ConfigId);
            switch ((ItemType)itemConfig.Type)
            {
                case ItemType.Weapon:
                    TalismanConfig talismanConfig = TalismanConfigCategory.Instance.Get(item.ConfigId);
                    self.u_DataHuo.SetValue(talismanConfig.Huo);
                    self.u_DataShui.SetValue(talismanConfig.Shui);
                    self.u_DataBing.SetValue(talismanConfig.Bing);
                    self.u_DataLei.SetValue(talismanConfig.Lei);
                    self.u_DataFeng.SetValue(talismanConfig.Feng);
                    break;
                default:
                    self.u_DataHuo.SetValue(itemConfig.Huo);
                    self.u_DataShui.SetValue(itemConfig.Shui);
                    self.u_DataBing.SetValue(itemConfig.Bing);
                    self.u_DataLei.SetValue(itemConfig.Lei);
                    self.u_DataFeng.SetValue(itemConfig.Feng);
                    break;
            }
            self.u_DataName.SetValue(itemConfig.Name);
            self.u_DataCollection.SetValue(itemConfig.BaseScore);
            self.u_DataElement.SetValue(itemConfig.Element);
            self.u_DataIcon.SetValue(itemConfig.Icon);
            self.u_DataMessage.SetValue(itemConfig.Desc);
            return true;
        }

        //消息 回收对象
        [EntitySystem]
        private static async ETTask DynamicEvent(this ItemTipsViewComponent self, ItemTipsClose message)
        {
            await self.OnEventCloseAction();
        }

        #region YIUIEvent开始

        private static async ETTask OnEventCloseAction(this ItemTipsViewComponent self)
        {
            self.UIView.Close();
            await ETTask.CompletedTask;
        }
        #endregion YIUIEvent结束
    }
}