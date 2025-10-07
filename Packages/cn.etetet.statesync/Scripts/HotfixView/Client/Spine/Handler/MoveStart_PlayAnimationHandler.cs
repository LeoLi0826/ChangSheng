namespace ET.Client
{
    /// <summary>
    /// 处理移动开始事件，更新动画状态
    /// </summary>
    [Event(SceneType.Current)]
    public class MoveStart_PlayAnimationHandler : AEvent<Scene,MoveStart>
    {
        protected override async ETTask Run(Scene scene, MoveStart a)
        {
            Unit unit = a.Unit;
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            var animationComponent = unit.GetComponent<UnitAnimationComponent>();
            animationComponent?.RequestBaseAnimation("walk");
            await ETTask.CompletedTask;
        }
    }
}
