using System;

using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(AnimationFrameEventInfo))]
    [FriendOf(typeof(AnimationFrameEventInfo))]
    public static partial class AnimationFrameEventInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.AnimationFrameEventInfo self)
        {

        }
       
        public static void Initialize(this AnimationFrameEventInfo self, string animationName, float frame, float time, string eventId)
        {
            self.AnimationName = animationName;
            self.Frame = frame;
            self.TimePoint = time;
            self.EventId = eventId;
            self.HasTriggered = false;
            self.LastTrackTime = -1;
        }
        
        [EntitySystem]
        private static void Destroy(this ET.AnimationFrameEventInfo self)
        {
            self.AnimationName = null;
            self.Frame = 0;
            self.TimePoint = 0;
            self.EventId = null;
            self.HasTriggered = false;
            self.LastTrackTime = -1;
        }
    }
}