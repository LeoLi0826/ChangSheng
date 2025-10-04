namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinish_AddComponent: AEvent<Scene, SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, SceneChangeFinish a)
        {
            scene.AddComponent<OperaComponent>();
            await ETTask.CompletedTask;
        }
    }
}

