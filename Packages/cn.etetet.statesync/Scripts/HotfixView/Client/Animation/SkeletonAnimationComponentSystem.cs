using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;


namespace ET.Client
{

    [EntitySystemOf(typeof(SkeletonAnimationComponent))]
    [FriendOf(typeof(SkeletonAnimationComponent))]
    [FriendOfAttribute(typeof(ET.AnimationFrameEventInfo))]
    [FriendOfAttribute(typeof(ET.PlayerBehaviourComponent))]
    [FriendOfAttribute(typeof(ET.Client.AttackComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    public static partial class SkeletonAnimationComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.SkeletonAnimationComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            GameObject gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
            self.SkeletonAnimation = gameObject.GetComponentInChildren<SkeletonAnimation>();
            if (self.SkeletonAnimation == null)
            {
                Debug.Log("没有找到动画机");
            }
            else
            {
                Debug.Log("找到动画机");
            }

            // 防止重复注册事件监听器（参考AI组件实现）
            if (!self.IsEventListenerRegistered)
            {
                self.SkeletonAnimation.AnimationState.Event += (trackEntry, e) =>
                {
                    Debug.Log($"玩家动画事件: {e.Data.Name}, 时间: {e.Time}");

                    // 事件去重机制：防止短时间内重复处理同一事件
                    string eventKey = $"{unit.Id}_{e.Data.Name}"; // 使用玩家ID+事件名作为防抖键
                    float currentTime = UnityEngine.Time.time; // 使用Unity时间

                    Debug.Log($"[玩家防抖检查] 事件: {eventKey}, 当前时间: {currentTime:F3}");

                    if (self.LastEventTriggerTime.ContainsKey(eventKey))
                    {
                        float timeDiff = currentTime - self.LastEventTriggerTime[eventKey];
                        Debug.Log($"[玩家防抖检查] 上次触发时间: {self.LastEventTriggerTime[eventKey]:F3}, 时间间隔: {timeDiff:F3}秒");

                        if (timeDiff < SkeletonAnimationComponent.EVENT_DEBOUNCE_INTERVAL)
                        {
                            Debug.Log($"[玩家防抖过滤] 事件 {eventKey} 被过滤，时间间隔: {timeDiff:F3}秒 < {SkeletonAnimationComponent.EVENT_DEBOUNCE_INTERVAL}秒");
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log($"[玩家防抖检查] 事件 {eventKey} 首次触发");
                    }

                    self.LastEventTriggerTime[eventKey] = currentTime;
                    Debug.Log($"[玩家防抖更新] 事件 {eventKey} 时间更新为: {currentTime:F3}");

                    // 处理不同的动画事件
                    switch (e.Data.Name)
                    {
                        case "zhaojia":
                            Debug.Log("玩家招架事件触发");
                            break;
                        case "attack":
                            Debug.Log("玩家攻击事件触发 - 开始伤害判定");
                            self.HandlePlayerAttack(unit);
                            break;
                    }
                };

                self.IsEventListenerRegistered = true;
                Debug.Log($"玩家[{unit.Id}]事件监听器注册完成");
            }
            else
            {
                Debug.Log($"玩家[{unit.Id}]事件监听器已注册，跳过重复注册");
            }

            // 添加事件桥接组件
            var bridge = gameObject.GetComponent<SkeletonAnimationEventBridge>();
            if (bridge == null)
            {
                bridge = gameObject.AddComponent<SkeletonAnimationEventBridge>();
            }

            self.EventBridge = bridge;

            self.IsAnimation = false;
            self.Play(SkeletonAnimationType.front_idle, true);

            UnitConfig aiConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);


            self.AttackEvent(0.82f);
            self.PickEvent(0.95f);
            self.ZhaojiaEvent(0.95f);
        }
        // 静态HandleEvent方法已删除，改为在Awake中直接注册lambda事件监听器
        [EntitySystem]
        private static void Update(this ET.SkeletonAnimationComponent self)
        {
            // CheckFrameEvents(self);
            // if (self.IsLoop)
            // {
            //     // Debug.Log("动画机1");
            //     return;
            // }
            //
            // if (!self.IsAnimation)
            // {
            //     //  Debug.Log("动画机2");
            //     return;
            // }
            //
            // self.CurrentTime += Time.deltaTime;
            // if (self.CurrentTime < self.CurrentTrackEntry.AnimationEnd)
            // {
            //     // Debug.Log("动画机3");
            //     return;
            // }
            //
            // self.IsAnimation = false;
            //self.Play(SkeletonAnimationType.front_idle, true);
        }

        [EntitySystem]
        private static void Destroy(this ET.SkeletonAnimationComponent self)
        {
            // 清除事件回调
            if (self.EventBridge != null)
            {
                self.EventBridge.ClearEvents();
            }

            // 清除帧事件记录
            self.FrameEvents.Clear();

            // 清除事件去重记录（参考AI组件）
            self.LastEventTriggerTime.Clear();

            // 重置事件监听器注册标志
            self.IsEventListenerRegistered = false;
        }


        private static void CheckFrameEvents(this SkeletonAnimationComponent self)
        {
            // 如果没有帧事件或SkeletonAnimation为空，直接返回
            if (self.FrameEvents.Count == 0 || self.SkeletonAnimation == null || self.EventBridge == null)
                return;

            // 获取当前播放的动画
            var track = self.SkeletonAnimation.AnimationState.GetCurrent(0);
            if (track == null || track.Animation == null)
                return;

            string animName = track.Animation.Name;
            float currentTime = track.TrackTime;

            // 遍历所有帧事件，检查是否需要触发
            foreach (AnimationFrameEventInfo eventInfo in self.FrameEvents.Values)
            {
                // 如果不是当前动画，重置触发状态
                if (eventInfo.AnimationName != animName)
                {
                    eventInfo.HasTriggered = false;
                    eventInfo.LastTrackTime = -1;
                    continue;
                }

                // 如果动画重新开始播放，重置触发状态
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
                        Debug.Log($"触发帧事件: {eventInfo.EventId}, 动画={animName}, 时间={currentTime:F2}秒, 目标时间={eventInfo.TimePoint:F2}秒");
                        self.EventBridge.HandleEvent(eventInfo.EventId);
                        eventInfo.HasTriggered = true;
                    }
                }

                // 更新上一帧时间
                eventInfo.LastTrackTime = currentTime;
            }
        }


