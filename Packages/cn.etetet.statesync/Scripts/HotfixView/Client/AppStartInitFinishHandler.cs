namespace ET.Client
{
    [Event(SceneType.StateSync)]
    public class AppStartInitFinishHandler: AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish a)
        {
            await root.YIUIRoot().OpenPanelAsync<LoginPanelComponent>();
        }
    }
}

