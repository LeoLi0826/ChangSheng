namespace ET.Client
{
    public static class AStarHelper 
    {
        public static async ETTask ScanAsync()
        {
            ETTask tcs = ETTask.Create();
        
            if (AstarPath.active != null)
            {
                AstarPath.active.Scan();
                tcs.SetResult();
            }
            else
            {
                tcs.SetResult();
            }
            await tcs;
        }
    }
}