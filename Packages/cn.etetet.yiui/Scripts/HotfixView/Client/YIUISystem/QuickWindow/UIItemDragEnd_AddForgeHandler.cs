namespace ET.Client
{
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.QuickItemPrefabComponent))]
    public class UIItemDragEnd_AddFunctionHandler : AEvent<Scene, UIItemDragEnd>
    {
        protected override async ETTask Run(Scene scene, UIItemDragEnd a)
        {
            Entity targetEntity = a.Target;
            Entity currentEntity = a.Current;
            if (targetEntity is QuickItemPrefabComponent targetView && currentEntity is QuickItemPrefabComponent currentView)
            {
                if (targetView.IsEquipBox && !currentView.IsEquipBox)
                {
                    //这里是普通背包移动到装备
                    Item item = currentView.ItemDataInfo;
                    if (item == null)
                    {
                        return;
                    }

                    if (item.config.Type != (int)ItemType.Weapon)
                    {
                        return;
                    }
                    ItemContainerComponent itemContainerComponent = scene.Root().GetComponent<ItemContainerComponent>();
                    ItemContainerHelper.MoveItem(itemContainerComponent, item, ItemContainerType.Fast);
                    scene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
                    scene.DynamicEvent(new EventQuickItemFunctionReFresh()).NoContext();
                    scene.DynamicEvent(new EventElementReFresh()).NoContext();
                    
                }
            }

            await ETTask.CompletedTask;
        }
    }
}