        public static bool Play(this SkeletonAnimationComponent self, SkeletonAnimationType type, bool loop)
        {
            if (self.CurrentTrackEntry != null && self.CurrentTrackEntry.Animation.Name.Equals(type.ToString()))
            {
                //Debug.Log("动画机状态： 失败");
                return false;
            }
            //Debug.Log("动画机状态： " + type.ToString() + "是否循环：" + self.IsLoop + "loop:" + loop);
            self.IsLoop = loop;
            self.CurrentTrackEntry = self.SkeletonAnimation.state.SetAnimation(0, type.ToString(), loop);
            self.IsAnimation = true;
            self.CurrentTime = 0;
            self.SkeletonAnimationType = type;
            self.AnimationEnd = self.CurrentTrackEntry.AnimationEnd;
            self.ChangeAnimationSpeed();
            return true;
        }

        //get animation lendgth frame
        public static int GetAnimationMaxLengthFrame(this SkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType)
        {
            var anim = self.SkeletonAnimation.skeleton.Data.FindAnimation(skeletonAnimationType.ToString());
            return (int)(anim.Duration * 60);
        }

        //get time length
        public static float GetTimeLength(this SkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType)
        {
            var anim = self.SkeletonAnimation.skeleton.Data.FindAnimation(skeletonAnimationType.ToString());
            return anim.Duration;
        }

        public static void AddFrameEvent(this SkeletonAnimationComponent self, SkeletonAnimationType skeletonAnimationType, float frame,
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
                Debug.LogError($"找不到动画: {skeletonAnimationType}");
                return;
            }

            // 将帧数转换为时间
            float fps = 60; // 默认帧率
            float time = frame / fps;

