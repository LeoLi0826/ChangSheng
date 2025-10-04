using Spine;
using Spine.Unity;

namespace ET.Client
{
    [EntitySystemOf(typeof(SpineComponent))]
    public static partial class SpineComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.SpineComponent self, Spine.Unity.SkeletonAnimation args2)
        {
            self.SkeletonAnimation = args2;
        }
        [EntitySystem]
        private static void Destroy(this ET.Client.SpineComponent self)
        {
            self.StopAnimation();
            self.Animations.Clear();
        }
        
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trackIndex"></param>
        /// <param name="clipName"></param>
        /// <param name="isLoop"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        public static TrackEntry PlayAnimation(this ET.Client.SpineComponent self,int trackIndex, string clipName, bool isLoop, bool isForce, System.Action onComplete = null)
        {
            if (!isForce)
            {
                TrackEntry nowTrackEntry = self.SkeletonAnimation.AnimationState?.GetCurrent(trackIndex);
                if (nowTrackEntry != null && nowTrackEntry.Animation.Name == clipName)
                {
                    return nowTrackEntry;
                }
            }

            TrackEntry entry = null;
            if (self.Animations.TryGetValue(clipName, out Animation animation))
            {
                entry = self.SkeletonAnimation.AnimationState?.SetAnimation(trackIndex, animation, isLoop);
            }
            else
            {
                entry = self.SkeletonAnimation.AnimationState?.SetAnimation(trackIndex, clipName, isLoop);
                if (entry != null)
                {
                    self.Animations.Add(clipName, entry.Animation);
                }
            }

            if (entry != null && onComplete != null && !isLoop)
            {
                entry.Complete += OnAnimationComplete;
                void OnAnimationComplete(TrackEntry trackEntry)
                {
                    trackEntry.Complete -= OnAnimationComplete;
                    onComplete.Invoke();
                }
            }

            return entry;
        }

        /// <summary>
        /// 停止所有动画
        /// </summary>
        /// <param name="self"></param>
        public static void StopAnimation(this ET.Client.SpineComponent self)
        {
            if (self.SkeletonAnimation == null)
            {
                return;
            }

            self.SkeletonAnimation.AnimationState?.ClearTracks();
        }

        /// <summary>
        /// 停止单个轨道动画
        /// </summary>
        /// <param name="self"></param>
        /// <param name="trackIndex"></param>
        public static void StopAnimation(this ET.Client.SpineComponent self,int trackIndex)
        {
            if (self.SkeletonAnimation == null)
            {
                return;
            }

            self.SkeletonAnimation.AnimationState?.ClearTrack(trackIndex);
        }

        /// <summary>
        /// 设置速率
        /// </summary>
        /// <param name="self"></param>
        /// <param name="timeScale"></param>
        public static void SetAnimationTimeScale(this ET.Client.SpineComponent self,float timeScale)
        {
            if (self.SkeletonAnimation == null)
            {
                return;
            }

            self.SkeletonAnimation.AnimationState.TimeScale = timeScale;
        }
    }
}

