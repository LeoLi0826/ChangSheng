using System.Collections.Generic;
using UnityEngine;
using YIUIFramework;

namespace ET.Client
{
    [GM(EGMType.Item, 3, "增加道具")]
    public class GM_AddItem : IGMCommand
    {
        public List<GMParamInfo> GetParams()
        {
            return new()
            {
                new GMParamInfo(EGMParamType.Int, "道具Id", "0"),
                new GMParamInfo(EGMParamType.Int, "增加数量", "0"),
            };
        }

        public async ETTask<bool> Run(Scene clientScene, ParamVo paramVo)
        {
            var configId = paramVo.Get<int>();
            var addNum = paramVo.Get<int>(1);
            ItemConfig itemConfig = ItemConfigCategory.Instance.Get(configId);
            ItemContainerType itemContainerType = ItemContainerType.Backpack;
            if (itemConfig == null)
            {
                await TipsHelper.Open<TipsTextViewComponent>(clientScene,"道具配置不存在");
                return false;
            }

            switch ((ItemType)itemConfig.Type)
            {
                case ItemType.Weapon:
                    addNum = 1;
                    break;
            }
            ItemContainerComponent itemContainerComponent = clientScene.GetComponent<ItemContainerComponent>();
            ItemContainerHelper.AddItem(itemContainerComponent, itemContainerType, configId,addNum);
            await TipsHelper.Open<TipsTextViewComponent>(clientScene,"道具增加成功");
            clientScene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
            clientScene.DynamicEvent(new EventQuickItemFunctionReFresh()).NoContext();
            return true;
        }
    }
    
    [GM(EGMType.Item, 3, "移除道具")]
    public class GM_RemoveItem : IGMCommand
    {
        public List<GMParamInfo> GetParams()
        {
            return new()
            {
                new GMParamInfo(EGMParamType.Int, "道具Id", "0"),
                new GMParamInfo(EGMParamType.Int, "移除数量", "0"),
            };
        }

        public async ETTask<bool> Run(Scene clientScene, ParamVo paramVo)
        {
            var configId = paramVo.Get<int>();
            var addNum = paramVo.Get<int>(1);
            ItemConfig itemConfig = ItemConfigCategory.Instance.Get(configId);
            if (itemConfig == null)
            {
                await TipsHelper.Open<TipsTextViewComponent>(clientScene,"道具配置不存在");
                return false;
            }

            ItemContainerType itemContainerType = ItemContainerType.Backpack;
            switch ((ItemType)itemConfig.Type)
            {
                case ItemType.Weapon:
                    addNum = 1;
                    break;
            }
            ItemContainerComponent itemContainerComponent = clientScene.GetComponent<ItemContainerComponent>();
            ItemContainerHelper.RemoveItem(itemContainerComponent, itemContainerType, configId,addNum);
            await TipsHelper.Open<TipsTextViewComponent>(clientScene,"道具删除成功");
            clientScene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
            clientScene.DynamicEvent(new EventQuickItemFunctionReFresh()).NoContext();
            return true;
        }
    }
}