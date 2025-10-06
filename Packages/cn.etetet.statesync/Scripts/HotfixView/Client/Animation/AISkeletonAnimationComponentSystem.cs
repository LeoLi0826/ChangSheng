using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;


namespace ET.Client
{

    [EntitySystemOf(typeof(AISkeletonAnimationComponent))]
    [FriendOf(typeof(AISkeletonAnimationComponent))]
    [FriendOfAttribute(typeof(ET.AnimationFrameEventInfo))]
    [FriendOfAttribute(typeof(ET.PlayerBehaviourComponent))]
    [FriendOfAttribute(typeof(ET.Client.AttackComponent))]
    public static partial class AISkeletonAnimationComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.AISkeletonAnimationComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            GameObject gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
            self.SkeletonAnimation = gameObject.GetComponentInChildren<SkeletonAnimation>();
            // if (self.SkeletonAnimation == null)
            // {
            //     Debug.Log("敌人ui 没有找到怪物动画机");
            // }
            // else
            // {
            //     Debug.Log("敌人ui 找到怪物动画机");
            // }
            // 添加事件桥接组件
            // var bridge = gameObject.GetComponent<SkeletonAnimationEventBridge>();
            // if (bridge == null)
            // {
            //     bridge = gameObject.AddComponent<SkeletonAnimationEventBridge>();
            // }

            // 防止重复注册事件监听器
            // if (!self.IsEventListenerRegistered)
            // {
            //     self.SkeletonAnimation.AnimationState.Event += (trackEntry, e) =>
            //     {
            //         Debug.Log($"动画事件: {e.Data.Name}, 时间: {e.Time}");
            //
            //         // 事件去重机制：防止短时间内重复处理同一事件
            //         string eventKey = $"{unit.Id}_{e.Data.Name}"; // 使用怪物ID+事件名作为防抖键
            //         float currentTime = UnityEngine.Time.time; // 使用Unity时间
            //
            //         Debug.Log($"[防抖检查] 事件: {eventKey}, 当前时间: {currentTime:F3}");
            //
            //         if (self.LastEventTriggerTime.ContainsKey(eventKey))
            //         {
            //             float timeDiff = currentTime - self.LastEventTriggerTime[eventKey];
            //             Debug.Log($"[防抖检查] 上次触发时间: {self.LastEventTriggerTime[eventKey]:F3}, 时间间隔: {timeDiff:F3}秒");
            //
            //             if (timeDiff < AISkeletonAnimationComponent.EVENT_DEBOUNCE_INTERVAL)
            //             {
            //                 return;
            //             }
            //         }
            //
            //         self.LastEventTriggerTime[eventKey] = currentTime;
            //
            //         // 发布动画事件到事件系统，让攻击组件自己处理
            //         self.PublishAnimationEvent(unit, e.Data.Name, e.Time);
            //     };
            //
            //     self.IsEventListenerRegistered = true;
            //     Debug.Log($"怪物[{unit.Id}]事件监听器注册完成");
            // }
            // else
            // {
            //     Debug.Log($"怪物[{unit.Id}]事件监听器已注册，跳过重复注册");
            // }

            // 动画组件不再需要直接引用攻击组件
            Debug.Log($"AISkeletonAnimationComponent 初始化完成，Unit: {unit.Id}");

            // self.EventBridge = bridge;

            self.IsAnimation = false;
            self.Play(SkeletonAnimationType.front_idle, true);

