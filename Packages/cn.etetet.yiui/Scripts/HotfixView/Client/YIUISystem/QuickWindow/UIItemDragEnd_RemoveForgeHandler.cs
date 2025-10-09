namespace ET.Client
{
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.QuickItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Client.ForgeItemPrefabComponent))]
    public class UIItemDragEnd_RemoveFunctionHandler : AEvent<Scene, UIItemDragEnd>
    {
        protected override async ETTask Run(Scene scene, UIItemDragEnd a)
        {
            Entity targetEntity = a.Target;
            Entity currentEntity = a.Current;
            if (targetEntity is QuickItemPrefabComponent targetView && currentEntity is QuickItemPrefabComponent currentView)
            {
                if (!targetView.IsEquipBox && currentView.IsEquipBox)
                {
                    //这里是装备移动到普通背包
                    Item item = currentView.ItemDataInfo;
                    if (item == null)
                    {
                        return;
                    }

                    ItemContainerComponent itemContainerComponent = scene.Root().GetComponent<ItemContainerComponent>();
                    ItemContainerHelper.MoveItem(itemContainerComponent, item, ItemContainerType.Backpack);
                    scene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
                    scene.DynamicEvent(new EventQuickItemFunctionReFresh()).NoContext();
                    scene.DynamicEvent(new EventElementReFresh()).NoContext();
                }
            }
            await ETTask.CompletedTask;
        }
    }
}

