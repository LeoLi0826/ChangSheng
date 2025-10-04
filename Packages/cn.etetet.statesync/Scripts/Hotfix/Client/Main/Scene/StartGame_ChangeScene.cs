namespace ET.Client
{
    
    [Event(SceneType.StateSync)]
    public class StartGame_ChangeScene: AEvent<Scene, StartGame>
    {
        protected override async ETTask Run(Scene scene, StartGame a)
        {
            SceneChangeHelper.SceneChangeTo(scene.Root(),"Map1").NoContext();
            await ETTask.CompletedTask;
        }
    }
}

