namespace ET.Client
{
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.QuickItemPrefabComponent))]
    public class UIItemDragEnd_AddForgeHandler : AEvent<Scene, UIItemDragEnd>
    {
        protected override async ETTask Run(Scene scene, UIItemDragEnd a)
        {
            Entity targetEntity = a.Target;
            Entity currentEntity = a.Current;
            if (targetEntity is ForgeItemViewComponent targetView && currentEntity is QuickItemPrefabComponent currentView)
            {
                //这里就能确定是哪个移动到哪个了
                Item item = currentView.ItemDataInfo;
                if (item == null)
                {
                    return;
                }
                //移动逻辑
                ItemContainerComponent itemContainerComponent = scene.Root().GetComponent<ItemContainerComponent>();
                ItemContainerHelper.MoveItem(itemContainerComponent, item, ItemContainerType.Forge);
                //发送事件
                scene.DynamicEvent(new EventForgeItemReFresh()).NoContext();
                scene.DynamicEvent(new EventQuickItemReFresh()).NoContext();
                // 刷新灵气值显示
                scene.DynamicEvent(new EventElementReFresh()).NoContext();
            }

            await ETTask.CompletedTask;
        }
    }
}

