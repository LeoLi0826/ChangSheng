namespace ET.Client
{
    public static partial class DataRefeshHelper
    {
        public static async ETTask DataRefesh(Scene root)
        {
            Log.Debug("我是DataRefeshHelper 运行");
            //刷新数据 通知所有ui界面的刷新
            await EventSystem.Instance.PublishAsync(root, new NumericReFresh());
             
        }
    }
}
