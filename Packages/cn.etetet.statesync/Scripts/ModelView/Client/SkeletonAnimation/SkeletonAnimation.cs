using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [FriendOfAttribute(typeof(ET.SkeletonAnimationComponent))]
    [ComponentOf(typeof(Unit))]
    public class SkeletonAnimationComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public SkeletonAnimation SkeletonAnimation;
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
        
        // 事件去重和防抖相关字段（参考AI组件）
        public Dictionary<string, float> LastEventTriggerTime = new Dictionary<string, float>();
        public bool IsEventListenerRegistered = false;
        
        // 事件防抖间隔常量
        public const float EVENT_DEBOUNCE_INTERVAL = 0.5f;
    }
    public enum SkeletonAnimationType
    {
        None ,
        // 正面动画
        front_idle,    // 正面待机
        front_walk ,    // 正面行走
        front_attack,  // 正面攻击
        front_attack1,  // 正面攻击
        front_die,     // 正面死亡
        front_pick_long, //正面捡东西 长
        front_pick_short, //正面捡东西 短
        front_zhaojia, //招架
        front_skill, //正面技能
        xuanfeng_1, //正面蓄力攻击1
        xuanfeng_2, //正面蓄力攻击2
        xuanfeng_3, //正面蓄力攻击3
        xuli_1,
        xuli_2,
        xuli_3,
        
        // 背面动画         
        behind_idle,    // 背面待机
        behind_walk ,   // 背面行走
        behind_attack,  // 背面攻击
        behind_attack1,  // 背面攻击
        
        // 左侧动画
        left_idle,      // 左侧待机
        left_walk ,     // 左侧行走
        left_attack ,   // 左侧攻击
        left_attack1 ,   // 左侧攻击
        
        // 右侧动画
        right_idle,     //右侧待机
        right_walk ,    // 右侧行走
        right_attack ,  // 右侧攻击
        right_attack1 ,  // 右侧攻击

        front_weak,
        
        
    }
    public enum SkeletonAnimationSkinLevel
    {
        p1,
        p2,
        p3,
        p4,
        p5,
        p6,
    }
    public enum SkeletonAnimationSkinLevelDarkHero
    {
        e1,
        e2,
        e3,
        e4,
        e5,
        e6,
    }
}