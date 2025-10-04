using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using ET.Client;
using UnityEngine;

namespace ET
{
    [FriendOfAttribute(typeof(ET.AISkeletonAnimationComponent))]
    [ComponentOf(typeof(Unit))]
    public class AISkeletonAnimationComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public SkeletonAnimation SkeletonAnimation;
        
        public EntityRef<MonsterAttackComponent> monsterAttackComponent;
        
        public float CurrentTime;
        public bool IsAnimation;
        public bool IsLoop;
        public float AnimationEnd;
        public TrackEntry CurrentTrackEntry;
        public SkeletonAnimationType SkeletonAnimationType = SkeletonAnimationType.front_idle;
        
        // 事件桥接器
        public SkeletonAnimationEventBridge EventBridge { get; set; }
        
        // 动画帧事件监控相关字段
        public Dictionary<string, EntityRef<AnimationFrameEventInfo>> FrameEvents = new Dictionary<string, EntityRef<AnimationFrameEventInfo>>();
        
        // 事件去重相关字段
        public Dictionary<string, float> LastEventTriggerTime = new Dictionary<string, float>();
        public const float EVENT_DEBOUNCE_INTERVAL = 0.3f; // 0.3秒防抖间隔，防止攻击动画重复触发
        
        // 防止重复注册事件监听器
        public bool IsEventListenerRegistered = false;
    }
    
   
  
}