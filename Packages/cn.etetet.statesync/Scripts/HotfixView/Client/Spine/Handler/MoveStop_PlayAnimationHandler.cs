namespace ET.Client
{
    /// <summary>
    /// 处理移动停止事件，更新动画状态
    /// </summary>
    [Event(SceneType.Current)]
    public class MoveStop_PlayAnimationHandler : AEvent<Scene,MoveStop>
    {
        protected override async ETTask Run(Scene scene, MoveStop a)
        {
            Unit unit = a.Unit;
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            var animationComponent = unit.GetComponent<UnitAnimationComponent>();
            animationComponent?.RequestBaseAnimation("idle");
            await ETTask.CompletedTask;
        }
    }
}
