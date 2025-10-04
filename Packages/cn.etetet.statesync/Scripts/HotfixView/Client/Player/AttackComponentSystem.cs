using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(AttackComponent))]
    [FriendOf(typeof(AttackComponent))]
    [FriendOfAttribute(typeof(ET.PlayerBehaviourComponent))]
    public static partial class AttackComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.AttackComponent self)
        {
            //找到他下面的
            self.unit =self.GetParent<Unit>();
            Unit unit = self.unit;
            GameObject obj = unit.GetComponent<GameObjectComponent>().GameObject;

            // 查找名为 a 的子物体
            // Transform childTransform = obj.transform.Find("a");
            // if (childTransform != null)
            // {
            //     GameObject childObj = childTransform.gameObject;
            //     Debug.Log($"找到子物体 a: {childObj.name}");
            // }
            // else
            // {
            //     Debug.Log("没有找到名为 a 的子物体");
            // }

            // 查找名为 AttackCollidler 的子物体
            Transform attackTransform = obj.transform.Find("AttackCollidler");
            if (attackTransform != null)
            {
                Debug.Log("攻击组件 找到 attackTransform");
                // 获取碰撞组件
                Collider attackCollider = attackTransform.GetComponent<Collider>();
                if (attackCollider != null)
                {
                    Debug.Log("攻击组件 找到 attackCollider");
                    // 设置碰撞器为触发器
                    attackCollider.isTrigger = true;

                    // 获取或添加刚体组件
                    Rigidbody rb = attackTransform.gameObject.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = attackTransform.gameObject.AddComponent<Rigidbody>();
                    }

                    // 配置刚体属性
                    rb.useGravity = false;  // 关闭重力
                    rb.isKinematic = true;  // 设置为运动学刚体，不受物理影响
                                            //rb.constraints = RigidbodyConstraints.FreezeAll;  // 冻结所有运动

                    // 保存碰撞组件引用和单位ID
                    self.AttackCollider = attackCollider;
                    self.unitId = unit.Id;

                    // 添加攻击碰撞1脚本
                    UnitAttack attackScript = attackTransform.gameObject.GetComponent<UnitAttack>();
                    if (attackScript == null)
                    {
                        attackScript = attackTransform.gameObject.AddComponent<UnitAttack>();
                    }
                    attackScript.unitId = unit.Id;

                    // 注册碰撞事件
                    attackScript.SetOnCollisionEnter(self.MyOnCollisionEnter);
                    attackScript.SetOnCollisionStay(self.MyOnCollisionStay);
                    attackScript.SetOnTriggerEnter(self.MyTriggerEnter);
                    attackScript.SetOnTriggerStay(self.MyTriggerStay);
                    attackScript.SetOnTriggerExit(self.MyTriggerExit);
                }
                else
                {
                    Debug.Log("攻击组件 没有找到 attackTransform");
                }
            }
            else
            {
                Debug.Log("攻击组件 没有找到 AttackCollidler");
            }
        }

        [EntitySystem]
        private static void Update(this ET.Client.AttackComponent self)
        {
            // 在Update中更新碰撞体位置
            //UpdateAttackColliderPosition(self,0.5);
        }

        // 更新攻击碰撞体位置的函数
        public static void UpdateAttackColliderPosition(this AttackComponent self, float distance)
        {
            Unit unit = self.GetParent<Unit>();
            if (unit == null) return;

            Transform attackTransform = unit.GetComponent<GameObjectComponent>().GameObject.transform.Find("AttackCollidler");
            if (attackTransform == null) return;

            // 获取单位当前位置
            Vector3 unitPosition = unit.Position;

            // 根据朝向调整碰撞体位置
            switch (unit.Direction)
            {
                case Direction.Front:  // 前
                    attackTransform.localPosition = new Vector3(0, 0.1f, -distance);  // 向前偏移
                    break;
                case Direction.Back:   // 后
                    attackTransform.localPosition = new Vector3(0, 0.1f, distance); // 向后偏移
                    break;
                case Direction.Left:   // 左
                    attackTransform.localPosition = new Vector3(distance, 0.1f, 0); // 向左偏移
                    break;
                case Direction.Right:  // 右
                    attackTransform.localPosition = new Vector3(-distance, 0.1f, 0);  // 向右偏移
                    break;
            }
        }

        public static void changeAttackFlag(this AttackComponent self, int state)
        {
            self.AttackState = state;
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.AttackComponent self)
        {
        }

        //碰撞器相关
        #region 碰撞器相关
        public static void MyOnCollisionEnter(this AttackComponent self, Collision collision)
        {
            Debug.Log($"攻击碰撞1： name:{collision.gameObject.name}, 碰撞点数量:{collision.contacts.Length}");

            // 处理攻击碰撞1
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("攻击到敌人");
                // TODO: 处理攻击伤害等逻辑
            }
        }

        public static void MyOnCollisionStay(this AttackComponent self, Collision collision)
        {
            // 持续碰撞时的处理
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // TODO: 处理持续攻击效果
            }
        }
        #endregion

        #region 触发器相关
        private static void MyTriggerEnter(this AttackComponent self, Collider other)
        {
            // Debug.Log($"攻击触发器进入： name:{other.gameObject.name}, 位置:{other.transform.position}");

            // 获取当前碰撞物体的所有碰撞器
            Collider[] colliders = Physics.OverlapBox(
                other.bounds.center,
                other.bounds.extents,
                other.transform.rotation
            );

            // Debug.Log($"当前区域内的碰撞体数量: {colliders.Length}");

            // switch (other.gameObject.tag)
            // {
            //     case "Enemy":
            //         long unitId = other.gameObject.GetComponent<UnitBehaviour>()?.unitId ?? 0;
            //         if (unitId != 0)
            //         {
            //             Debug.Log($"攻击到敌人，ID: {unitId}");
            //             // 获取场景中的单位组件
            //             UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
            //             if (unitComponent != null)
            //             {
            //                 // 获取并销毁单位
            //                 Unit targetUnit = unitComponent.Get(unitId);
            //                 if (targetUnit != null)
            //                 {
            //                     Debug.Log($"销毁单位: {targetUnit.Id}");
            //                     unitComponent.Remove(unitId);
            //                 }
            //             }
            //         }
            //         break;
            //         
            //     case "Boss":
            //         Debug.Log($"攻击到Boss，ID: {other.gameObject.GetComponent<UnitBehaviour>()?.unitId}");
            //         // TODO: 处理Boss攻击逻辑
            //         break;
            // }
        }

        //碰撞Stay检测
        public static void MyTriggerStay(this AttackComponent self, Collider other)
        {
            // 获取当前区域内的所有碰撞体
            Collider[] colliders = Physics.OverlapBox(
                other.bounds.center,
                other.bounds.extents,
                other.transform.rotation
            );


            // 打印所有碰撞体的名称
            // foreach (var collider in colliders)
            // { 
            //     if (collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
            //     {
            //         Debug.Log($"攻击触发器保持：当前状态 {self.AttackState} 当前区域内有 {colliders.Length} 个碰撞体");
            //
            //         Debug.Log($"碰撞体名称: {collider.gameObject.name}, Tag: {collider.gameObject.tag}");
            //     }
            // }
            
            
            

            if (self.AttackState == 1)
            {
                self.AttackStateChange(0);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
                    {
                        // 获取父对象上的 UnitMonoBehaviour 组件
                        GameObjectEntityRef unitMonoBehaviour = collider.transform.GetComponentInParent<GameObjectEntityRef>();
                        if (unitMonoBehaviour == null)
                            continue;
                        // long unitId = unitMonoBehaviour.UnitId;
                        // if (unitId != 0)
                        // {
                        //     UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
                        //     Unit monsterUnit = unitComponent.Get(unitId);
                        //     
                        //     if (monsterUnit != null && monsterUnit.IsAlive())
                        //     {
                        //         // 处理玩家攻击怪物的伤害
                        //         self.HandlePlayerAttack(monsterUnit);
                        //         
                        //         // // 激活怪物仇恨
                        //         // EnemyBehaviourComponent enemyBehaviourComponent = monsterUnit.GetComponent<EnemyBehaviourComponent>();
                        //         // if (enemyBehaviourComponent != null)
                        //         // {
                        //         //     enemyBehaviourComponent.ChangeAngry();
                        //         // }
                        //     }
                        // }
                    }
                }
            }

            // 处理捡取状态
            bool hasItemUnit = false;
            foreach (var collider in colliders)
            { 
                if (collider.CompareTag("ItemUnit"))
                {
                    hasItemUnit = true;
                    Unit unit = self.unit;
                    PlayerBehaviourComponent behaviourComponent = unit.GetComponent<PlayerBehaviourComponent>();
                    // if (behaviourComponent != null)
                    // {
                    //     Debug.Log("捡触发器 找到碰撞体 pickflag 被打开" );
                    //     behaviourComponent.PickFlag = true;
                    // }
                    // Debug.Log("捡触发器 找到碰撞体 1 " );
                    // 如果处于捡取状态，执行捡取逻辑
                    if (self.PickState == 1)
                    {
                        UnitBehaviour unitBehaviour = collider.transform?.GetComponent<UnitBehaviour>();
                       
                        long unitId = unitBehaviour?.unitId ?? 0;
                       // Debug.Log("捡触发器 找到碰撞体 2 id: "+unitId );
                        if (unitId != 0)
                        {
                            // Debug.Log("捡触发器 找到碰撞体 id: " + unitId);
                            UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
                            Unit targetUnit = unitComponent?.Get(unitId);

                            if (targetUnit != null)
                            {
                                try
                                {
                                    // UnitConfig unitConfig = UnitConfigCategory.Instance.Get(targetUnit.ConfigId);
                                    // C2M_Harvest C2M_harvest = C2M_Harvest.Create();
                                    // ItemInfo itemInfo = ItemInfo.Create();
                                    // itemInfo.ItemConfigId = unitConfig.ItemConfigId;
                                    // C2M_harvest.ItemInfo = itemInfo;
                                    // C2M_harvest.GetTypeState = 4;
                                    //
                                    // // Debug.Log("测试获取资源: id: " + targetUnit.ConfigId);
                                    //
                                    // // 发送捡取请求
                                    // self.Root().GetComponent<ClientSenderComponent>().Send(C2M_harvest);
                                    //
                                    // // 等待服务器响应后再销毁单位
                                    // // TODO: 添加服务器响应处理
                                    // targetUnit.Dispose();
                                    // self.PickState = 0;
                                    // behaviourComponent.PickFlag = false;
                                    // Debug.Log("测试获取资源: 关闭flag： "+self.PickState);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError($"捡取物品失败: {e.Message}");
                                }
                            }
                        }
                    }
                }
            }

            // // 如果没有检测到物品，重置捡取标志
            // if (!hasItemUnit)
            // {
            //     Debug.Log("捡触发器 找到碰撞体 pickflag 关闭" );
            //     Unit unit = self.unit;
            //     PlayerBehaviourComponent behaviourComponent = unit.GetComponent<PlayerBehaviourComponent>();
            //     if (behaviourComponent != null)
            //     {
            //         behaviourComponent.PickFlag = false;
            //     }
            // }
        }

        private static void MyTriggerExit(this AttackComponent self, Collider other)
        {
            Debug.Log($"攻击触发器退出： name:{other.gameObject.name}, 位置:{other.transform.position}");

            // 获取当前区域内的所有碰撞体
            Collider[] colliders = Physics.OverlapBox(
                other.bounds.center,
                other.bounds.extents,
                other.transform.rotation
            );
            
            foreach (var collider in colliders)
            { 
                if (collider.CompareTag("ItemUnit"))
                {
                    Unit unit = self.unit;

                    PlayerBehaviourComponent behaviourComponent = unit.GetComponent<PlayerBehaviourComponent>();

                    behaviourComponent.PickFlag = false;
                }
            }
            
            switch (other.gameObject.tag)
            {
                case "Enemy":
                    Debug.Log("离开敌人攻击范围");
                    break;

                case "Boss":
                    Debug.Log("离开Boss攻击范围");
                    break;
            }
        }
        #endregion


        public static void AttackStateChange(this AttackComponent self, int attackState)
        {
            self.AttackState = attackState;

        }

        public static void PickStateChange(this AttackComponent self, int PickState)
        {
            self.PickState = PickState;

        }

        // 处理玩家攻击怪物的伤害计算
        public static void HandlePlayerAttack(this AttackComponent self, Unit monsterUnit)
        {
            // 防抖机制：避免单次攻击造成多次伤害
            string attackKey = $"PlayerAttack_{monsterUnit.Id}";
            float currentTime = UnityEngine.Time.time;
            
            if (self.LastAttackTime.ContainsKey(attackKey))
            {
                float timeDiff = currentTime - self.LastAttackTime[attackKey];
                if (timeDiff < 0.5f) // 0.5秒防抖间隔
                {
                    return; // 防抖期内，忽略攻击
                }
            }
            
            self.LastAttackTime[attackKey] = currentTime;
            
            // 获取玩家攻击力
            Unit playerUnit = self.GetParent<Unit>();
            NumericComponent playerNumeric = playerUnit.GetComponent<NumericComponent>();
            int attackPower = (int)playerNumeric[NumericType.Damage]+self.AttackBuff;
            
            // 获取怪物血量组件
            NumericComponent monsterNumeric = monsterUnit.GetComponent<NumericComponent>();
            long currentHp = monsterNumeric[NumericType.Hp];
            
            // 统一扣血入口（玩家打怪直接扣血，不走振刀）
            int power = attackPower == 0 ? 10 : attackPower;
            EventSystem.Instance.PublishAsync(monsterUnit.Root(), new AttackUnitStart()
            {
                AttackUnit = playerUnit,
                TargetUnit = monsterUnit,
                Damage = power
            }).NoContext();
            // 检查怪物是否死亡
            // 死亡逻辑由 DamageHelper 内部统一处理（玩家是 ChangeToDie，怪物这里可拓展）
            
            //清空战意条buff加成
            self.AttackBuff = 0;
            EventSystem.Instance.Publish(self.Root(), new ZYAttackInitEvent() {Unit = self.unit , State = 0});
            
        }
    }

    // 战意准备就绪事件处理器
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Client.AttackComponent))]
    public class ZhanYi_AttackEvent_Handler : AEvent<Scene, ZYAttackEvent>
    {
        protected override async ETTask Run(Scene scene, ZYAttackEvent args)
        {
            //Debug.Log("战意事件触发");
            Unit unit = args.Unit;

            // 只处理拥有HuangFengDaShengAttackComponent的Unit
            AttackComponent attackComponent = unit.GetComponent<AttackComponent>();
            if (attackComponent == null)
            {
                Debug.Log("战意事件触发 找不到组件");
                return;
            }
            attackComponent.AttackBuff = args.State * 10; // 每个等级增加5点攻击力
            //Debug.Log("战意事件触发：能量值: "+ args.State + "增加了攻击力： "+attackComponent.AttackBuff); 

            await ETTask.CompletedTask;
        }
    }
    
    // 玩家攻击状态重置定时器
    [Invoke(TimerInvokeType.PlayerAttackCdTimer)]
    [FriendOfAttribute(typeof(ET.Client.AttackComponent))]
    public class PlayerAttackComponent_AttackTimer : ATimer<AttackComponent>
    {
        protected override void Run(AttackComponent self)
        {
            if (self == null || self.IsDisposed)
                return;

            // 重置玩家攻击状态
            self.AttackStateChange(0);
            
            // 清除防抖记录，确保下次攻击能正常扣血
            self.LastAttackTime.Clear();
            
            Debug.Log("玩家攻击状态重置完成，防抖记录已清除");
        }
    }
}