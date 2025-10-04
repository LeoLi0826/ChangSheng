namespace ET.Client
{
    [Event(SceneType.StateSync)]
    public class StartGame_HideUI: AEvent<Scene, StartGame>
    {
        protected override async ETTask Run(Scene scene, StartGame a)
        {
            await scene.YIUIMgr().ClosePanelAsync<LoginPanelComponent>();
        }
    }
}

