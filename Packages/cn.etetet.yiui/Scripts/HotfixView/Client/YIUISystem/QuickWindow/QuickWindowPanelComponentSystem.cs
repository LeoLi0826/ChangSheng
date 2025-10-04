using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2025.3.19
    /// Desc
    /// </summary>
    [FriendOf(typeof(QuickWindowPanelComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    [FriendOfAttribute(typeof(ET.Client.QuickItemPrefabComponent))]
    public static partial class QuickWindowPanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this QuickWindowPanelComponent self)
        {
            Debug.Log("快捷item初始化无限循环");
            //初始化无限循环列表 
            self.m_LoopScrollQuick = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComLoopScrollQuick, typeof(QuickItemPrefabComponent), "u_EventSelect");
          
            self.m_LoopScrollFunction = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComLoopScrollFunction, typeof(QuickItemPrefabComponent), "u_EventSelect");

            // self.u_DataInfoState.SetValue(false);
            self.u_ComItemDescRectTransform.gameObject.SetActive(false);

            PlayerDataInit(self);
        }

        [EntitySystem]
        private static void Destroy(this QuickWindowPanelComponent self)
        {

        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this QuickWindowPanelComponent self)
        {
            await self.DynamicEvent(new EventQuickItemReFresh());
            await self.DynamicEvent(new EventQuickItemFunctionReFresh());

            await ETTask.CompletedTask;
            return true;
        }



        #region YIUIEvent开始

        //刷新物品信息界面 切换场景时 RefreshBagItemView_Event里面刷新
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, QuickWindowFresh message)
        {
            await self.DynamicEvent(new EventQuickItemReFresh());
            await self.DynamicEvent(new EventQuickItemFunctionReFresh());
            //await ETTask.CompletedTask;
        }

        #endregion YIUIEvent结束

        #region 快捷背包
        //刷新物品信息界面 这个是我添加物品或者干嘛的操作之后 刷新 但是 index 我不会刷新这个
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, EventQuickItemReFresh message)
        {

            //清空数据
            //self.m_ItemQuickDataList.Clear();
            self.quickList.Clear();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Backpack, self.quickList);
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.Backpack);

            int addNum = itemContainerConfig.CellCountMax - self.quickList.Count;
            for (int i = 0; i < addNum; i++)
            {
                //填充
                self.quickList.Add(null);
            }



            //无限循环列表初始化 调用 这里开始调用刷新
            await self.LoopScrollQuick.SetDataRefresh(self.quickList);
        }

        
        [EntitySystem]
        private static void YIUILoopRenderer(this QuickWindowPanelComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.IsEquipBox = false;
            item.ResetItem(data);

            item.SelectItem(select);
            if (select)
            {
            }
        }

        [EntitySystem]
        private static void YIUILoopOnClick(this QuickWindowPanelComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.BagInfoShowSelect(data);
        }

        #endregion

        #region 功能装备栏

        //刷新功能装备栏
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, EventQuickItemFunctionReFresh message)
        {
            self.functionList.Clear();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Fast, self.functionList);
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.Fast);
            int addNun = itemContainerConfig.CellCountMax - self.functionList.Count;
            for (int i = 0; i < addNun; i++)
            {
                self.functionList.Add(null);
            }
            await self.LoopScrollFunction.SetDataRefresh(self.functionList);
        }


        //无限循环列表 数据渲染到组件上去
        //第几个，数据，组件，是否选中
        private static void ItemCommandRendererFunction(this QuickWindowPanelComponent self, int index, EntityRef<Item> data,
        QuickItemPrefabComponent item, bool select)
        {
            item.IsEquipBox = true;
            Debug.Log("快捷背包系统：ItemCommandRenderer刷新");
            item.ResetItemFunction(data);

            item.SelectItemFunction(select);
            if (select)
            {
                Debug.Log("快捷背包系统：点击了第" + index + "个格子");
                //self.SelectTitleRefreshCommand(data);
            }
        }

        public static void OnClickItemFunction(this QuickWindowPanelComponent self, int index, EntityRef<Item> data, QuickItemPrefabComponent item,
        bool select)
        {
            Debug.Log("快捷背包系统：点击了第" + index + "个功能格子");
            item.SelectItemFunction(select);

            //发送信息给主panel 显示物品信息

            //显示第几个信息
            item.BagInfoShowSelect(data);
        }

        #endregion



        #region 功能法宝效果

        //功能法宝属性生效
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, EventFunctionCalReFresh message)
        {
            //初始化
            self.ScoreMult = 0;
            self.ScoreAdd = 0;

            Debug.Log("我是功能法宝的数据统计1:");
        }

        #endregion


        #region 人物面板相关
        //法力刷新
        public static void ManaInfoRefresh(this QuickWindowPanelComponent self, long manaValue)
        {

            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            numericComponent[NumericType.Mp] = manaValue;
            // UI界面显示
            //法力值最大值
            self.u_DataManaMax.SetValue((int)numericComponent[NumericType.MaxMp]);
            //法力值百分比
            self.u_DataManaPrecent.SetValue(numericComponent[NumericType.Mp] / (float)numericComponent[NumericType.MaxMp]);
            self.u_DataManaValue.SetValue((int)numericComponent[NumericType.Mp]);
            // 启动法力值衰减定时器
            //StartManaDecayTimer(self);
        }


        //天劫倒计时刷新
        public static void CalamityRefresh(this QuickWindowPanelComponent self)
        {

            self.u_DataCalamityDate.SetValue(self.CalamityDate);


        }

        //法力增加
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, ManaAddRefresh message)
        {
            int addValue = message.date;
            if (addValue <= 0)
            {
                Log.Debug("法力增加值必须大于0");
                return;
            }
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            if (numericComponent[NumericType.Mp] + addValue > numericComponent[NumericType.MaxHp])
            {
                self.CalamityDate = 0;
            }

            //这里会涉及到升级突破
            // 计算新的法力值，不超过最大值
            long newManaValue = Math.Min(numericComponent[NumericType.Mp] + addValue, numericComponent[NumericType.MaxMp]);

            // 如果法力值没有变化，直接返回
            if (newManaValue == numericComponent[NumericType.Mp])
            {
                Log.Debug("法力值没有变化");
                return;
            }
            self.ManaInfoRefresh(newManaValue);
            await ETTask.CompletedTask;
        }

        //法力减少
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, ManaReduceRefresh message)
        {
            int reduceValue = message.date;
            if (reduceValue <= 0)
            {
                Log.Debug("法力减少值必须大于0");
                return;
            }
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            // 计算新的法力值，不低于0
            long newManaValue = Math.Max(numericComponent[NumericType.Mp] - reduceValue, 0);

            // 如果法力值没有变化，直接返回
            if (newManaValue == numericComponent[NumericType.Mp])
            {
                Log.Debug("法力值已达到最小值");
                return;
            }
            self.ManaInfoRefresh(newManaValue);
            await ETTask.CompletedTask;
        }

        //法力上限增加
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, ManaMaxAddRefresh message)
        {
            int reduceValue = message.date;
            if (reduceValue <= 0)
            {
                Log.Debug("法力Max减少值必须大于0");
                return;
            }
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            long newManaValue = numericComponent[NumericType.MaxMp] + reduceValue;

            // 如果法力值没有变化，直接返回
            if (newManaValue == numericComponent[NumericType.MaxMp])
            {
                Log.Debug("法力值已达到最小值");
                return;
            }

            numericComponent[NumericType.MaxMp] = newManaValue;
            numericComponent[NumericType.Mp] = numericComponent[NumericType.MaxMp];
            self.ManaInfoRefresh(numericComponent[NumericType.Mp]);
            await ETTask.CompletedTask;
        }

        //生命刷新
        public static void LifeInfoRefresh(this QuickWindowPanelComponent self, long hp)
        {
            //最大生命值
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            // 使用SetNoEvent避免重复触发NumericWatcher
            numericComponent.SetNoEvent(NumericType.Hp, hp);
            self.u_DataLifeMax.SetValue((int)numericComponent[NumericType.MaxMp]);
            self.u_DataLifeValue.SetValue((int)numericComponent[NumericType.Hp]);
            self.u_DataLife.SetValue((float)numericComponent[NumericType.Hp] / numericComponent[NumericType.MaxHp]);

        }

        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, LifeRefresh message)
        {
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            // 只读取当前血量并更新UI显示，不再重复扣血
            long currentHp = numericComponent[NumericType.Hp];
            self.LifeInfoRefresh(currentHp);
            await ETTask.CompletedTask;
        }

        //初始化
        public static void PlayerDataInit(this QuickWindowPanelComponent self)
        {
            //天劫开关
            self.CalamityFlag = false;

            NumericComponent numericComponent = UnitHelper.GetMyUnitNumericComponent(self.Root().CurrentScene());
            //天劫
            self.CalamityDate = 100;
            self.CalamityDamage = 2001;

            self.LifeInfoRefresh(numericComponent[NumericType.Hp]);
            self.ManaInfoRefresh(numericComponent[NumericType.Mp]);
            self.CalamityRefresh();
        }



        [EntitySystem]
        private static void Update(this ET.Client.QuickWindowPanelComponent self)
        {
            // 法力衰减由定时器处理，不需要在这里处理
        }
        #endregion


        #region 游戏结算

        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, CalamitySettlement message)
        {

            await ETTask.CompletedTask;
        }


        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, GameSettlementJudge message)
        {
            // Debug.Log("法力上限 增加："+message.ManaAdd);

            //self.manaMaxValue += message.ManaAdd;
            NumericComponent numericComponent = UnitHelper.GetMyUnitNumericComponent(self.Root().CurrentScene());

            self.ManaInfoRefresh(numericComponent[NumericType.Mp]);
            if (numericComponent[NumericType.Mp] < self.CalamityDamage)
            {
                // await self.YIUIRoot().OpenPanelAsync<FishingPanelComponent, EFishingPanelViewEnum>(EFishingPanelViewEnum.FailView);

                //死亡判定
                self.Die();

                Debug.Log("渡劫失败！ 法力不够 无法抵抗天劫！");
            }
            else
            {
                //self.manaValue -= self.CalamityDamage;
                //渡劫成功 法力恢复到法力上限
                numericComponent[NumericType.Mp] = numericComponent[NumericType.MaxMp];
                self.ManaInfoRefresh(numericComponent[NumericType.Mp]);

                //下一个天劫数据更新
                self.CalamityDate = 200;
                self.CalamityDamage += 1000;

                self.CalamityRefresh();
                //
            }
            await ETTask.CompletedTask;
        }


        //天劫测试 倒计时增加
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, CalamityAdd message)
        {

            int reduceValue = message.day;

            if (reduceValue <= 0)
            {
                Log.Debug("生命减少值必须大于0");
                return;
            }

            // 计算新的生命值，不低于0
            int newDayValue = Math.Max(self.CalamityDate + reduceValue, 0);

            // 如果生命值没有变化，直接返回
            if (newDayValue == self.CalamityDate)
            {
                Log.Debug("天劫已达到最小值");
                return;
            }


            self.CalamityDate = newDayValue;
            self.CalamityRefresh();
            await ETTask.CompletedTask;
            //
        }

        //天劫测试 倒计时减少
        [EntitySystem]
        private static async ETTask DynamicEvent(this QuickWindowPanelComponent self, CalamityReduce message)
        {
            int reduceValue = message.day;

            if (reduceValue <= 0)
            {
                Log.Debug("生命减少值必须大于0");
                return;
            }

            // 计算新的生命值，不低于0
            int newDayValue = Math.Max(self.CalamityDate - reduceValue, 0);

            // 如果生命值没有变化，直接返回
            if (newDayValue == self.CalamityDate)
            {
                Log.Debug("天劫已达到最小值");
                return;
            }

            self.CalamityDate = newDayValue;
            self.CalamityRefresh();
        }

        #endregion

        //死亡判定
        public static void Die(this QuickWindowPanelComponent self)
        {
            PlayerBehaviourComponent PlayerBehaviour = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene()).GetComponent<PlayerBehaviourComponent>();

            PlayerBehaviour.ChangeToDie();
        }


        #region 简介栏
      
        //鼠标跟随简介信息栏
        public static void UpdatePos(this QuickWindowPanelComponent self, RectTransform target, Vector3 offset = default)
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
      
        [EntitySystem]
        public static async ETTask DynamicEvent(this QuickWindowPanelComponent self, QuickTipsClose message)
        {
            self.u_DataInfoTipsState.SetValue(false);
            await ETTask.CompletedTask;
        }
        
        [EntitySystem]
        public static async ETTask DynamicEvent(this QuickWindowPanelComponent self, ModalTipsClose message)
        {
            self.u_ComItemDescRectTransform.gameObject.SetActive(false);
            // self.u_DataInfoState.SetValue(false);
            await ETTask.CompletedTask;
        }

        public static string LevelInttoString(this QuickWindowPanelComponent self, int levelInt, int levelType = 0)
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

        public static string ElementInttoString(this QuickWindowPanelComponent self, int ElementInt)
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

        #endregion

    }
}