            // 检查时间是否在动画长度内
            if (time < 0 || time > anim.Duration)
            {
                Debug.LogError($"事件时间 {time} 超出动画 {skeletonAnimationType} 的长度 {anim.Duration}");
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

        public static void ChangeSkin(this SkeletonAnimationComponent self, SkeletonAnimationSkinLevelDarkHero skinLevel)
        {
            self.SkeletonAnimation.skeleton.SetSkin(skinLevel.ToString());
            self.SkeletonAnimation.skeleton.SetToSetupPose();
            self.Play(SkeletonAnimationType.front_idle, true);
        }

        public static void ChangeSkin(this SkeletonAnimationComponent self, SkeletonAnimationSkinLevel skinLevel)
        {
            self.SkeletonAnimation.skeleton.SetSkin(skinLevel.ToString());
            self.SkeletonAnimation.skeleton.SetToSetupPose();
            self.Play(SkeletonAnimationType.front_idle, true);
        }

        // 处理玩家攻击事件的实例方法（参考AI组件的HandleNormalAttack）
        public static void HandlePlayerAttack(this SkeletonAnimationComponent self, Unit playerUnit)
        {
            Debug.Log($"玩家 {playerUnit.Id} 执行攻击判定");

            // 获取玩家的攻击组件
            AttackComponent attackComponent = playerUnit.GetComponent<AttackComponent>();
            if (attackComponent == null)
            {
                Debug.LogWarning("玩家没有攻击组件");
                return;
            }

            // 获取当前场景（使用正确的ET框架API）
            Scene currentScene = self.Root().CurrentScene();
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();

            // 获取玩家GameObject
            GameObject playerGameObject = playerUnit.GetComponent<GameObjectComponent>().GameObject;

            // 检测攻击范围内的怪物
            Transform attackTransform = playerGameObject.transform.Find("AttackCollidler");
            if (attackTransform == null)
            {
                Debug.LogWarning("找不到AttackCollidler攻击碰撞器");
                return;
            }

            Collider attackCollider = attackTransform.GetComponent<Collider>();
            if (attackCollider == null)
            {
                Debug.LogWarning("AttackCollidler没有Collider组件");
                return;
            }

            // 获取攻击范围内的所有碰撞体
            Collider[] colliders = Physics.OverlapBox(
                attackCollider.bounds.center,
                attackCollider.bounds.extents,
                attackTransform.rotation
            );

            Debug.Log($"攻击范围内检测到 {colliders.Length} 个碰撞体");

            // 遍历所有碰撞体，寻找怪物并造成伤害
            foreach (var collider in colliders)
            {
                GameObjectEntityRef unitMonoBehaviour = collider.GetComponent<GameObjectEntityRef>();
                if (unitMonoBehaviour == null)
                    continue;
                Unit unit = unitMonoBehaviour.Entity as Unit;
                if (unit == null)
                    continue;
                if (unit.UnitType != UnitType.Monster)
                    continue;
                if (!unit.IsAlive())
                    continue;
                // 直接调用现有的攻击处理逻辑
                attackComponent.HandlePlayerAttack(unit);
                Debug.Log($"玩家攻击命中怪物: {unit.Id}");
    
                // 激活怪物仇恨（如果有敌人行为组件）
                // EnemyBehaviourComponent enemyBehaviourComponent = unit.GetComponent<EnemyBehaviourComponent>();
                // if (enemyBehaviourComponent != null)
                //     enemyBehaviourComponent.ChangeAngry();
                // if (collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
                // {
                //     UnitBehaviour enemyBehaviour = collider.transform.parent?.GetComponent<UnitBehaviour>();
                //     long enemyUnitId = enemyBehaviour?.unitId ?? 0;
                //     
                //     if (enemyUnitId != 0)
                //     {
                //         Unit monsterUnit = unitComponent.Get(enemyUnitId);
                //         if (monsterUnit != null && monsterUnit.IsAlive())
                //         {
                //             // 直接调用现有的攻击处理逻辑
                //             attackComponent.HandlePlayerAttack(monsterUnit);
                //             Debug.Log($"玩家攻击命中怪物: {monsterUnit.Id}");
                //             
                //             // 激活怪物仇恨（如果有敌人行为组件）
                //             EnemyBehaviourComponent enemyBehaviourComponent = monsterUnit.GetComponent<EnemyBehaviourComponent>();
                //             if (enemyBehaviourComponent != null)
                //             {
                //                 enemyBehaviourComponent.ChangeAngry();
                //             }
                //         }
                //     }
            // }
        }
    }

    //播放速度 加攻速什么的
    public static void ChangeAnimationSpeed(this SkeletonAnimationComponent self)
        {
            var unit = self.GetParent<Unit>();
            var numericComponent = unit.GetComponent<NumericComponent>();
            switch (self.SkeletonAnimationType)
            {
                case SkeletonAnimationType.front_walk:
                    self.SkeletonAnimation.timeScale = 1;//numericComponent.GetAsFloat(NumericType.Speed);
                    break;
                case SkeletonAnimationType.front_attack1:
                    self.SkeletonAnimation.timeScale = 1;// numericComponent.GetAsFloat(NumericType.AttackSpeed);
                    break;
                default:
                    self.SkeletonAnimation.timeScale = 1;
                    break;

            }
        }


        //攻击事件 添加
        public static void AttackEvent(this SkeletonAnimationComponent self, float time)
        {
            Unit unit = self.GetParent<Unit>();
            PlayerBehaviourComponent PlayerBehaviour = unit.GetComponent<PlayerBehaviourComponent>();
            AttackComponent attackComponent = unit.GetComponent<AttackComponent>();

            if (PlayerBehaviour == null)
            {
                Debug.Log("玩家动画 攻击！ 找不到player");
            }
            else
            {
                Debug.Log("玩家动画 攻击！ 找到player ");
            }

            self.AddFrameEvent(SkeletonAnimationType.front_attack1, time * 60, () =>
            {
                Debug.Log("玩家动画 front_attack 攻击动画结束！");
                PlayerBehaviour.StartMove();
            });


            self.AddFrameEvent(SkeletonAnimationType.left_attack1, time * 60, () =>
            {
                Debug.Log("玩家动画 left_attack 攻击动画结束！");
                PlayerBehaviour.StartMove();
            });

            self.AddFrameEvent(SkeletonAnimationType.right_attack1, time * 60, () =>
            {
                Debug.Log("玩家动画 right_attack 攻击动画结束！");
                PlayerBehaviour.StartMove();
            });

            self.AddFrameEvent(SkeletonAnimationType.behind_attack1, time * 60, () =>
            {
                Debug.Log("玩家动画 behind_attack 攻击动画结束！");
                PlayerBehaviour.StartMove();
            });

        }
        //拾取事件 添加
        public static void PickEvent(this SkeletonAnimationComponent self, float time)
        {
            Unit unit = self.GetParent<Unit>();
            PlayerBehaviourComponent PlayerBehaviour = unit.GetComponent<PlayerBehaviourComponent>();
            AttackComponent attackComponent = unit.GetComponent<AttackComponent>();

            if (PlayerBehaviour == null)
            {
                Debug.Log("玩家动画 攻击！ 找不到player");
            }
            else
            {
                Debug.Log("玩家动画 攻击！ 找到player ");
            }

            self.AddFrameEvent(SkeletonAnimationType.front_pick_short, time * 60, () =>
            {
                Debug.Log("玩家动画 front_pick_short 攻击动画结束！");
                PlayerBehaviour.StartMove();
                attackComponent.PickStateChange(1);
            });
        }
        
        //拾取事件 添加
        public static void ZhaojiaEvent(this SkeletonAnimationComponent self, float time)
        {
            Unit unit = self.GetParent<Unit>();
            PlayerBehaviourComponent PlayerBehaviour = unit.GetComponent<PlayerBehaviourComponent>();
            AttackComponent attackComponent = unit.GetComponent<AttackComponent>();

            if (PlayerBehaviour == null)
            {
                Debug.Log("玩家动画 招架！ 找不到player");
            }
            else
            {
                Debug.Log("玩家动画 招架！ 找到player ");
            }

            self.AddFrameEvent(SkeletonAnimationType.front_zhaojia, time * 60, () =>
            {
                Debug.Log("玩家动画 招架触发1！");
                PlayerBehaviour.StartMove();
                //attackComponent.PickState = 0;
                //attackComponent.PickStateChange(1);
                //attackComponent.changeAttackFlag(0); //攻击结束
                //attackComponent.Attack();
            });
        }
    }
}
