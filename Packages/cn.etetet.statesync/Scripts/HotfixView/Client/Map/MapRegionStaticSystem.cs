namespace ET.Client
{
    [EntitySystemOf(typeof(MapRegionStatic))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionStatic))]
    public static partial class MapRegionStaticSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MapRegionStatic self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Client.MapRegionStatic self)
        {
            if (self.GameObject == null) return;
            
            YIUIGameObjectPool.Inst.Put(self.GameObject);
        }
    }
}

