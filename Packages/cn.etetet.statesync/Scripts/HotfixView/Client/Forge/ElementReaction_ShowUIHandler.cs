namespace ET.Client
{
    [Event(SceneType.StateSync)]
    public class ElementReaction_ShowUIHandler : AEvent<Scene, ElementReaction>
    {
        protected override async ETTask Run(Scene scene, ElementReaction a)
        {
            await TipsHelper.Open<TipsTextViewComponent>(scene,$"{a.Name},值:{a.Value}");
        }
    }
}

