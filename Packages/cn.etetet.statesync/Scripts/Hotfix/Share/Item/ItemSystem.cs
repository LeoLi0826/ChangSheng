namespace ET
{
    [EntitySystemOf(typeof(Item))]
    [FriendOfAttribute(typeof(ET.Item))]
    public static partial class ItemSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Item self, int args2)
        {
            self.ConfigId = args2;
        }
        [EntitySystem]
        private static void Destroy(this ET.Item self)
        {

        }
    }
}

