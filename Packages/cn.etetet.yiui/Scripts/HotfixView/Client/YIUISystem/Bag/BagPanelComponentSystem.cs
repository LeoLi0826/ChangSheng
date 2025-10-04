using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.7
    /// Desc
    /// </summary>
    [FriendOf(typeof(BagPanelComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class BagPanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this BagPanelComponent self)
        {
            self.u_ComItemDescRectTransform.gameObject.SetActive(false);
        }

        [EntitySystem]
        private static void Destroy(this BagPanelComponent self)
        { 
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this BagPanelComponent self)
        {
            await ETTask.CompletedTask;
            return true;
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this BagPanelComponent self, EBagPanelViewEnum viewEnum)
        {
            Debug.Log("leo:YIUIOpen 带参数的 " + viewEnum);
            Debug.Log("leo:YIUIOpen 带参数的 " + (int)viewEnum);
            Debug.Log("leo:YIUIOpen 带参数的 " + viewEnum.ToString());

            self.u_DataView.SetValue((int)viewEnum);

            await self.UIPanel.OpenViewAsync(viewEnum.ToString());//viewEnum.ToString());

            return true;
        }
      
        public static void UpdatePos(this BagPanelComponent self, RectTransform target, Vector3 offset = default)
        {
            self.Target = target;
            if (self.Target == null || self.u_ComItemDescRectTransform == null)
                return;

            // 获取必要的RectTransform组件
            var tipsRect = self.u_ComItemDescRectTransform;
            var buttonRect = self.Target.transform as RectTransform;
            var canvasRect = self.YIUIMgr().UICanvas.GetComponent<RectTransform>();
            if (buttonRect == null || canvasRect == null)
                return;

            // 计算基础位置
            Vector3 buttonWorldPos = buttonRect.TransformPoint(Vector3.zero);
            Vector3 tipsLocalPos = tipsRect.parent.InverseTransformPoint(buttonWorldPos);
            tipsLocalPos += offset;

            // 水平位置限制
            float maxHorizontalOffset = canvasRect.rect.width * 0.5f - tipsRect.rect.width * 0.5f;
            tipsLocalPos.x = Mathf.Clamp(tipsLocalPos.x, -maxHorizontalOffset, maxHorizontalOffset);

            // 计算垂直空间
            float totalHeight = tipsRect.rect.height;
            float spaceAbove = canvasRect.rect.height * 0.5f - (tipsLocalPos.y + buttonRect.rect.height * 0.5f);
            float spaceBelow = canvasRect.rect.height * 0.5f + (tipsLocalPos.y - buttonRect.rect.height * 0.5f);

            // 决定显示在按钮上方还是下方
            bool showAbove = spaceBelow < totalHeight && spaceAbove > totalHeight;
            float verticalOffset = (showAbove ? 1 : -1) * (totalHeight * 0.5f + buttonRect.rect.height * 0.5f);
            // 设置最终位置
            tipsRect.anchoredPosition3D = tipsLocalPos + new Vector3(0, verticalOffset, 0);
        }
        public static string LevelInttoString(this BagPanelComponent self, int levelInt, int levelType = 0)
        {
            switch (levelInt)
            {
                case 1:
                    return "普通";
                case 2:
                    return "精良";
                case 3:
                    return "稀有";
                case 4:
                    return "史诗";
                case 5:
                    return "传说";
                case 6:
                    return "神话";
            }

            return null;
        }
        
        public static string ElementInttoString(this BagPanelComponent self, int ElementInt )
        {
            switch (ElementInt)
            {
                case 1:
                    return "金";
                case 2:
                    return "木";
                case 3:
                    return "水";
                case 4:
                    return "火";
                case 5:
                    return "土";
            }

            return null;
        }
        #region YIUIEvent开始

        //返回主页面按钮
        private static async ETTask OnEventHomeActionAction(this BagPanelComponent self)
        {
            await ETTask.CompletedTask;
        }

        //返回按钮
        private static async ETTask OnEventBackActionAction(this BagPanelComponent self)
        {
            self.UIPanel.Close();
            await ETTask.CompletedTask;
        }

        //打开物品界面
        private static async ETTask OnEventOpenItemActionAction(this BagPanelComponent self)
        {
            Debug.Log("我打开了物品界面");
            await self.UIPanel.OpenViewAsync(EBagPanelViewEnum.BagItemView.ToString());//viewEnum.ToString());

            await ETTask.CompletedTask;


        }

        //打开道具界面
        private static async ETTask OnEventOpenToolActionAction(this BagPanelComponent self)
        {
            Debug.Log("我打开了道具界面");
            await self.UIPanel.OpenViewAsync(EBagPanelViewEnum.BagToolView.ToString());//viewEnum.ToString());

            await ETTask.CompletedTask;
        }
        //打开其他界面
        private static async ETTask OnEventOpenOtherActionAction(this BagPanelComponent self)
        {
            Debug.Log("我打开了其他界面");
            await self.UIPanel.OpenViewAsync(EBagPanelViewEnum.BagOtherView.ToString());//viewEnum.ToString());
            await ETTask.CompletedTask;
        }


        //测试刷新背包
        private static async ETTask OnEventTestFreshAction(this BagPanelComponent self)
        {
            await self.DynamicEvent(new EventBagItemReFresh());
            await ETTask.CompletedTask;
        }
        //测试拉杆收获
        private static async ETTask OnEventTestGetAction(this BagPanelComponent self)
        {
            //这里发送寻路消息给服务器
        
            await ETTask.CompletedTask;
        }

        private static async ETTask OnEventObtainAction(this BagPanelComponent self)
        {
            await ETTask.CompletedTask;
        }

        private static async ETTask OnEventCollectAction(this BagPanelComponent self)
        {
            await ETTask.CompletedTask;
        }

        private static async ETTask OnEventSellAction(this BagPanelComponent self)
        {
            await ETTask.CompletedTask;
        }
        #endregion YIUIEvent结束
    }
}