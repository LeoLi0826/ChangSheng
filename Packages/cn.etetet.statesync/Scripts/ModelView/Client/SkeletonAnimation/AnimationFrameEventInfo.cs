using System;
using UnityEngine.PlayerLoop;

namespace ET
{
    [ChildOf]
    public class AnimationFrameEventInfo : Entity, IAwake, IDestroy
    {
        public string AnimationName;   // 动画名称
        public float Frame;            // 帧位置
        public float TimePoint;        // 对应的时间点
        public string EventId;         // 事件ID
        public bool HasTriggered;      // 是否已触发
        public float LastTrackTime = -1; // 上一帧的时间，用于检测时间点经过
    }
}