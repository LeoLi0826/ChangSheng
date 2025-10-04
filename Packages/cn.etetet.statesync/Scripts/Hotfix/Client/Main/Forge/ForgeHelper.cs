using System.Collections.Generic;
using System.Linq;
using YIUIFramework;

namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.ItemContainer))]
    [FriendOfAttribute(typeof(ET.Client.ForgeComponent))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static class ForgeHelper
    {

        // 合成台：宝成逻辑
        public static void ForgeItem(Scene scene)
        {
            ForgeComponent forgeComponent = scene.GetComponent<ForgeComponent>();
            if (forgeComponent == null)
            {
                return;
            }

            using var configs = ListComponent<TalismanConfig>.Create();
            using var weights = ListComponent<int>.Create();
            //得到可以合成的列表
            TalismanConfigCategory.Instance.GetSatisfyElements(forgeComponent.Element,configs,weights);
            if (configs.Count <= 0)
            {
                //没有可以合成的
                return;
            }

            int index = RandomGenerator.RandomWeight(weights);
            TalismanConfig config = configs[index];
            
            //这里可以弹框事件
            
            //将道具加入背包
            ItemContainerComponent itemContainerComponent = scene.GetComponent<ItemContainerComponent>();
            //直接将法宝放入背包
            ItemContainerHelper.AddItem(itemContainerComponent,ItemContainerType.Backpack, config.Id,1);
            // 清空最终灵气值，避免重复累加
            forgeComponent.Element.Clear();
            
            
            //最终法宝灵气值
            forgeComponent.FinalScore = 0;
            
            scene.DynamicEvent(new EventForgeReadyReFresh()).NoContext();
            scene.DynamicEvent(new EventElementReFresh()).NoContext();
            scene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
            // 刷新合成台物品显示
            scene.DynamicEvent(new EventForgeItemReFresh()).NoContext();
        }
        
        /// <summary>
        /// 执行合成 - 按照从左往右的顺序进行元素反应
        /// </summary>
        public static async ETTask Forge(Scene scene)
        {
            ForgeComponent forgeComponent = scene.GetComponent<ForgeComponent>();
            if (forgeComponent == null)
            {
                return;
            }

            if (forgeComponent.ForgeNum <= 0)
            {
                return;
            }

            ItemContainerComponent itemContainerComponent = scene.GetComponent<ItemContainerComponent>();
            //先找到合成台内的物品（从左往右排序）
            using var list = ListComponent<EntityRef<Item>>.Create();
            ItemContainerHelper.GetItems(itemContainerComponent, ItemContainerType.Forge, list);

            if (list.Count < 2)
            {
                return;
            }

            ItemContainer readyContainer = itemContainerComponent.GetContainer(ItemContainerType.ForgeReady);
            ItemContainerConfig readyConfig = ItemContainerConfigCategory.Instance.Get(ItemContainerType.ForgeReady);
            //判断最多还能合成多少个
            int maxNum = readyContainer.CurrentCellCount - readyContainer.Items.Count;

            if (maxNum <= 0)
            {
                return;
            }

            // 使用现有的字典，避免new
            Dictionary<ItemElementAttr, int> totalElements = new Dictionary<ItemElementAttr, int>();

            // 执行合成计算 - 按照您的公式和示例
            int finalScore = await ExecuteForgeProcess(list, totalElements,scene);

            Log.Info($"合成完成，最终灵气值: {finalScore}");

            //写入总属性到合成组件
            forgeComponent.AddItemElement(totalElements);
            
            // 注意：Score 现在由UI实时计算，不再在这里累加
            //直接移除
            using var forgeList = ListComponent<EntityRef<Item>>.Create();
            for (int i = 0; i < list.Count && i < maxNum; i++)
            {
                forgeList.Add(list[i]);
            }

            foreach (var entityRef in forgeList)
            {
                Item item = entityRef;
                ItemContainerHelper.RemoveItem(itemContainerComponent, item, item.Count);
            }

            //可以合成次数-1
            forgeComponent.FinalScore += finalScore;
            //forgeComponent.Score += finalScore;
            
            forgeComponent.ForgeNum -= 1;
            
            //刷新UI通知
            scene.DynamicEvent(new EventForgeItemReFresh()).NoContext();
            scene.DynamicEvent(new EventForgeReadyReFresh()).NoContext();
            
            //合成台界面属性刷新
            scene.DynamicEvent(new EventElementReFresh()).NoContext();
        }

        
        #region 灵气值计算
        private static async ETTask<int> ExecuteForgeProcess(ListComponent<EntityRef<Item>> items, Dictionary<ItemElementAttr, int> totalElements,Scene scene)
        {
            ForgeComponent forgeComponent = scene.GetComponent<ForgeComponent>();
            if (forgeComponent == null)
            {
                Log.Debug("没有找到合成组件");
                return 0;
            }
            
            if (items.Count < 2)
            {
                return 0;
            }

          
            //各个反应反应次数
            Dictionary<ElementReactionType, int> reactionData = new Dictionary<ElementReactionType, int>();
            
            //先拿到第一个
            Item oneItem = items[0];
            //第一个的属性
            
            ItemElementAttrData oneData = ItemConfigCategory.Instance.GetElement(oneItem.ConfigId);

            ItemConfig nowConfig = oneItem.config;
            ItemElementAttr nowAttr = oneData.ItemElementAttr;
            int nowValue = oneData.Value;
            
            //反应积分
            int finalScore = 0;
            (int treasureBaseAdd, int treasureMagnificationAdd, int treasureRideAdd) = GetTreasureForgeNum(scene);
           
            //从第二个开始逐个反应
            for (int i = 1; i < items.Count; i++)
            {
                Item item = items[i];
                ItemConfig itemConfig = item.config;
                ItemElementAttrData attrData = ItemConfigCategory.Instance.GetElement(item.ConfigId);
                if (nowValue <= 0)
                {
                    //如果当前属性已经没了
                    nowValue = attrData.Value;
                    nowAttr = attrData.ItemElementAttr;
                    continue;
                }
                //判断是否能反应
                ElementReactionConfig elementReactionConfig = ElementReactionConfigCategory.Instance.Get(nowAttr, attrData.ItemElementAttr);
                if (elementReactionConfig == null)
                {
                    //不反应
                    nowValue += attrData.Value;
                    continue;
                }

                int reactionNum = 0;
                
                //计算反应次数
                if (nowValue >= attrData.Value)
                {
                    //如果大于,属性不变
                    reactionNum = attrData.Value;
                    totalElements.AddNum(nowAttr,reactionNum);
                    totalElements.AddNum(attrData.ItemElementAttr,reactionNum);
                    nowValue -= attrData.Value;
                }
                else
                {
                    //小于，属性改变
                    reactionNum = nowValue;
                    totalElements.AddNum(nowAttr,reactionNum);
                    totalElements.AddNum(attrData.ItemElementAttr,reactionNum);
                    nowValue = attrData.Value - nowValue;
                    nowAttr = attrData.ItemElementAttr;
                }
                reactionData.AddNum(elementReactionConfig.ReactionType,reactionNum);
                
                //总公式：（物品1基础分+物品2基础分）+法宝提供的基础分+基础反应基础分）*（（基础倍乘+聚变反应倍乘+法宝倍乘）*反应次数*（特殊反应倍率+法宝倍率））
                //合成时最终 灵气值计算：（物品1基础分+物品2基础分）+法宝提供的基础分+基础反应基础分）
                int baseAdd = ((nowConfig.BaseScore + itemConfig.BaseScore) + treasureBaseAdd + elementReactionConfig.BaseScore);
                
                //合成时最终 基础倍乘计算:（基础倍乘+聚变反应倍乘+法宝倍乘）*反应次数
                int baseRide = ((1 + elementReactionConfig.Ride + treasureRideAdd) * reactionNum);
                
                //合成时最终 倍率计算 : 特殊反应倍率+法宝倍率
                float baseMultiplier = (1 + (treasureMagnificationAdd + elementReactionConfig.Multiplier) / 10000f);
                
                //最终灵气值结算
                int newFinalScore = (int)(baseAdd * baseRide * baseMultiplier);
                
                // 详细日志显示所有数据
                Log.Info($"=== 合成计算详细数据 ===");
                Log.Info($"物品1: {nowConfig.Name} (基础分: {nowConfig.BaseScore})");
                Log.Info($"物品2: {itemConfig.Name} (基础分: {itemConfig.BaseScore})");
                Log.Info($"反应配置: {elementReactionConfig.ReactionName}");
                Log.Info($"  反应基础分: {elementReactionConfig.BaseScore}");
                Log.Info($"  反应倍乘: {elementReactionConfig.Ride}");
                Log.Info($"  反应倍率: {elementReactionConfig.Multiplier}");
                Log.Info($"法宝加成:");
                Log.Info($"  法宝基础分: {treasureBaseAdd}");
                Log.Info($"  法宝倍乘: {treasureRideAdd}");
                Log.Info($"  法宝倍率: {treasureMagnificationAdd}");
                Log.Info($"反应次数: {reactionNum}");
                Log.Info($"计算过程:");
                Log.Info($"  基础分计算: ({nowConfig.BaseScore} + {itemConfig.BaseScore}) + {treasureBaseAdd} + {elementReactionConfig.BaseScore} = {baseAdd}");
                Log.Info($"  倍乘计算: (1 + {elementReactionConfig.Ride} + {treasureRideAdd}) * {reactionNum} = {baseRide}");
                Log.Info($"  倍率计算: 1 + ({treasureMagnificationAdd} + {elementReactionConfig.Multiplier}) / 10000 = {baseMultiplier:F4}");
                Log.Info($"最终灵气值: {baseAdd} * {baseRide} * {baseMultiplier:F4} = {newFinalScore}");
                Log.Info($"========================");
                finalScore += newFinalScore;
                
                //消息弹窗
                await EventSystem.Instance.PublishAsync(scene,new ElementReaction
                {
                    Name = elementReactionConfig.ReactionName,
                    Value = newFinalScore
                });

                //更新合成组件的数据 后面会刷新UI
                forgeComponent.ElementReactionConfig = elementReactionConfig;
                


            }
            return finalScore;
        }
        #endregion


        public static (int, int, int) GetTreasureForgeNum(Scene scene)
        {
            ItemContainerComponent itemContainerComponent = scene.GetComponent<ItemContainerComponent>();
            return GetTreasureForgeNum(itemContainerComponent);
        }
        
        public static (int, int, int) GetTreasureForgeNum(ItemContainerComponent itemContainerComponent)
        {
            //先找到合成台内的物品（从左往右排序）
            using var list = ListComponent<Item>.Create();
            ItemContainerHelper.GetItemsByType(itemContainerComponent,ItemContainerType.Fast,ItemType.Weapon,list);
            int treasureBaseAdd = 0;//法宝基础加成
            int treasureMagnificationAdd = 0;//法宝倍率加成
            int treasureRideAdd = 0;//法宝倍成加成

            foreach (var item in list)
            {
                TalismanConfig itemConfig = TalismanConfigCategory.Instance.Get(item.ConfigId);
                if (itemConfig == null)
                {
                    continue;
                }
                
                treasureBaseAdd += itemConfig.AddBuff;
                treasureRideAdd += itemConfig.TimeBuff;
                treasureMagnificationAdd += itemConfig.MultBuff;
            }
            return (treasureBaseAdd,treasureMagnificationAdd,treasureRideAdd);
        }
        
        public static void  AddNum(this Dictionary<ItemElementAttr, int> self,ItemElementAttr attr,int num)
        {
            if (self.TryGetValue(attr, out var old))
            {
                self[attr] = old + num;
            }
            else
            {
                self[attr] = num;
            }
        }
        
        public static void  AddNum(this Dictionary<ElementReactionType, int> self,ElementReactionType attr,int num)
        {
            if (self.TryGetValue(attr, out var old))
            {
                self[attr] = old + num;
            }
            else
            {
                self[attr] = num;
            }
        }
        
    }
}