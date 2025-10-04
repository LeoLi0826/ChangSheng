using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [EnableClass]
    // 骨骼动画事件桥接器
    public class SkeletonAnimationEventBridge : MonoBehaviour
    {
        // 事件回调字典
        public Dictionary<string, Action> eventCallbacks = new Dictionary<string, Action>();

        // 注册事件
        public void RegisterEvent(string eventId, Action callback)
        {
            eventCallbacks[eventId] = callback;
        }

        // 处理事件
        public void HandleEvent(string eventId)
        {
            if (eventCallbacks.TryGetValue(eventId, out Action callback))
            {
                callback?.Invoke();
            }
        }

        // 清除所有事件
        public void ClearEvents()
        {
            eventCallbacks.Clear();
        }
    }
}