            UnitConfig aiConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);

            // 禁用旧的帧事件攻击处理，改用"attack"动画事件处理
            // self.EnemyAttackEvent(0.72f);

        }



        [EntitySystem]
        private static void Update(this ET.AISkeletonAnimationComponent self)
        {
            // CheckFrameEvents(self);
            // if (self.IsLoop)
            // {
            //     return;
            // }
            //
            // if (!self.IsAnimation)
            // {
            //     return;
            // }
            //
            // self.CurrentTime += Time.deltaTime;
            // if (self.CurrentTime < self.CurrentTrackEntry.AnimationEnd)
            // {
            //     return;
            // }
            //
            // self.IsAnimation = false;
            // self.CurrentTrackEntry = null; // 动画结束时清空 CurrentTrackEntry
        }

        [EntitySystem]
        private static void Destroy(this ET.AISkeletonAnimationComponent self)
        {
            // 清除事件回调
            if (self.EventBridge != null)
            {
                self.EventBridge.ClearEvents();
            }

            // 清除帧事件记录
            self.FrameEvents.Clear();

            // 清除事件去重记录
            self.LastEventTriggerTime.Clear();
            
            // 重置事件监听器注册标志
            self.IsEventListenerRegistered = false;
        }


        private static void CheckFrameEvents(this AISkeletonAnimationComponent self)
        {
            if (self.FrameEvents.Count == 0 || self.SkeletonAnimation == null || self.EventBridge == null)
                return;

            var track = self.SkeletonAnimation.AnimationState.GetCurrent(0);
            if (track == null || track.Animation == null)
            {
                // 如果当前没有播放动画，清空 CurrentTrackEntry
                self.CurrentTrackEntry = null;
                return;
            }

            // 更新 CurrentTrackEntry
            self.CurrentTrackEntry = track;

            string animName = track.Animation.Name;
            float currentTime = track.TrackTime;

            // 遍历所有帧事件，检查是否需要触发
            foreach (AnimationFrameEventInfo eventInfo in self.FrameEvents.Values)
            {
                // 如果不是当前怪物动画，重置触发状态
                if (eventInfo.AnimationName != animName)
                {
                    eventInfo.HasTriggered = false;
                    eventInfo.LastTrackTime = -1;
                    continue;
                }

                // 如果怪物动画重新开始播放，重置触发状态
                if (eventInfo.LastTrackTime > currentTime)
                {
                    eventInfo.HasTriggered = false;
                }

                // 检查是否需要触发事件
                if (!eventInfo.HasTriggered && eventInfo.LastTrackTime >= 0)
                {
                    // 如果上一帧时间小于目标时间点，当前帧时间大于等于目标时间点，表示跨过了时间点
                    if ((eventInfo.LastTrackTime <= eventInfo.TimePoint && currentTime >= eventInfo.TimePoint) ||
                        Math.Abs(currentTime - eventInfo.TimePoint) < 0.05f)
                    {
                        // 触发事件
                        Debug.Log($"触发帧事件: {eventInfo.EventId}, 怪物动画={animName}, 时间={currentTime:F2}秒, 目标时间={eventInfo.TimePoint:F2}秒");
                        self.EventBridge.HandleEvent(eventInfo.EventId);
                        eventInfo.HasTriggered = true;
                    }
                }

                // 更新上一帧时间
                eventInfo.LastTrackTime = currentTime;
            }
        }


        public static bool Play(this AISkeletonAnimationComponent self, SkeletonAnimationType type, bool loop)
        {
            // 检查 SkeletonAnimation 是否为空
            if (self.SkeletonAnimation == null)
            {
                return false;
            }

            // 检查当前动画状态
            if (self.CurrentTrackEntry != null && self.CurrentTrackEntry.Animation != null)
            {
                if (self.CurrentTrackEntry.Animation.Name.Equals(type.ToString()))
                {
                    return false;
                }
            }

            // 清理旧的动画状态
            self.CurrentTrackEntry = null;
            self.IsAnimation = false;
            self.CurrentTime = 0;

            // 设置新的动画
            self.IsLoop = loop;
            self.CurrentTrackEntry = self.SkeletonAnimation.state.SetAnimation(0, type.ToString(), loop);

            // 检查新设置的动画是否成功
            if (self.CurrentTrackEntry == null)
            {
                return false;
            }
            self.IsAnimation = true;
            self.SkeletonAnimationType = type;
            self.AnimationEnd = self.CurrentTrackEntry.AnimationEnd;
            self.ChangeAnimationSpeed();
            return true;
        }

        //get animation lendgth frame
        public static int GetAnimationMaxLengthFrame(this AISkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType)
        {
            var anim = self.SkeletonAnimation.skeleton.Data.FindAnimation(skeletonAnimationType.ToString());
            return (int)(anim.Duration * 60);
        }

        //get time length
        public static float GetTimeLength(this AISkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType)
        {
            var anim = self.SkeletonAnimation.skeleton.Data.FindAnimation(skeletonAnimationType.ToString());
            return anim.Duration;
        }

        public static void AddFrameEvent(this AISkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType, float frame,
        Action callback)
        {
            if (self.SkeletonAnimation == null || self.EventBridge == null)
            {
                Debug.LogError("SkeletonAnimation或EventBridge为空，无法添加帧事件");
                return;
            }


            // 生成唯一事件ID
            string eventId = $"{skeletonAnimationType}_{frame}";

            // 注册事件回调
            self.EventBridge.RegisterEvent(eventId, callback);

            // 获取Animation对象
            var anim = self.SkeletonAnimation.skeleton.Data.FindAnimation(skeletonAnimationType.ToString());
            if (anim == null)
            {
                Debug.LogError($"找不到怪物动画: {skeletonAnimationType}");
                return;
            }
            // 将帧数转换为时间
            float fps = 60; // 默认帧率
            float time = frame / fps;

            // 检查时间是否在怪物动画长度内
            if (time < 0 || time > anim.Duration)
            {
                Debug.LogError($"事件时间 {time} 超出怪物动画 {skeletonAnimationType} 的长度 {anim.Duration}");
                return;
            }

            // 创建帧事件信息对象
            var eventInfo = self.AddChild<AnimationFrameEventInfo>();
            eventInfo.Initialize(skeletonAnimationType.ToString(), frame, time, eventId);

            // 添加到组件的事件字典中
            self.FrameEvents[eventId] = eventInfo;
            // 保留原始事件系统作为备用
            self.SkeletonAnimation.AnimationState.Event += (trackEntry, e) =>
            {
                // 输出调试信息
            };
        }

        public static void ChangeSkin(this AISkeletonAnimationComponent self, SkeletonAnimationSkinLevelDarkHero skinLevel)
        {
            self.SkeletonAnimation.skeleton.SetSkin(skinLevel.ToString());
            self.SkeletonAnimation.skeleton.SetToSetupPose();
            self.Play(SkeletonAnimationType.front_idle, true);
        }

        public static void ChangeSkin(this AISkeletonAnimationComponent self, SkeletonAnimationSkinLevel skinLevel)
        {
            self.SkeletonAnimation.skeleton.SetSkin(skinLevel.ToString());
            self.SkeletonAnimation.skeleton.SetToSetupPose();
            self.Play(SkeletonAnimationType.front_idle, true);
        }

        //播放速度 加攻速什么的
        public static void ChangeAnimationSpeed(this AISkeletonAnimationComponent self)
        {
            var unit = self.GetParent<Unit>();
            var numericComponent = unit.GetComponent<NumericComponent>();
            switch (self.SkeletonAnimationType)
            {
                case SkeletonAnimationType.front_walk:
                    self.SkeletonAnimation.timeScale = 1;//numericComponent.GetAsFloat(NumericType.Speed);
                    break;
                case SkeletonAnimationType.front_attack:
                    self.SkeletonAnimation.timeScale = 1;// numericComponent.GetAsFloat(NumericType.AttackSpeed);
                    break;
                default:
                    self.SkeletonAnimation.timeScale = 1;
                    break;

            }
        }

        // 发布动画事件到ET事件系统
        private static void PublishAnimationEvent(this AISkeletonAnimationComponent self, Unit unit, string eventName, float eventTime)
        {
            // 安全检查
            if (unit == null || unit.IsDisposed)
            {
                Debug.LogWarning($"Unit已销毁，跳过动画事件发布: {eventName}");
                return;
            }
            
            Scene root = unit.Root();
            if (root == null)
            {
                Debug.LogError($"无法获取Root Scene，跳过动画事件发布: {eventName}");
                return;
            }
            
            Debug.Log($"发布动画事件: {eventName}, Unit: {unit.Id}, 时间: {eventTime}");
            
            try
            {
                // 发布通用动画事件
                EventSystem.Instance.Publish(root, new AnimationEvent() 
                { 
                    Unit = unit, 
                    EventName = eventName, 
                    EventTime = eventTime 
                });
                
                // 发布具体的事件类型
                switch (eventName)
                {
                    case "attack_start":
                        EventSystem.Instance.Publish(root, new AttackStartEvent() { Unit = unit });
                        break;
                    case "attack_finish":
                        EventSystem.Instance.Publish(root, new AttackFinishEvent() { Unit = unit });
                        break;
                    case "attack":
                        EventSystem.Instance.Publish(root, new AttackEvent() { Unit = unit });
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"发布动画事件失败: {eventName}, 错误: {e.Message}");
            }
        }
    }
}
