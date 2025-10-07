namespace ET.Client
{
    [EntitySystemOf(typeof(UnitAnimationComponent))]
    public static partial class UnitAnimationComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.UnitAnimationComponent self, UnitSpine args2,string args3)
        {
            self.m_unit = self.GetParent<Unit>();
            self.m_spineComponent = self.Unit.GetComponent<SpineComponent>();
            self.RequestedBaseAnimation = args3;
            self.UnitSpine = args2;
            self.ApplyBaseAnimation();
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Client.UnitAnimationComponent self)
        {
        }
        
        
        /// <summary>
        /// 播放高优先级的技能动画。
        /// </summary>
        /// <param name="animName">动画名称</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="onComplete">[可选] 动画播放完成时的回调。如果提供，则由调用方负责后续状态；否则，将自动恢复基础动画。</param>
        public static void PlaySkillAnimation(this ET.Client.UnitAnimationComponent self,string name, System.Action onComplete = null)
        {
            self.CurrentPriority = AnimationPriority.Skill;
            SpineAnimationData spineAnimationData = self.UnitSpine.GetAnimationData(name);
            self.SpineComponent.PlayAnimation(0, spineAnimationData.GetAnimationName((int)self.Unit.Direction),spineAnimationData.IsLoop, true, () =>
            {
                // 动画播放完成
                if (onComplete != null)
                {
                    // 如果有自定义回调，则执行它，并将动画控制权完全交还
                    onComplete.Invoke();
                }
                else
                {
                    // 否则，执行默认的恢复逻辑
                    self.CurrentPriority = AnimationPriority.Base;
                    self.RequestBaseAnimation(self.RequestedBaseAnimation);
                }
            });
        }

        /// <summary>
        /// 手动结束技能动画，恢复到基础动画状态
        /// </summary>
        public static void EndSkillAnimation(this ET.Client.UnitAnimationComponent self)
        {
            if (self.CurrentPriority == AnimationPriority.Skill)
            {
                self.CurrentPriority = AnimationPriority.Base;
                self.RequestBaseAnimation(self.RequestedBaseAnimation);
            }
        }


        /// <summary>
        /// 请求播放基础动画。实际播放会延迟到下一帧以防止动画闪烁。
        /// </summary>
        public static void RequestBaseAnimation(this ET.Client.UnitAnimationComponent self,string baseAnimationType)
        {
            self.RequestedBaseAnimation = baseAnimationType;
            self.QueueAnimationUpdate().NoContext();
        }

        private static async ETTask QueueAnimationUpdate(this ET.Client.UnitAnimationComponent self)
        {
            if (self.IsUpdateQueued) return;

            self.IsUpdateQueued = true;
            await self.Root().GetComponent<TimerComponent>()?.WaitFrameAsync();
            self.IsUpdateQueued = false;

            // 在下一帧，应用最终请求的动画
            self.ApplyBaseAnimation();
        }

        /// <summary>
        /// 检查优先级并播放最终请求的基础动画。
        /// </summary>
        private static void ApplyBaseAnimation(this ET.Client.UnitAnimationComponent self)
        {
            // 如果更高优先级的动画正在播放，则不执行任何操作
            if (self.CurrentPriority > AnimationPriority.Base)
            {
                return;
            }
            SpineAnimationData spineAnimationData = self.UnitSpine.GetAnimationData(self.RequestedBaseAnimation);
            // isForce为false，避免重复播放相同的动画
            self.SpineComponent.PlayAnimation(0, spineAnimationData.GetAnimationName((int)self.Unit.Direction),spineAnimationData.IsLoop, false);
        }
    }
}

