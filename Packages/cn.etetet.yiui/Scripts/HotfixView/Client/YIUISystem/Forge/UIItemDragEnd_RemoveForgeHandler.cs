namespace ET.Client
{
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.QuickItemPrefabComponent))]
    [FriendOfAttribute(typeof(ET.Client.ForgeItemPrefabComponent))]
    public class UIItemDragEnd_RemoveForgeHandler : AEvent<Scene, UIItemDragEnd>
    {
        protected override async ETTask Run(Scene scene, UIItemDragEnd a)
        {
            Entity targetEntity = a.Target;
            Entity currentEntity = a.Current;
            if (targetEntity is QuickItemPrefabComponent targetView && currentEntity is ForgeItemPrefabComponent currentView)
            {
                //这里就能确定是哪个移动到哪个了
                Item item = currentView.ItemDataInfo;
                if (item == null)
                {
                    return;
                }
                //移动逻辑
                ItemContainerComponent itemContainerComponent = scene.Root().GetComponent<ItemContainerComponent>();
                ItemContainerHelper.MoveItem(itemContainerComponent, item, ItemContainerType.Backpack);
                //发送事件
                scene.DynamicEvent(new EventForgeItemReFresh()).NoContext();
                scene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
            }

            await ETTask.CompletedTask;
        }
    }
}

