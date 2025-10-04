using UnityEngine;
using UnityEngine.UI;
using YIUIFramework;
using System;

namespace ET.Client
{
    /// <summary>
    /// Author  Leo
    /// Date    2024.9.8
    /// Desc
    /// </summary>
    [FriendOf(typeof(ForgeItemViewComponent))]
    [FriendOf(typeof(Item))]
    [FriendOfAttribute(typeof(ET.Client.ItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Puzzle))]
    [FriendOfAttribute(typeof(ET.Client.ForgeComponent))]
    public static partial class ForgeItemViewComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this ForgeItemViewComponent self)
        {
            self.u_ComForgeBgUIDragItem.Entity = self;
            self.u_ComForgeBgUIDragItem.CanDrag = false;
            self.u_ComForgeBgUIDragItem.CanReceive = true;

            self.m_ComForgeScrollRect = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type>(self.u_ComForgeScrollRect, typeof(ForgeItemPrefabComponent));

            self.m_ComForgeReadyScrollRect = self.AddChild<YIUILoopScrollChild, LoopScrollRect, Type, string>(self.u_ComForgeReadyScrollRect, typeof(ForgeItemPrefabComponent), "u_EventSelect");
        }

        [EntitySystem]
        private static void Destroy(this ForgeItemViewComponent self)
        {

        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this ForgeItemViewComponent self)
        {
            self.u_ComForgeLv_SkeSkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
            //刷新所有
            await self.DynamicEvent(new ForgeWindowAllFresh());

            await ETTask.CompletedTask;
            return true;
        }



        //刷新物品信息界面 切换场景时 RefreshBagItemView_Event里面刷新
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, ForgeItemWindowFresh message)
        {
            //刷新窗口所有
            await self.DynamicEvent(new ForgeWindowAllFresh());

            //await ETTask.CompletedTask;
        }

        //刷新窗口所有
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, ForgeWindowAllFresh message)
        {
            //初始化数据   也包括了 无限循环列表初始化
            await self.DynamicEvent(new EventForgeItemReFresh());

            //老的合成台初始化 背包英雄模式 暂时没用到
            await self.DynamicEvent(new EventForgeInputItemReFresh());

            //新合成台 简单版初始化
            await self.DynamicEvent(new EventForgeReFresh());
            //合成法宝预览栏
            await self.DynamicEvent(new EventForgeReadyReFresh());
            //合成法宝预览栏 数据刷新
            await self.DynamicEvent(new EventScoreTotalReFresh());

            Debug.Log("开始初始化元素栏");
            //属性栏的刷新
            await self.DynamicEvent(new EventElementReFresh());


        }

        #region YIUIEvent开始


        //合成按钮点击事件 合成开始
        private static async ETTask OnEventForgeStartAction(this ForgeItemViewComponent self)
        {
            self.u_ComForgeLv_SkeSkeletonGraphic.AnimationState.SetAnimation(0, "fire", true);
            await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            await ForgeHelper.Forge(self.Root());
            self.u_ComForgeLv_SkeSkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
            await ETTask.CompletedTask;
        }


        private static async ETTask OnEventForgeFinishAction(this ForgeItemViewComponent self)
        {
            ForgeHelper.ForgeItem(self.Root());
            await ETTask.CompletedTask;
        }
        #endregion YIUIEvent结束

        #region 刷新背包物品信息界面

        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, EventForgeItemReFresh message)
        {
            //清空数据
            self.m_ForgeDataList.Clear();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Forge, self.m_ForgeDataList);
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.Forge);
            int addNum = itemContainerConfig.CellCountMax - self.m_ForgeDataList.Count;
            for (int i = 0; i < addNum; i++)
            {
                self.m_ForgeDataList.Add(null);
            }

            //无限循环列表初始化 调用 这里开始调用刷新
            self.RefreshItem(3);
            await ETTask.CompletedTask;
        }


        #endregion

        #region 法宝预览栏

        //刷新法宝完成栏信息界面
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, EventForgeReadyReFresh message)
        {
            //清空数据
            self.m_ForgeReadyDataList.Clear();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.ForgeReady, self.m_ForgeReadyDataList);
            ItemContainerConfig itemContainerConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.ForgeReady);
            int addNum = itemContainerConfig.CellCountMax - self.m_ForgeReadyDataList.Count;
            for (int i = 0; i < addNum; i++)
            {
                self.m_ForgeReadyDataList.Add(null);
            }
            self.RefreshItem(4);
            await ETTask.CompletedTask;
        }

        #endregion

        #region 合成台 信息界面刷新

        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, EventElementReFresh message)
        {
            ForgeComponent forgeComponent = self.Root().GetComponent<ForgeComponent>();

            self.u_DataForgeTime.SetValue(forgeComponent.ForgeNum);

            // 计算合成台材料栏里的物品灵气值总和
            // int forgeItemsScore = self.CalculateForgeItemsScore();

            // 计算法宝的灵气加成
            // int talismanBonus = self.CalculateTalismanBonus();

            // 设置总灵气值 = 合成台物品灵气值 + 法宝加成
            // int totalBaseScore = forgeItemsScore;//+ talismanBonus;

            #region 获取材料栏里总灵气值
            ItemContainerComponent itemContainerComponent = self.Root().Scene().GetComponent<ItemContainerComponent>();
            //先找到合成台内的物品（从左往右排序）
            using var list = ListComponent<EntityRef<Item>>.Create();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Forge, list);
            //基础灵气值 = （材料总和的灵气值）
            //self.u_DataScoreBase.SetValue(forgeComponent.Score);

            int baseScore = 0;
            int RideAdd = 1;
            for (int i = 0; i < list.Count; i++)
            {
                Item item = list[i];
                ItemConfig itemConfig = item.config;
                baseScore += itemConfig.BaseScore;
            }
            (int treasureBaseAdd, int treasureMagnificationAdd, int treasureRideAdd) = ForgeHelper.GetTreasureForgeNum(itemContainerComponent);
            self.u_DataScoreBase.SetValue(baseScore + treasureBaseAdd);
            self.u_DataScoreMult.SetValue(treasureRideAdd + RideAdd);
            #endregion


            //法宝灵气预览值
            self.u_DataScoreTotal.SetValue(forgeComponent.FinalScore);

            //五行属性
            forgeComponent.Element.TryGetValue(ItemElementAttr.Huo, out int HuoValue);
            self.u_DataHuo.SetValue(HuoValue);

            forgeComponent.Element.TryGetValue(ItemElementAttr.Shui, out int ShuiValue);
            self.u_DataShui.SetValue(ShuiValue);

            forgeComponent.Element.TryGetValue(ItemElementAttr.Bing, out int BingValue);
            self.u_DataBing.SetValue(BingValue);

            forgeComponent.Element.TryGetValue(ItemElementAttr.Lei, out int LeiValue);
            self.u_DataLei.SetValue(LeiValue);

            forgeComponent.Element.TryGetValue(ItemElementAttr.Feng, out int FengValue);
            self.u_DataFeng.SetValue(FengValue);

            // 五行属性日志
            Log.Info($"=== UI数据赋值 - 五行属性 ===");
            Log.Info($"火属性: {HuoValue}");
            Log.Info($"水属性: {ShuiValue}");
            Log.Info($"冰属性: {BingValue}");
            Log.Info($"雷属性: {LeiValue}");
            Log.Info($"风属性: {FengValue}");
            Log.Info($"========================");


            ElementReactionConfig elementReactionConfig = forgeComponent.ElementReactionConfig;

            // 显示反应信息 - 使用elementReactionConfig的数据
            if (elementReactionConfig != null)
            {
                Log.Info($"=== UI数据赋值 - 反应信息 ===");
                Log.Info($"反应名称: {elementReactionConfig.ReactionName}");
                Log.Info($"反应基础分: {elementReactionConfig.BaseScore}");
                Log.Info($"反应倍乘: {elementReactionConfig.Ride}");
                Log.Info($"反应倍率: {elementReactionConfig.Multiplier}");
                Log.Info($"========================");

                self.u_DataFanYing.SetValue(elementReactionConfig.ReactionName);
                self.u_DataFanYinAdd.SetValue(elementReactionConfig.BaseScore);
                self.u_DataFanYinMult.SetValue(elementReactionConfig.Ride);
            }
            else
            {
                Log.Info("=== UI数据赋值 - 无反应配置，使用默认值 ===");
                // 如果没有反应配置，使用默认值
                self.u_DataFanYing.SetValue("");
                self.u_DataFanYinAdd.SetValue(0);
                self.u_DataFanYinMult.SetValue(0f);
            }



            //在这里发送一个事件给功能栏 功能栏来判断是否有加成 加成结束后 在回来给我一个事件显示相应数据
            await self.DynamicEvent(new EventFunctionCalReFresh()
            {
                Huo = HuoValue,
                Shui = ShuiValue,
                Bing = BingValue,
                Lei = LeiValue,
                Feng = FengValue,
            });

            await ETTask.CompletedTask;
        }


        //法宝预览属性刷新
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, EventScoreTotalReFresh message)
        {

            await ETTask.CompletedTask;
        }

        //最终结算 功能计算
        [EntitySystem]
        private static async ETTask DynamicEvent(this ForgeItemViewComponent self, EventScoreReFresh message)
        {
            using var list = ListComponent<EntityRef<Item>>.Create();
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.ForgeReady, list);

            // 遍历字典中的物品，累加五行属性值
            foreach (Item itemRef in list)
            {

                if (itemRef == null) continue;

                //这里的itemRef.config.BaseScore是物品的基础分数 + 功能装备提供的加成
                self.ScoreTotal += (itemRef.config.BaseScore + message.ScoreAdd);

            }

            //功能台那边已经算好了 直接用
            self.ScoreMult += message.ScoreMult;

            //总分
            //self.u_DataScoreBase.SetValue(self.ScoreTotal);
            self.u_DataScoreMult.SetValue(self.ScoreMult);
            self.u_DataScoreTotal.SetValue(self.ScoreTotal);
        }


        #endregion



        //刷新物品界面
        public static void RefreshItem(this ForgeItemViewComponent self, int type)
        {
            switch (type)
            {
                case 3:
                    self.ComForgeScrollRect.SetDataRefresh(self.m_ForgeDataList).NoContext();
                    break;
                case 4:
                    self.ComForgeReadyScrollRect.SetDataRefresh(self.m_ForgeReadyDataList).NoContext();
                    break;
            }
        }

        [EntitySystem]
        private static void YIUILoopRenderer(this ForgeItemViewComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            //ui界面的数据渲染
            item.ResetItem(data);

            item.SelectItem(select);
            if (select)
            {
                Debug.Log("锻造完成物品：点击了第" + index + " 个格子");
            }
        }

        [EntitySystem]
        private static void YIUILoopOnClick(this ForgeItemViewComponent self, QuickItemPrefabComponent item, EntityRef<Item> data, int index, bool select)
        {
            item.SelectItem(select, 2, data);
        }
    }
}