using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using System.Timers;
using UnityEngine.EventSystems;
using ET;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.18
    /// Desc
    /// </summary>
    [FriendOf(typeof(PlayerUIPanelComponent))]
    public static partial class PlayerUIPanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this PlayerUIPanelComponent self)
        {
            //一些初始化
            self.OpenInit();
        }

        public static void OpenInit(this PlayerUIPanelComponent self)
        {
            self.originPos = self.u_ComHandleImgTransform.transform.position;

            self.FishingFlag = false;

            self.u_DataButtonState.SetValue(1);
        }
        [EntitySystem]
        private static void Destroy(this PlayerUIPanelComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this PlayerUIPanelComponent self)
        {

            //一些初始化
            self.OpenInit();

            Log.Debug("打开了人物栏");


            PlayerComponent playerComponent = self.Root().GetComponent<PlayerComponent>();

            //刷新数据 通知所有ui界面的刷新
            // EventSystem.Instance.Publish( new NumericReFresh(){});
            // await self.Fiber().UIEvent(new NumericReFresh() {} );
            await DataRefeshHelper.DataRefesh(self.Root());


            self.unit = self.Root().CurrentScene().GetComponent<UnitComponent>().Get(playerComponent.MyId);

            //Unit unit = self.Root().CurrentScene().GetComponent<UnitComponent>().Get(playerComponent.MyId);

            // if (unit.GetComponent<Rigidbody>() == null)
            // {
            //     Debug.Log("物体上带有 Rigidbody 组件");
            // }
            // else
            // {
            //     Debug.Log("物体上没有 Rigidbody 组件");
            // }
            await ETTask.CompletedTask;
            return true;
        }

        #region YIUIEvent开始

        
        [EntitySystem]
        private static async ETTask DynamicEvent(this ET.Client.PlayerUIPanelComponent self, ET.Client.FishingFlag param1)
        {
            //关闭之前 保存之前失败的信息 可能不需要
            self.FishingFlag = param1.flag;
            await ETTask.CompletedTask;
        }
        [EntitySystem]
        private static async ETTask DynamicEvent(this ET.Client.PlayerUIPanelComponent self, ET.Client.GetResource param1)
        {
            switch (param1.TagType)
            {
                //普通模式 暂时没想好
                case 1:
                    break;
                //钓鱼获取
                case 2:
                    //关闭之前 保存之前失败的信息 可能不需要
                    self.u_DataButtonState.SetValue(2);

                    break;
                //获取资源
                case 3:
                    //关闭之前 保存之前失败的信息 可能不需要
                    self.u_DataButtonState.SetValue(3);

                    //物品信息
                    self.ResourceId = param1.UnitId;
                    break;
                //模式能量
                case 4:
                    //关闭之前 保存之前失败的信息 可能不需要
                    self.u_DataButtonState.SetValue(4);
                    //物品信息
                    self.ResourceId = param1.UnitId;
                    break;

            }
            await ETTask.CompletedTask;
        }
        
        [EntitySystem]
        private static async ETTask DynamicEvent(this ET.Client.PlayerUIPanelComponent self, ET.Client.OutResource param1)
        {
            //关闭之前 保存之前失败的信息 可能不需要
            self.u_DataButtonState.SetValue(1);

            //物品信息
            self.ResourceId = 0;
            await ETTask.CompletedTask;
        }
        
        //点击钓鱼
        private static async ETTask OnEventFinshingAction(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            if (self.FishingFlag == true)
            {
                await ETTask.CompletedTask;
                return;
            }

            //是否有需要保存鱼的信息，确保关闭相关窗口
            await self.FishingViewCloseInit();

            // await self.YIUIRoot().OpenPanelAsync<FishingPanelComponent, EFishingPanelViewEnum>(EFishingPanelViewEnum.ThrowingView);

            //await self.Fiber().UIEvent(new EventThrowing());
            await ETTask.CompletedTask;
        }

        //关闭钓鱼窗口
        public static async ETTask FishingViewCloseInit(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            Log.Debug("PlayerUi关闭钓鱼窗口");
            //关闭收获窗口
            // await self.DynamicEvent(new HarvestCloseInit());
            // 关闭失败窗口
            // await self.DynamicEvent(new FailCloseInit());

            //有可能需要关闭其他窗口
            await ETTask.CompletedTask;
        }


        [YIUIInvoke(PlayerUIPanelComponent.OnEventForgeActionInvoke)]
        private static async ETTask OnEventForgeActionInvoke(this PlayerUIPanelComponent self)
        {
            
            await self.YIUIRoot().OpenPanelAsync<ForgePanelComponent, EForgePanelViewEnum>(EForgePanelViewEnum.ForgeItemView);
            //关闭快捷栏的信息框
            await self.DynamicEvent(new EventQuickItemForgeState() { State = 1 });
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventTestOpenModalTipPanelInvoke)]
        private static async ETTask OnEventTestOpenModalTipPanelInvoke(this PlayerUIPanelComponent self)
        {
            //await YIUIMgrComponent.Inst.Root.OpenPanelAsync<ModalTipsPanelComponent, RectTransform, Item>(self.u_ComButton_AddButton.transform as RectTransform, null);//test
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventHandBookInvoke)]
        private static async ETTask OnEventHandBookInvoke(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<HandBookPanelComponent, EHandBookPanelViewEnum>(EHandBookPanelViewEnum.FishView);

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventDataTestInvoke)]
        private static async ETTask OnEventDataTestInvoke(this PlayerUIPanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventHomeInvoke)]
        private static async ETTask OnEventHomeInvoke(this PlayerUIPanelComponent self)
        {
            
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<HomePanelComponent>();

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventButtonInteractionInvoke)]
        private static async ETTask OnEventButtonInteractionInvoke(this PlayerUIPanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventShopInvoke)]
        private static async ETTask OnEventShopInvoke(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<ShopPanelComponent, EShopPanelViewEnum>(EShopPanelViewEnum.ShopGlodView);

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventBagInvoke)]
        private static async ETTask OnEventBagInvoke(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            await self.YIUIRoot().OpenPanelAsync<BagPanelComponent, EBagPanelViewEnum>(EBagPanelViewEnum.BagItemView);

            //刷新背包
            // await self.Fiber().UIEvent(new EventReFresh());

            await ETTask.CompletedTask;
        }
        
        [YIUIInvoke(PlayerUIPanelComponent.OnEventDragInvoke)]
        private static async ETTask OnEventDragInvoke(this PlayerUIPanelComponent self)
        {
            
            await ETTask.CompletedTask;
        }
        #endregion YIUIEvent结束


        public static void ShowWindow(this PlayerUIPanelComponent self)
        {
            //注意 这里的TimerType 可能会导致出错 在6.0的位置 和在8.0位置不一样 8.0放在了share目录下面
            // self.joyMoveTimerId = self.Root().GetComponent<TimerComponent>().NewFrameTimer(TimerType.JoyMoveTimer, self);
        }

        public static void HideWindow(this PlayerUIPanelComponent self)
        {
            // self.Root().GetComponent<TimerComponent>().Remove(ref self.joyMoveTimerId);
        }

        //人物界面 数据刷新
        [EntitySystem]
        private static async ETTask DynamicEvent(this PlayerUIPanelComponent self, PlayerUIDateRefresh message)
        {
            Debug.Log("人物主界面刷新 " + message.date.GetAsFloat(NumericType.Speed));

            self.u_DataLevel.SetValue(message.date.GetAsInt(NumericType.Level));
            self.u_DataExp.SetValue(message.date.GetAsInt(NumericType.Exp));
            self.u_DataGold.SetValue(message.date.GetAsInt(NumericType.Gold));
            self.u_DataDiamond.SetValue(message.date.GetAsInt(NumericType.Diamond));
            // self.u_DataEnergy.SetValue(message.date.GetAsInt(NumericType.Energy));
            await ETTask.CompletedTask;
        }

        [EntitySystem]
        private static async ETTask DynamicEvent(this PlayerUIPanelComponent self, EventThrowingCheckEnergy message)
        {


            await ETTask.CompletedTask;
        }

    }
}