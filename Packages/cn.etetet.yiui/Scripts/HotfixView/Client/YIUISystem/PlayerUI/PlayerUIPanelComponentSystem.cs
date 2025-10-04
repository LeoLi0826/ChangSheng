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
            //摇杆事件初始化
            self.eventTriggerInit();

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

        
        
        //打开背包
        private static async ETTask OnEventBagAction(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            await self.YIUIRoot().OpenPanelAsync<BagPanelComponent, EBagPanelViewEnum>(EBagPanelViewEnum.BagItemView);

            //刷新背包
            // await self.Fiber().UIEvent(new EventReFresh());

            await ETTask.CompletedTask;
        }

        //打开商店
        private static async ETTask OnEventShopAction(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<ShopPanelComponent, EShopPanelViewEnum>(EShopPanelViewEnum.ShopGlodView);

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

        //遥感事件
        private static async ETTask OnEventDragAction(this PlayerUIPanelComponent self)
        {
            await ETTask.CompletedTask;
        }

        //回到主页面
        private static async ETTask OnEventHomeAction(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<HomePanelComponent>();

            await ETTask.CompletedTask;
        }

        //测试升级
        private static async ETTask OnEventDataTestAction(this PlayerUIPanelComponent self)
        {
          
            await ETTask.CompletedTask;
        }

        private static async ETTask OnEventHandBookAction(this PlayerUIPanelComponent self)
        {
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
            // await self.YIUIRoot().OpenPanelAsync<HandBookPanelComponent, EHandBookPanelViewEnum>(EHandBookPanelViewEnum.FishView);

            await ETTask.CompletedTask;
        }

        //交互按钮
        private static async ETTask OnEventButtonInteractionAction(this PlayerUIPanelComponent self)
        {
            await ETTask.CompletedTask;
        }



        private static async ETTask OnEventTestOpenModalTipPanelAction(this PlayerUIPanelComponent self)
        {
            //await YIUIMgrComponent.Inst.Root.OpenPanelAsync<ModalTipsPanelComponent, RectTransform, Item>(self.u_ComButton_AddButton.transform as RectTransform, null);//test
            await ETTask.CompletedTask;
        }

        private static async ETTask OnEventForgeActionAction(this PlayerUIPanelComponent self)
        {
            await self.YIUIRoot().OpenPanelAsync<ForgePanelComponent, EForgePanelViewEnum>(EForgePanelViewEnum.ForgeItemView);
            //关闭快捷栏的信息框
            await self.DynamicEvent(new EventQuickItemForgeState() { State = 1 });

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


        //移动 摇杆事件初始化
        #region MyRegion
        private static void eventTriggerInit(this PlayerUIPanelComponent self)
        {
            // 注册 PointerDown 事件
            self.RegisterEvent<PointerEventData>(EventTriggerType.PointerDown, self.OnPointerDown);

            // 注册 PointerUp 事件
            self.RegisterEvent<PointerEventData>(EventTriggerType.PointerUp, self.OnPointerUp);

            // 注册 Drag 事件
            self.RegisterEvent<PointerEventData>(EventTriggerType.Drag, self.OnDrag);
        }

        private static void RegisterEvent<T>(this PlayerUIPanelComponent self, EventTriggerType eventType, Action<T> action) where T : BaseEventData
        {
            // Debug.Log("注册事件！！！");
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            entry.callback.AddListener((data) => action((T)data));
            self.u_ComJoystickEventTrigger.triggers.Add(entry);
        }




        public static void OnPointerDown(this PlayerUIPanelComponent self, PointerEventData eventData)
        {
            //Debug.Log("我按下了按钮！！！");
            //打开
            self.isUpdate = true;

            Vector2 localPos;

            //获取按下的位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(self.u_ComJoystickEventTrigger.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPos);

            //获取移动方向
            self.moveDir = (localPos - self.originPos).normalized;

            //修改按键图标位置
            self.u_ComHandleImgTransform.transform.position = localPos;

            // Vector3 dir = new Vector3(self.moveDir.x, 0, self.moveDir.y).normalized;
            // Vector3 Camepos = (Camera.main.transform.rotation * dir);
            // Vector3 finialmoveDir = new Vector3(Camepos.x, 0, Camepos.z).normalized;
            // self.Root().CurrentScene().GetComponent<OperaComponent>().JoyMove(finialmoveDir);
            self.coolTime = 0;

        }

        //拖拽
        public static void OnDrag(this PlayerUIPanelComponent self, PointerEventData eventData)
        {
            Debug.Log("我拖动了按钮！！！");
            Vector2 localPos;
            //拖拽的实现
            RectTransformUtility.ScreenPointToLocalPointInRectangle(self.u_ComJoystickEventTrigger.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPos);

            if ((localPos - self.originPos).magnitude >= 138.5f)
                localPos = self.originPos + ((localPos - self.originPos).normalized * 138.5f);
            self.moveDir = (localPos - self.originPos).normalized;

            Debug.Log("我拖动了按钮：self.moveDir: " + self.moveDir + " localPos: " + localPos);
            //修改位置
            self.u_ComHandleImgTransform.transform.localPosition = localPos;

        }

        //按键抬起
        public static void OnPointerUp(this PlayerUIPanelComponent self, PointerEventData eventData)
        {
            Debug.Log("我抬起了按钮！！！");
            self.u_ComHandleImgTransform.transform.localPosition = self.originPos;
            self.isUpdate = false;
            self.moveDir = Vector2.zero;
            self.coolTime = 0.0f;
            Unit unit = self.unit;
            unit.GetComponent<PlayerBehaviourComponent>().StopMove();

            //里面涉及服务器相关
            self.Root().CurrentScene().GetComponent<OperaComponent>().StopMove();

        }



        public static void JoyMoveUpdate(this PlayerUIPanelComponent self)
        {
            if (!self.isUpdate)
                return;
            if (self.moveDir == Vector2.zero)
            {
                self.LastDir = Vector2.zero;
                return;
            }

            Debug.Log("移动！！！x: " + self.moveDir.x + " y: " + self.moveDir.y);
            Debug.Log("上次移动！！！x: " + self.LastDir.x + " y: " + self.LastDir.y);


            self.coolTime += Time.deltaTime;
            //Vector3 Camepos = (Camera.main.transform.rotation * new Vector3(self.moveDir.x, 0.0f, self.moveDir.y));
            Vector3 Camepos = new Vector3(self.moveDir.x, self.moveDir.y, 0.0f);

            Vector3 finialmoveDir = new Vector3(Camepos.x, Camepos.y, 0.0f).normalized;
            Debug.Log("显示coolTime: " + self.coolTime + "finalmoveDir: " + finialmoveDir);
            if (self.moveDir != self.LastDir)
            {
                Debug.Log("我进入了移动1");
                if (self.coolTime >= 0.2f)
                {
                    Debug.Log("我进入了移动2");
                    self.Root().CurrentScene().GetComponent<OperaComponent>().OnMove(finialmoveDir);
                    self.coolTime = 0;
                }
            }
            else
            {
                Debug.Log("我进入了移动3");
                if (self.coolTime >= 0.3f)
                {
                    Debug.Log("我进入了移动4");
                    self.Root().CurrentScene().GetComponent<OperaComponent>().OnMove(finialmoveDir);
                    self.coolTime = 0;
                }
            }

            self.LastDir = self.moveDir;

        }

        #endregion


        [EntitySystem]
        private static void Update(this PlayerUIPanelComponent self)
        {
            try
            {
                // Debug.Log("我被调用了！");
                self.JoyMoveUpdate();
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
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