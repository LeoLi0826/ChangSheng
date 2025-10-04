using System;
using UnityEngine;
using DG.Tweening;

namespace ET.Client
{
    [EntitySystemOf(typeof(HuangFengDaShengAttackComponent))]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    [FriendOfAttribute(typeof(ET.Client.LongJuanFengMoveComponent))]
    public static partial class HuangFengDaShengAttackComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.HuangFengDaShengAttackComponent self)
        {
            self.IsCd = false;
            self.IsNormalAttacking = false;
            self.IsChargeAttacking = false;
            self.IsUltimateAttacking = false;
            self.IsFinished = false;
            self.IsChargeAttackOnCd = false;
            self.HasActiveTornadoes = false;
            self.TornadoUnits?.Clear();

            // 订阅动画事件
            self.SubscribeAnimationEvents();
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.HuangFengDaShengAttackComponent self)
        {
            self.Target = null;
            TimerComponent timerComponent = self.Root().GetComponent<TimerComponent>();

            // 清理所有定时器
            timerComponent.Remove(ref self.AttackCdTimerId);
            timerComponent.Remove(ref self.ChargeAttackCdTimerId);
            timerComponent.Remove(ref self.ZhendaoWindowTimerId);
            timerComponent.Remove(ref self.UltimateAttackCdTimerId);

            // 清理龙卷风单位
            if (self.TornadoUnits != null)
            {
                foreach (var refUnit in self.TornadoUnits)
                {
                    Unit u = refUnit;
                    if (u != null && !u.IsDisposed)
                    {
                        u.Dispose();
                    }
                }
                self.TornadoUnits.Clear();
            }
            self.HasActiveTornadoes = false;

            // 回收沙尘暴效果
            self.RecycleSandstorm();

            // 重置所有状态
            self.IsCd = false;
            self.IsNormalAttacking = false;
            self.IsChargeAttacking = false;
            self.IsUltimateAttacking = false;
            self.IsFinished = false;
            self.IsChargeAttackOnCd = false;
        }

        public static void Attack(this HuangFengDaShengAttackComponent self, Unit unit)
        {
            self.AttackBefore(unit);
        }

        public static void AttackBefore(this HuangFengDaShengAttackComponent self, Unit target)
        {
            self.Target = target;
            Unit unit = self.GetParent<Unit>();
            unit.SetMonsterActionType(MonsterActionType.Attack);
        }

        [EntitySystem]
        private static void Update(this ET.Client.HuangFengDaShengAttackComponent self)
        {
            // 先拿到自身Unit，供刷新龙卷风位置使用
            Unit unit = self.GetParent<Unit>();
            if (unit == null || unit.IsDisposed)
                return;

            // 虚弱中：禁止一切攻击逻辑（不重复设置动画）
            if (self.IsWeakened)
            {
                return;
            }

            // 不受战斗状态与CD影响的龙卷风刷新机制：放在最前面
            TryRefreshTornadoes(self, unit).NoContext();

            // 检查攻击状态
            if (unit.GetMonsterActionType() != MonsterActionType.Attack)
                return;

            // 检查目标有效性
            Unit target = self.Target;
            if (target == null || target.IsDisposed)
            {
                // 如果正在执行蓄力攻击，不中断
                if (self.IsChargeAttacking)
                {
                    Debug.Log("黄风大圣：正在执行蓄力攻击，忽略目标无效检查");
                }
                // 如果处于虚弱状态，不中断
                else if (self.IsWeakened)
                {
                    Debug.Log("黄风大圣：处于虚弱状态，忽略目标无效检查");
                }
                // 如果正在释放大招，不中断
                else if (self.IsUltimateAttacking)
                {
                    Debug.Log("黄风大圣：正在释放大招，忽略目标无效检查");
                }
                else
                {
                    self.Target = null;
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Walk);
                    return;
                }
            }

            // 检查目标存活
            if (!target.IsAlive())
            {
                // 如果正在执行蓄力攻击，不中断
                if (self.IsChargeAttacking)
                {
                    Debug.Log("黄风大圣：正在执行蓄力攻击，忽略目标死亡检查");
                }
                // 如果处于虚弱状态，不中断
                else if (self.IsWeakened)
                {
                    Debug.Log("黄风大圣：处于虚弱状态，忽略目标死亡检查");
                }
                // 如果正在释放大招，不中断
                else if (self.IsUltimateAttacking)
                {
                    Debug.Log("黄风大圣：正在释放大招，忽略目标死亡检查");
                }
                else
                {
                    self.Target = null;
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Walk);
                    return;
                }
            }

            // 判断是否超出攻击范围 - 只有普通攻击才需要检查AttackFlag
            // 蓄力攻击可以超出攻击碰撞体范围执行
            if (!self.AttackFlag)
            {
                // 如果正在执行蓄力攻击，不中断
                if (self.IsChargeAttacking)
                {
                    Debug.Log("黄风大圣：正在执行蓄力攻击，忽略AttackFlag检查");
                }
                // 如果处于虚弱状态，不中断
                else if (self.IsWeakened)
                {
                    Debug.Log("黄风大圣：处于虚弱状态，忽略AttackFlag检查");
                }
                // 如果正在释放大招，不中断
                else if (self.IsUltimateAttacking)
                {
                    Debug.Log("黄风大圣：正在释放大招，忽略AttackFlag检查");
                }
                else
                {
                    Debug.LogWarning("超出仇恨范围！！！");
                    self.Target = null;
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Walk);
                    return;
                }
            }

            // 如果正在执行任何类型的攻击，跳过
            if (self.IsNormalAttacking || self.IsChargeAttacking || self.IsUltimateAttacking)
            {
                return;
            }

            // 检查是否有可用的攻击类型
            bool canNormalAttack = !self.IsCd;
            bool canChargeAttack = !self.IsChargeAttackOnCd;

            // 如果所有攻击类型都在CD中，跳过
            if (!canNormalAttack && !canChargeAttack)
            {
                // Debug.Log("黄风大圣：所有攻击都在CD中，等待CD结束" +
                //           $" (普通攻击CD={self.IsCd}, 蓄力攻击CD={self.IsChargeAttackOnCd}");
                // 每隔2秒输出一次，避免刷屏
                if (TimeInfo.Instance.ClientFrameTime() % 60 == 0) // 60fps情况下，120帧=2秒
                {
                    Debug.Log($"黄风大圣：所有攻击都在CD中 (普通攻击CD={self.IsCd}, 蓄力攻击CD={self.IsChargeAttackOnCd})");
                }

                return;
            }

            // 输出当前可用的攻击类型（调试用）
            if (TimeInfo.Instance.ClientFrameTime() % 1200 == 0)
            {
                Debug.Log($"黄风大圣：可用攻击类型 - 普通攻击:{(canNormalAttack ? "可用" : "CD中")}, 蓄力攻击:{(canChargeAttack ? "可用" : "CD中")}");
            }

            // 执行攻击选择逻辑（这里会设置IsCd=true，防止重复触发）
            TimerComponent timerComponent = unit.Root().GetComponent<TimerComponent>();
            if (timerComponent == null)
            {
                Debug.LogError("黄风大圣：TimerComponent为空，无法执行攻击");
                return;
            }

            self.ExecuteAttackDecision();

          
        }

        // 攻击决策系统
        private static void ExecuteAttackDecision(this HuangFengDaShengAttackComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            TimerComponent timerComponent = self.Root().GetComponent<TimerComponent>();

            // 检查可用的攻击类型
            bool canNormalAttack = !self.IsCd;
            bool canChargeAttack = !self.IsChargeAttackOnCd;

            
            // 大招释放条件：自身HP <= 50% MaxHp 才允许
            NumericComponent nc = unit.GetComponent<NumericComponent>();
            long hp = nc[NumericType.Hp];
            long maxHp = nc[NumericType.MaxHp];
            bool canCastUltimate = maxHp > 0 && hp * 2 <= maxHp; // hp/maxHp <= 0.5

            if (canCastUltimate && self.CanCastUltimateNow(unit))
            {
                self.ExecuteUltimate(unit, timerComponent);
                return;
            }

            // 否则按权重选择 普攻/蓄力
            int randomValue = RandomGenerator.RandomNumber(1, 101);
            
            if (randomValue <= self.NormalAttackWeight )//|| !canCastUltimate)
            {
                Debug.Log($"攻击权重111： "+randomValue + $"黄风大圣：开始攻击决策 - 普通攻击:{(canNormalAttack ? "可用" : "CD中")}, 蓄力攻击:{(canChargeAttack ? "可用" : "CD中")}");
             
                self.ExecuteNormalAttack(unit, timerComponent).NoContext(); 
                
            }
            else
            {
                Debug.Log($"攻击权重111： "+randomValue + $"黄风大圣：开始攻击决策 - 普通攻击:{(canNormalAttack ? "可用" : "CD中")}, 蓄力攻击:{(canChargeAttack ? "可用" : "CD中")}");
            
                self.ExecuteChargeAttack(unit, timerComponent).NoContext();
               
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                self.TestState = 7;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                self.TestState = 8;
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                self.TestState = 9;
            }

            // switch (self.TestState)
            // {
            //     case 7:
            //         self.ExecuteNormalAttack(unit, timerComponent).Coroutine();
            //         break;
            //     case 8 :
            //         self.ExecuteChargeAttack(unit, timerComponent).Coroutine();
            //         break;
            //     case 9 :
            //         self.ExecuteUltimate(unit, timerComponent);
            //         break;
            // }
            // //普通攻击
            //
            // //释放蓄力攻击
            //  self.ExecuteChargeAttack(unit, timerComponent).Coroutine();
            // //释放大招
            //  self.ExecuteUltimate(unit, timerComponent);
        }

        // 清理所有定时器
        private static void ClearAllTimers(this HuangFengDaShengAttackComponent self, TimerComponent timerComponent)
        {
            timerComponent.Remove(ref self.AttackCdTimerId);
            timerComponent.Remove(ref self.ChargeAttackCdTimerId);
            timerComponent.Remove(ref self.ZhendaoWindowTimerId);
        }

        // 执行普通攻击
        private static async ETTask ExecuteNormalAttack(this HuangFengDaShengAttackComponent self, Unit unit, TimerComponent timerComponent)
        {
            // 检查是否可以执行普通攻击
            if (self.IsCd || self.IsNormalAttacking)
            {
                Debug.Log("黄风大圣：普通攻击在CD或正在执行中，跳过");
                return;
            }

            Debug.Log("黄风大圣：执行普通攻击！虚弱状态"+!self.IsWeakened);

            // 设置攻击状态，防止重复触发和互相打断
            self.IsCd = true;
            self.IsNormalAttacking = true;

            try
            {
                // 设置普通攻击动画
                unit.SetAIUnitActionType(UnitActionType.Attack, false);
                Debug.Log("黄风大圣：普通攻击动画开始");

                // 计算动画时长与命中时机（若无法获取长度，则默认1秒，命中在0.5秒）
                AISkeletonAnimationComponent animComponent = unit.GetComponent<AISkeletonAnimationComponent>();
                float normalAnimSeconds = animComponent?.GetTimeLength(SkeletonAnimationType.front_attack) ?? 1.0f;
                int normalAnimMs = (int)(normalAnimSeconds * 1000);
                int hitDelayMs = Math.Max(0, Math.Min(normalAnimMs / 2, normalAnimMs - 50));

                // 命中前等待
                await timerComponent.WaitAsync(hitDelayMs);

                // 触发伤害事件（与小怪一致使用AttackEvent），确保普通攻击会扣血
                try
                {
                    EventSystem.Instance.Publish(unit.Root(), new AttackEvent() { Unit = unit });
                    Debug.Log("黄风大圣：普通攻击命中，已发布AttackEvent");
                }
                catch (Exception e)
                {
                    Debug.LogError($"黄风大圣：发布AttackEvent失败: {e.Message}");
                }

                // 等待剩余动画时间
                int remainMs = Math.Max(0, normalAnimMs - hitDelayMs);
                if (remainMs > 0)
                {
                    await timerComponent.WaitAsync(remainMs);
                }

                // 检查是否被销毁
                if (self.IsDisposed || unit.IsDisposed)
                {
                    Debug.LogWarning("黄风大圣：普通攻击执行期间组件被销毁");
                    return;
                }

                

                // 虚弱状态下不改变动画，否则恢复到Idle并保持Attack状态继续战斗
                if (!self.IsWeakened)
                {
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Attack);
                    Debug.Log("黄风大圣：普通攻击完成，Unit状态已设置为Attack-Idle");
                }

                // 设置普通攻击CD定时器
                Debug.Log($"黄风大圣：设置普通攻击CD定时器: {self.NormalAttackCooldown}ms");
                self.AttackCdTimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + self.NormalAttackCooldown,
                    TimerInvokeType.HuangFengNormalAttackTimer,
                    self);
                
                // 普通攻击完成，重置攻击状态
                self.IsNormalAttacking = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"黄风大圣普通攻击异常: {e.Message}");

                // 异常时确保状态重置
                if (!self.IsDisposed)
                {
                    self.IsNormalAttacking = false;

                    // 设置一个短暂的CD后重新开始
                    self.AttackCdTimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + 1000,
                        TimerInvokeType.HuangFengNormalAttackTimer,
                        self);

                    if (unit != null && !unit.IsDisposed && !self.IsWeakened)
                    {
                        unit.SetAIUnitActionType(UnitActionType.Idle);
                        unit.SetMonsterActionType(MonsterActionType.Attack);
                    }
                }
            }
        }

        // 执行蓄力攻击
        private static async ETTask ExecuteChargeAttack(this HuangFengDaShengAttackComponent self, Unit unit, TimerComponent timerComponent)
        {
            // 检查是否可以执行蓄力攻击
            if (self.IsChargeAttackOnCd || self.IsChargeAttacking)
            {
                Debug.Log("黄风大圣：蓄力攻击在CD或正在执行中，跳过");
                return;
            }

            Debug.Log("黄风大圣：开始蓄力攻击！");

            // 只设置蓄力攻击相关状态，不影响普通攻击CD
            self.IsChargeAttacking = true;
            self.IsFinished = false;

            // 阶段1：开始蓄力（先转向，再切动画1）
            self.OrientTowardsTarget2p5D(unit);
            unit.SetAIUnitActionType(UnitActionType.HuangFengXuLi_1, false);
            await timerComponent.WaitAsync(300);
            unit.SetAIUnitActionType(UnitActionType.HuangFengXuLi_2, false);
            await timerComponent.WaitAsync(1000);
            unit.SetAIUnitActionType(UnitActionType.HuangFengAttack1, false);

            
            // 启动蓄力攻击流程（若中途进入虚弱，应立即中断）
            self.StartChargeAttackSequence(unit, timerComponent).NoContext();
            
        }

        // 执行大招：增加蓄力前摇，然后生成龙卷风；刷新逻辑仍在Update中
        private static void ExecuteUltimate(this HuangFengDaShengAttackComponent self, Unit unit, TimerComponent timerComponent)
        {
            if (self.IsUltimateAttacking)
            {
                Debug.Log("黄风大圣：大招正在进行中，跳过");
                return;
            }

            // 只有“可以释放大招”时才进入蓄力前摇，避免蓄力后发现无法释放
            if (!self.CanCastUltimateNow(unit))
            {
                Debug.Log("黄风大圣：当前不满足释放大招条件，跳过蓄力");
                return;
            }

            // 启动带有前摇的释放流程
            self.StartUltimateSequence(unit, timerComponent).NoContext();
        }

        // 大招前摇 + 释放流程
        private static async ETTask StartUltimateSequence(this HuangFengDaShengAttackComponent self, Unit unit, TimerComponent timerComponent)
        {
            try
            {
                self.IsUltimateAttacking = true;

                // 阶段1：面向目标 + 蓄力前摇（复用蓄力攻击的前摇风格）
                self.OrientTowardsTarget2p5D(unit);
                unit.SetAIUnitActionType(UnitActionType.HuangFengXuLi_1, false);
                await timerComponent.WaitAsync(300);
                unit.SetAIUnitActionType(UnitActionType.HuangFengXuLi_2, false);
                await timerComponent.WaitAsync(1000);

                if (self.IsDisposed || unit.IsDisposed)
                {
                    return;
                }
                // 阶段2：释放大招动画
                unit.SetAIUnitActionType(UnitActionType.HuangFengXuLi_3, false);
                await timerComponent.WaitAsync(500);

                // 阶段3：生成龙卷风（仅当当前不存在龙卷风时才生成）
                if (!self.HasActiveTornadoes && !self.IsRefreshingTornadoes)
                {
                    await SpawnAndTrackTornadoes(self, unit);
                }

                // 结束大招标记
                self.IsUltimateAttacking = false;

                // 重置普通攻击CD状态，确保大招后可以立即进行普通攻击
                self.IsCd = false;
                if (self.AttackCdTimerId != 0)
                {
                    timerComponent.Remove(ref self.AttackCdTimerId);
                    self.AttackCdTimerId = 0;
                }

                // 虚弱状态下不改变动画，否则保持战斗状态
                if (!self.IsWeakened)
                {
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Attack);
                    
                   
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"黄风大圣：大招前摇/释放异常 {e.Message}");
                self.IsUltimateAttacking = false;
                
                // 重置普通攻击CD状态，确保异常后可以立即进行普通攻击
                self.IsCd = false;
                if (self.AttackCdTimerId != 0)
                {
                    timerComponent.Remove(ref self.AttackCdTimerId);
                    self.AttackCdTimerId = 0;
                }
                
                if (unit != null && !unit.IsDisposed && !self.IsWeakened)
                {
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Attack);
                }
            }
        }

        // 大招可释放性校验：仅当满足时才进入蓄力
        private static bool CanCastUltimateNow(this HuangFengDaShengAttackComponent self, Unit unit)
        {
            // 1. 检查是否已有活跃的龙卷风
            if (self.HasActiveTornadoes) return false;
            // 2. 检查是否正在刷新龙卷风
            if (self.IsRefreshingTornadoes) return false;

            // 3. 检查目标是否存在且有效
            Unit target = self.Target;
            if (target == null || target.IsDisposed) return false;
            GameObject targetGO = target.GetComponent<GameObjectComponent>()?.GameObject;
            if (targetGO == null) return false;

            // 4. 检查黄风大圣的GameObject是否存在
            GameObject bossGO = unit.GetComponent<GameObjectComponent>()?.GameObject;
            if (bossGO == null) return false;

            return true;
        }

        // 生成并记录四个龙卷风（围绕当前目标四向），保存释放点
        private static async ETTask SpawnAndTrackTornadoes(HuangFengDaShengAttackComponent self, Unit bossUnit)
        {
            try
            {
                Unit target = self.Target;
                if (target == null || target.IsDisposed) return;
                GameObject targetGO = target.GetComponent<GameObjectComponent>()?.GameObject;
                if (targetGO == null) return;

                // 记录释放点（使用GameObject位置，避免Unit.Position不同步）
                GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
                if (bossGO == null) return;
                Vector3 origin = bossGO.transform.position;
                self.UltimateOriginX = origin.x;
                self.UltimateOriginY = origin.y;
                self.UltimateOriginZ = origin.z;

                // 清空旧引用
                if (self.TornadoUnits != null)
                    self.TornadoUnits.Clear();

                // 以释放者(黄风大圣)为基准点生成龙卷风
                Vector3 center = bossGO.transform.position;
                float radius = 5f;
                Vector3[] offsets = new Vector3[]
                {
                    new Vector3(+radius, 0, 0),
                    new Vector3(-radius, 0, 0),
                    new Vector3(0, 0, +radius),
                    new Vector3(0, 0, -radius),
                    new Vector3(+radius, 0, +radius),
                    new Vector3(+radius, 0, -radius),
                    new Vector3(-radius, 0, +radius),
                    new Vector3(-radius, 0, -radius),
                };

                foreach (var off in offsets)
                {
                    Vector3 pos = center + off;
                    Unit tornado = await UnitFactory.CreateLongJuanFeng(bossUnit.Root().CurrentScene(), pos);
                    if (tornado != null && !tornado.IsDisposed)
                    {
                        // 设置龙卷风的BossUnit引用，让龙卷风知道自己的主人
                        LongJuanFengMoveComponent tornadoMoveComponent = tornado.GetComponent<LongJuanFengMoveComponent>();
                        if (tornadoMoveComponent != null)
                        {
                            tornadoMoveComponent.BossUnit = bossUnit;
                            // 初始化个人偏移，后续用于防汇聚矫正
                            tornadoMoveComponent.PersonalOffset = off;
                            tornadoMoveComponent.OrbitRadius = radius;
                            tornadoMoveComponent.RandomRadius = radius * 0.75f; // 缩小随机半径，避免偏移被快速抵消
                        }
                        self.TornadoUnits.Add(tornado);
                    }
                }

                // 生成沙尘暴效果并保存引用
                self.SandstormGameObject = await YIUIGameObjectPool.Inst.Get("Sandstorm_Particles1");
                if (self.SandstormGameObject != null)
                {
                    // 设置沙尘暴位置为黄风大圣当前位置
                    self.SandstormGameObject.transform.position = new Vector3(0, 4, 0);
                    self.SandstormGameObject.SetActive(true);
                    Debug.Log($"黄风大圣：生成沙尘暴效果，位置=({center.x},{center.y+4},{center.z})");
                }

                self.HasActiveTornadoes = self.TornadoUnits != null && self.TornadoUnits.Count > 0;
                Debug.Log($"黄风大圣：生成龙卷风完成，数量={self.TornadoUnits.Count}，记录释放点=({self.UltimateOriginX},{self.UltimateOriginY},{self.UltimateOriginZ})");
            }
            catch (Exception e)
            {
                Debug.LogError($"黄风大圣：生成龙卷风异常 {e.Message}");
            }
        }

        // 在Update中调用：若怪物当前与释放点的距离超过阈值，销毁旧龙卷风并在当前位置重新施放大招
        private static async ETTask TryRefreshTornadoes(HuangFengDaShengAttackComponent self, Unit bossUnit)
        {
            if (!self.HasActiveTornadoes)
                return;
            if (self.IsUltimateAttacking)
                return;
            if (self.IsRefreshingTornadoes)
                return;

            GameObject bossGO = bossUnit.GetComponent<GameObjectComponent>()?.GameObject;
            if (bossGO == null) return;
            Vector3 origin = new Vector3(self.UltimateOriginX, self.UltimateOriginY, self.UltimateOriginZ);
            Vector3 current = bossGO.transform.position;
            // 2.5D：仅比较XZ平面距离
            origin.y = 0;
            current.y = 0;
            float dist = Vector3.Distance(current, origin);
            if (dist <= self.TornadoRefreshDistance)
                return;

            // 标记刷新，防止重复触发
            self.IsRefreshingTornadoes = true;
            try
            {
                // 销毁旧龙卷风和沙尘暴
                if (self.TornadoUnits != null)
                {
                    foreach (var refUnit in self.TornadoUnits)
                    {
                        Unit tu = refUnit;
                        if (tu != null && !tu.IsDisposed)
                        {
                            tu.Dispose();
                        }
                    }
                    self.TornadoUnits.Clear();
                }
                self.HasActiveTornadoes = false;
                
                // 回收旧的沙尘暴效果
                self.RecycleSandstorm();
                
                Debug.Log($"黄风大圣：与释放点距离{dist:F2}超过阈值{self.TornadoRefreshDistance}，已销毁旧龙卷风和沙尘暴，准备在新位置重召");

                // 在当前位置重新生成
                await SpawnAndTrackTornadoes(self, bossUnit);
            }
            finally
            {
                self.IsRefreshingTornadoes = false;
            }
        }

        // 蓄力攻击序列
        public static async ETTask StartChargeAttackSequence(this HuangFengDaShengAttackComponent self, Unit unit, TimerComponent timerComponent)
        {
            try
            {
                Debug.Log("黄风大圣：开始蓄力攻击序列");

                // 确保在开始前状态正确
                if (self.IsDisposed || unit.IsDisposed)
                {
                    Debug.LogWarning("黄风大圣：组件或Unit已销毁，取消蓄力攻击");
                    return;
                }

                // 蓄力攻击不需要检查AttackFlag，可以超出攻击碰撞体范围
                // 只要目标存在且存活即可执行蓄力攻击
                Unit target = self.Target;
                if (target == null || target.IsDisposed || !target.IsAlive())
                {
                    Debug.LogWarning("黄风大圣：开始蓄力攻击时目标无效或已死亡，取消攻击");
                    self.IsChargeAttacking = false;
                    return;
                }

                GameObject gameObject = unit.GetComponent<GameObjectComponent>().GameObject;
                MonsterAttackCollector monsterAttackCollector = gameObject.GetComponentInChildren<MonsterAttackCollector>(true);
                if (monsterAttackCollector != null)
                {
                    // 停留与离开此处无需逻辑
                    monsterAttackCollector.SetOnTriggerStay(other =>
                    {
                        var player = other.GetComponentInParent<PlayerBehaviour>();
                        if (player == null) return;
                        if (self._hasDealtDamageThisRush) return;
                        Unit target = UnitHelper.GetMyUnitFromClientScene(unit.Root());
                        if (target == null || !target.IsAlive()) return;
                        self._hasDealtDamageThisRush = true;

                        int baseDamage = (int)unit.GetComponent<NumericComponent>()[NumericType.Damage];
                        int damage = baseDamage == 0 ? 25 : (int)(baseDamage * 1.5f);
                        EventSystem.Instance.PublishAsync(self.Root(), new AttackUnitStart
                        {
                            AttackUnit = unit,
                            TargetUnit = target,
                            Damage = damage
                        }).NoContext();
                    });
                }
                // 连续三轮：动画1-2-3 + 冲刺
                AISkeletonAnimationComponent animComponent = unit.GetComponent<AISkeletonAnimationComponent>();
                for (int round = 1; round <= 3; round++)
                {
                    // 蓄力攻击不需要检查AttackFlag，只检查目标是否有效
                    if (target == null || target.IsDisposed || !target.IsAlive())
                    {
                        Debug.LogWarning($"黄风大圣：第{round}轮蓄力攻击时目标无效，停止攻击");
                        break;
                    }

                    // 每轮开始前重置本轮伤害标记
                    self._hasDealtDamageThisRush = false;

                    // 阶段1：准备（先转向，再切动画1）
                    Debug.Log($"黄风大圣：第{round}轮 蓄力攻击阶段1 - 准备");
                    if (self.IsWeakened) break;
                    self.OrientTowardsTarget2p5D(unit);
                    unit.SetAIUnitActionType(UnitActionType.HuangFengAttack1, false);
                    
                    // 添加保护检查，确保动画设置成功
                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段1动画设置后组件已销毁，退出");
                        return;
                    }
                    
                    await timerComponent.WaitAsync(500);

                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段1后组件已销毁，退出");
                        return;
                    }

                    // 阶段2：蓄力中 + 冲刺（先转向，再开始蓄力/冲刺）
                    Debug.Log($"黄风大圣：第{round}轮 蓄力攻击阶段2 - 蓄力中 + 冲刺");
                    if (self.IsWeakened) break;
                    self.OrientTowardsTarget2p5D(unit);
                    unit.SetAIUnitActionType(UnitActionType.HuangFengAttack2);
                    
                    // 添加保护检查，确保动画设置成功
                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段2动画设置后组件已销毁，退出");
                        return;
                    }
                    
                    // 先开启碰撞体，再开始位移，确保进入触发器会触发 OnTriggerEnter
                    if (monsterAttackCollector != null) monsterAttackCollector.gameObject.SetActive(true);
                    Debug.Log("黄风大圣：已开启攻击碰撞体");
                    self.ExecuteChargeRush(unit, 500);
                    await timerComponent.WaitAsync(500);

                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段2后组件已销毁，退出");
                        return;
                    }

                    // 阶段3：爆发
                    if (self.IsWeakened) break;
                    Debug.Log($"黄风大圣：第{round}轮 蓄力攻击阶段3 - 爆发攻击！");
                    unit.SetAIUnitActionType(UnitActionType.HuangFengAttack3, false);
                    
                    // 添加保护检查，确保动画设置成功
                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段3动画设置后组件已销毁，退出");
                        return;
                    }
                    
                    if (monsterAttackCollector != null) monsterAttackCollector.gameObject.SetActive(false);
                    Debug.Log("黄风大圣：已关闭攻击碰撞体");
                    // 等待攻击动画完成
                    float animationDuration = animComponent?.GetTimeLength(SkeletonAnimationType.xuanfeng_3) ?? 1.5f;
                    int attackDuration = (int)(animationDuration * 1000);
                    Debug.Log($"黄风大圣：第{round}轮 等待爆发动画完成，时长: {attackDuration}ms");
                    await timerComponent.WaitAsync(attackDuration);

                    if (self.IsDisposed || unit.IsDisposed)
                    {
                        Debug.LogWarning("黄风大圣：阶段3后组件已销毁，退出");
                        return;
                    }
                }
                if (monsterAttackCollector != null) monsterAttackCollector.gameObject.SetActive(false);
                // 攻击完成，立即设置Unit状态
                Debug.Log("黄风大圣：蓄力攻击完成");
                
                self.IsFinished = true;

                // 设置蓄力攻击进入CD状态（不影响普通攻击）
                self.IsChargeAttackOnCd = true;

                // 虚弱状态下不改变动画，否则立即设置Unit状态为攻击状态，让它可以继续战斗逻辑
                if (!self.IsWeakened)
                {
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Attack);
                    Debug.Log("黄风大圣：蓄力攻击完成，Unit状态已设置为Attack，蓄力攻击进入CD");
                }

                // 设置蓄力攻击CD定时器
                Debug.Log($"黄风大圣：设置蓄力攻击CD定时器: {self.ChargeAttackCooldown}ms");
                self.ChargeAttackCdTimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + self.ChargeAttackCooldown,
                    TimerInvokeType.HuangFengChargeAttackTimer,
                    self);
                Debug.Log($"黄风大圣：蓄力攻击CD定时器已设置，普通攻击不受影响，TimerId: {self.ChargeAttackCdTimerId}");
                self.IsChargeAttacking = false;
                
            }
            catch (Exception e)
            {
                Debug.LogError($"黄风大圣蓄力攻击序列异常: {e.Message}");

                // 异常时确保状态正确重置
                if (!self.IsDisposed)
                {
                    
                    // 设置蓄力攻击进入CD状态（即使异常也要CD）
                    self.IsChargeAttackOnCd = true;

                    // 虚弱状态下不改变动画，否则立即设置Unit状态
                    if (unit != null && !unit.IsDisposed && !self.IsWeakened)
                    {
                        unit.SetAIUnitActionType(UnitActionType.Idle);
                        unit.SetMonsterActionType(MonsterActionType.Attack);
                        Debug.Log("黄风大圣：异常恢复，Unit状态已设置为Attack，蓄力攻击进入CD");
                    }

                    // 设置一个短暂的蓄力攻击恢复CD（1秒）
                    self.ChargeAttackCdTimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + 1000,
                        TimerInvokeType.HuangFengChargeAttackTimer,
                        self);
                    
                    Debug.LogWarning("黄风大圣：蓄力攻击异常，立即恢复状态");
                    self.IsChargeAttacking = false;

                }
            }
        }

        // 执行冲刺攻击（动画和移动同步）
        private static void ExecuteChargeRush(this HuangFengDaShengAttackComponent self, Unit unit, int rushDuration)
        {
            Debug.Log("黄风大圣：设置冲刺动画并开始移动");
            // unit.SetAIUnitActionType(UnitActionType.Rush); // 如果有专门的冲刺动画
            // 检查是否有目标
            Unit target = self.Target;
            if (target == null || target.IsDisposed)
            {
                Debug.LogWarning("黄风大圣：冲刺时没有目标，跳过冲刺");
                return;
            }

            GameObject monsterGO = unit.GetComponent<GameObjectComponent>().GameObject;
            GameObject targetGO = target.GetComponent<GameObjectComponent>().GameObject;

            if (monsterGO == null || targetGO == null)
            {
                Debug.LogWarning("黄风大圣：冲刺时GameObject为空，跳过冲刺");
                return;
            }

            // 获取当前位置和目标位置（只考虑XZ轴，保持Y轴不变）
            Vector3 startPos = monsterGO.transform.position;
            Vector3 targetPos = targetGO.transform.position;

            // 2.5D游戏：只在XZ平面计算方向，保持原始Y轴高度
            Vector3 startPosXZ = new Vector3(startPos.x, 0, startPos.z);
            Vector3 targetPosXZ = new Vector3(targetPos.x, 0, targetPos.z);

            // 计算XZ平面上的冲刺方向
            Vector3 directionXZ = (targetPosXZ - startPosXZ).normalized;

            // 2.5D转向：仅绕Y轴（与随机移动一致）
            OrientTransform2p5D(monsterGO.transform, directionXZ);

            float rushDistance = 3.0f;

            // 计算冲刺目标位置（保持原始Y轴高度）
            Vector3 rushTargetPosXZ = startPosXZ + directionXZ * rushDistance;

            // 最终冲刺位置：使用原始Y轴高度
            Vector3 rushTargetPos = new Vector3(rushTargetPosXZ.x, startPos.y, rushTargetPosXZ.z);

            Debug.Log($"黄风大圣：开始冲刺(XZ轴) 从{startPos} 到{rushTargetPos}，距离:{Vector3.Distance(startPosXZ, rushTargetPosXZ)}米");

            // 🎬 同步开始：设置冲刺动画 + 开始移动
            Debug.Log("黄风大圣：🎬 动画和移动同步开始！");
            monsterGO.transform.DOMove(rushTargetPos, rushDuration / 1000f).SetEase(Ease.OutCubic);
        }

        // 仅绕Y轴的2.5D转向辅助
        private static void OrientTransform2p5D(Transform root, Vector3 directionXZ)
        {
            try
            {
                Transform modelTransform = root.childCount > 0 ? root.GetChild(0) : root;
                Vector3 currentEuler = modelTransform.localEulerAngles;
                currentEuler.y = directionXZ.x >= 0f ? 0f : 180f;
                modelTransform.localEulerAngles = currentEuler;
            }
            catch (Exception)
            {
                // 忽略
            }
        }

        // 面向当前目标（2.5D，仅Y轴），供阶段2开始前调用
        private static void OrientTowardsTarget2p5D(this HuangFengDaShengAttackComponent self, Unit unit)
        {
            Unit target = self.Target;
            if (target == null || target.IsDisposed) return;
            GameObject monsterGO = unit.GetComponent<GameObjectComponent>().GameObject;
            GameObject targetGO = target.GetComponent<GameObjectComponent>().GameObject;
            if (monsterGO == null || targetGO == null) return;

            Vector3 startPos = monsterGO.transform.position;
            Vector3 targetPos = targetGO.transform.position;
            Vector3 startPosXZ = new Vector3(startPos.x, 0, startPos.z);
            Vector3 targetPosXZ = new Vector3(targetPos.x, 0, targetPos.z);
            Vector3 directionXZ = (targetPosXZ - startPosXZ).normalized;
            if (directionXZ == Vector3.zero) return;
            OrientTransform2p5D(monsterGO.transform, directionXZ);
        }

        // 订阅动画事件
        private static void SubscribeAnimationEvents(this HuangFengDaShengAttackComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            Debug.Log($"黄风大圣[{unit.Id}]订阅动画事件");
        }

        // 回收沙尘暴效果
        private static void RecycleSandstorm(this HuangFengDaShengAttackComponent self)
        {
            if (self.SandstormGameObject != null)
            {
                Debug.Log("黄风大圣：回收沙尘暴效果");
                YIUIGameObjectPool.Inst.Put(self.SandstormGameObject);
                self.SandstormGameObject = null;
            }
        }
    }

    // 黄风大圣普通攻击CD定时器
    [Invoke(TimerInvokeType.HuangFengNormalAttackTimer)]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public class HuangFengDaSheng_NormalAttackCdTimer : ATimer<HuangFengDaShengAttackComponent>
    {
        protected override void Run(HuangFengDaShengAttackComponent self)
        {
            if (self == null || self.IsDisposed)
                return;

            var unit = self.GetParent<Unit>();
            if (unit == null || unit.IsDisposed)
                return;

            Debug.Log("🟢 黄风大圣：普通攻击CD结束，现在可以继续普通攻击了！");

            // 重置普通攻击CD状态（不影响蓄力攻击CD）
            self.IsCd = false;

            Debug.Log($"黄风大圣：普通攻击CD重置完成 - 普通攻击CD={self.IsCd}, 蓄力攻击CD={self.IsChargeAttackOnCd}");
        }
    }

    // 黄风大圣大招CD定时器
    [Invoke(TimerInvokeType.HuangFengWindUltimateTimer)]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public class HuangFengDaSheng_UltimateCdTimer : ATimer<HuangFengDaShengAttackComponent>
    {
        protected override void Run(HuangFengDaShengAttackComponent self)
        {
            if (self == null || self.IsDisposed)
                return;

            var unit = self.GetParent<Unit>();
            if (unit == null || unit.IsDisposed)
                return;

            self.UltimateAttackCdTimerId = 0;
            Debug.Log("🟢 黄风大圣：大招CD结束，现在可以再次释放大招！");
        }
    }

    // 黄风大圣蓄力攻击CD定时器
    [Invoke(TimerInvokeType.HuangFengChargeAttackTimer)]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public class HuangFengDaSheng_ChargeAttackCdTimer : ATimer<HuangFengDaShengAttackComponent>
    {
        protected override void Run(HuangFengDaShengAttackComponent self)
        {
            if (self == null || self.IsDisposed)
                return;

            var unit = self.GetParent<Unit>();
            if (unit == null || unit.IsDisposed)
                return;

            Debug.Log("🟢 黄风大圣：蓄力攻击CD结束，现在可以继续蓄力攻击了！");

            // 重置蓄力攻击CD状态（不影响普通攻击CD）
            self.IsChargeAttackOnCd = false;

            Debug.Log($"黄风大圣：蓄力攻击CD重置完成 - 普通攻击CD={self.IsCd}, 蓄力攻击CD={self.IsChargeAttackOnCd}");
        }
    }

    // 黄风大圣攻击开始事件处理器
    [Event(SceneType.StateSync)]
    public class HuangFengDaSheng_AttackStartEvent_Handler : AEvent<Scene, AttackStartEvent>
    {
        protected override async ETTask Run(Scene scene, AttackStartEvent args)
        {
            Unit unit = args.Unit;

            // 只处理拥有HuangFengDaShengAttackComponent的Unit
            HuangFengDaShengAttackComponent attackComponent = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (attackComponent == null)
                return;

            Debug.Log($"黄风大圣[{unit.Id}]处理attack_start事件");

            // 动画事件仅用于记录，实际攻击序列由ExecuteChargeAttack启动
            Debug.Log("黄风大圣：attack_start动画事件已接收，攻击序列已在ExecuteChargeAttack中启动");

            await ETTask.CompletedTask;
        }
    }

    // 黄风大圣攻击结束事件处理器
    [Event(SceneType.StateSync)]
    public class HuangFengDaSheng_AttackFinishEvent_Handler : AEvent<Scene, AttackFinishEvent>
    {
        protected override async ETTask Run(Scene scene, AttackFinishEvent args)
        {
            Unit unit = args.Unit;

            // 只处理拥有HuangFengDaShengAttackComponent的Unit
            HuangFengDaShengAttackComponent attackComponent = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (attackComponent == null)
                return;

            await ETTask.CompletedTask;
        }
    }

    // 黄风大圣普通攻击事件处理器
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public class HuangFengDaSheng_AttackEvent_Handler : AEvent<Scene, AttackEvent>
    {
        protected override async ETTask Run(Scene scene, AttackEvent args)
        {
            Unit unit = args.Unit;

            // 只处理拥有HuangFengDaShengAttackComponent的Unit
            HuangFengDaShengAttackComponent attackComponent = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (attackComponent == null)
                return;

            Debug.Log($"黄风大圣[{unit.Id}]处理attack事件 - 普通攻击");

            // 处理黄风大圣的普通攻击逻辑
            HandleHuangFengDaShengNormalAttack(unit, attackComponent);

            await ETTask.CompletedTask;
        }

        private static void HandleHuangFengDaShengNormalAttack(Unit bossUnit, HuangFengDaShengAttackComponent attackComponent)
        {
            // 普通攻击需要检查AttackFlag，确保在攻击范围内
            if (!attackComponent.AttackFlag)
            {
                Debug.Log($"黄风大圣[{bossUnit.Id}]普通攻击标志为false，跳过攻击");
                return;
            }

            // 获取攻击目标(玩家)
            Unit target = attackComponent.Target;
            if (target == null || !target.IsAlive())
            {
                return;
            }

            // 获取黄风大圣的攻击力（比普通怪物更强）
            int attackPower = (int)bossUnit.GetComponent<NumericComponent>()[NumericType.Damage];

            // 统一扣血入口（含振刀判定）
            int power = attackPower == 0 ? 25 : (int)(attackPower * 1.5f); // 比普通怪物强50%、
            EventSystem.Instance.PublishAsync(bossUnit.Root(), new AttackUnitStart()
            {
                AttackUnit = bossUnit,
                TargetUnit = target,
                Damage = power
            }).NoContext();
        }
    }
}