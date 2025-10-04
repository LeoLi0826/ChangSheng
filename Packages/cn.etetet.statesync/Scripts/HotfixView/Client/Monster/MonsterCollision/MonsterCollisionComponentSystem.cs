using System;
using UnityEngine;
using ET.Client;

namespace ET.Client
{
    [EntitySystemOf(typeof(MonsterCollisionComponent))]
    [FriendOf(typeof(MonsterCollisionComponent))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public static partial class MonsterCollisionComponentSystem
    {
        [EntitySystem]
        private static void Awake(this MonsterCollisionComponent self)
        {

            Unit unit = self.GetParent<Unit>();
            self.unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            //Log.Debug("怪物碰撞委托注册0 ：unit.ConfigId： " + unit.ConfigId + " Type: " + self.unitConfig.Type);



            BoxCollider boxCollider = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<BoxCollider>();

            GameObject obj = unit.GetComponent<GameObjectComponent>().GameObject;

            // 尝试多种方式查找子物体
            Transform unitCollidler = null;

            // 首先查找Root子物体
            Transform rootTransform = obj.transform.Find("Root");
            if (rootTransform != null)
            {
                // 在Root下查找unitCollidler
                unitCollidler = rootTransform.Find("unitCollider");

            }
            else
            {
                Log.Debug("怪物碰撞委托04 - 未找到Root子物体，尝试直接查找");
            }

            //Log.Debug("怪物碰撞委托09 - 查找结果: " + (unitCollidler != null ? "成功" : "失败"));
            if (unitCollidler != null)
            {
                //Log.Debug($"怪物碰撞委托10 - 找到子物体: {unitCollidler.name}");
                // 获取碰撞组件
                Collider attackCollider = unitCollidler.GetComponent<Collider>();
                if (attackCollider != null)
                {
                    //Debug.Log("怪物攻击组件 找到 attackCollider");
                    // 设置碰撞器为触发器
                    attackCollider.isTrigger = true;

                    // 获取或添加刚体组件
                    Rigidbody rb = unitCollidler.gameObject.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = unitCollidler.gameObject.AddComponent<Rigidbody>();
                    }

                    // 配置刚体属性
                    rb.useGravity = false; // 关闭重力
                    rb.isKinematic = true; // 设置为运动学刚体，不受物理影响
                    //rb.constraints = RigidbodyConstraints.FreezeAll;  // 冻结所有运动

                    // 保存碰撞组件引用和单位ID
                    self.AttackCollider = attackCollider;
                    self.unitId = unit.Id;

                    // 添加攻击碰撞1脚本
                    MonsterCollector attackScript = unitCollidler.gameObject.GetComponent<MonsterCollector>();
                    if (attackScript == null)
                    {
                        attackScript = unitCollidler.gameObject.AddComponent<MonsterCollector>();
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
                    Debug.Log("怪物攻击组件 没有找到 unitCollidler");
                }


                // if (self.unitConfig.Type == 2)
                // {
                //     Log.Debug("怪物碰撞委托注册1 - Type: " + self.unitConfig.Type);
                //     self.MonsterCollector = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<MonsterCollector>();
                //     
                //     if (self.MonsterCollector != null)
                //     {
                //         Log.Debug("怪物碰撞委托注册2 - 成功获取MonsterCollector");
                //         self.MonsterCollector.SetOnCollisionEnter(self.MyOnCollisionEnter);
                //         self.MonsterCollector.SetOnCollisionStay(self.MyOnCollisionStay);
                //         
                //         //触发器
                //         self.MonsterCollector.SetOnTriggerEnter(self.MyTriggerEnter);
                //         self.MonsterCollector.SetOnTriggerExit(self.MyTriggerExit);
                //         Log.Debug("怪物碰撞委托注册3 - 触发器委托注册完成");
                //     }
                //     else
                //     {
                //         Log.Error("怪物碰撞委托注册失败 - MonsterCollector为空");
                //     }
                // }
                // else
                // {
                //     Log.Debug("怪物碰撞委托注册跳过 - Type不符合条件: " + self.unitConfig.Type);
                // }

            }
            else
            {
                Debug.Log("怪物攻击组件  attackCollider 没有！！！");
            }
        }

        [EntitySystem]
        private static void Update(this MonsterCollisionComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this MonsterCollisionComponent self)
        {

        }


        //怪物碰撞器相关
        #region 怪物碰撞器相关
        public static void MyOnCollisionEnter(this MonsterCollisionComponent self, Collision collision)
        {
            Debug.Log("ET怪物碰撞： name:" + collision.gameObject.name);

            // // 处理边界墙怪物碰撞
            // if (collision.gameObject.CompareTag("Collider"))
            // {
            //     Debug.Log("碰到边界墙，停止移动");
            //     self.MonsterCollector.isMove = false;
            // }
        }

        public static void MyOnCollisionStay(this MonsterCollisionComponent self, Collision collision)
        {
            // 持续怪物碰撞时保持停止状态
            // if (collision.gameObject.CompareTag("Collider"))
            // {
            //     self.MonsterCollector.isMove = false;
            // }
        }

        #endregion


        #region 触发器相关
        private static void MyTriggerEnter(this MonsterCollisionComponent self, Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Unit unit = self.GetParent<Unit>();
                MonsterMoveComponent MonsterMove = (MonsterMoveComponent)self.MonsterMove;

                if (MonsterMove == null)
                {
                    self.MonsterMove = unit.GetComponent<MonsterMoveComponent>();
                    MonsterMove = self.MonsterMove;

                    if (MonsterMove == null)
                    {
                        Debug.Log("触发测试1：找不到 移动组件！");
                        return;
                    }
                    else
                    {
                        Debug.Log("触发测试1：找到 移动组件！");
                    }
                }

                // 检查是否有攻击组件
                if (!unit.HasAttackComponent())
                {
                    Debug.LogWarning($"怪物 {unit.Id} 没有任何攻击组件！类型: {unit.GetAttackComponentType()}");
                    return;
                }

                // 使用统一扩展方法设置攻击标志
                MonsterMove.AttackFlag = true;
                unit.SetAttackFlag(true);

                Debug.Log($"怪物触发器 进入： name: {other.gameObject.name}, 攻击标记: {MonsterMove.AttackFlag}, 组件类型: {unit.GetAttackComponentType()}");
            }
            else
            {
                return;
            }

            // switch (other.gameObject.tag)
            // {
            //     //钓鱼
            //     case "Lake":
            //         Debug.Log("怪物触发器 进入2：  Lake");
            //
            //         //unitId = other.gameObject.GetComponent<MonsterCollector>().unitId;
            //
            //         self.Fiber().UIEvent(new GetResource() {TagType = 2} ).Coroutine();
            //         break;
            //     //资源类
            //     case "Resource":
            //         Debug.Log("怪物触发器 进入3：  id:"+other.gameObject.GetComponent<MonsterCollector>().unitId);
            //
            //          unitId = other.gameObject.GetComponent<MonsterCollector>().unitId;
            //
            //         self.Fiber().UIEvent(new GetResource() {UnitId = unitId ,TagType = 3} ).Coroutine();
            //         break;
            //     //能量类
            //     case "Energy":
            //         Debug.Log("怪物触发器 进入4：  id:"+other.gameObject.GetComponent<MonsterCollector>().unitId);
            //
            //          unitId = other.gameObject.GetComponent<MonsterCollector>().unitId;
            //
            //         self.Fiber().UIEvent(new GetResource() {UnitId = unitId,TagType = 4} ).Coroutine();
            //         break;
            // }
        }

        //暂时不需要
        public static void MyTriggerStay(this MonsterCollisionComponent self, Collider other)
        {
            //Debug.Log("怪物触发器 保持  name:" +other.gameObject.name+" id:"+other.gameObject.GetComponent<MonsterCollector>().unitId);

            // long unitId = other.gameObject.GetComponent<MonsterCollector>().unitId;
            //
            // UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
            //
            // Unit unit =unitComponent.Get(unitId);
            //
            // UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            //
            //
            // if(unitConfig == null)
            // {
            //     Debug.Log("unit为空");
            // }
            // else
            // {
            //     Debug.Log("unit存在 name:" + unitConfig.Id + " Name: " + unitConfig.Name + " PrefabName: " + unitConfig.PrefabName);
            // }
        }

        private static void MyTriggerExit(this MonsterCollisionComponent self, Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Unit unit = self.GetParent<Unit>();
                Debug.Log($"怪物触发器 退出： name: {other.gameObject.name}, 组件类型: {unit.GetAttackComponentType()}");

                MonsterMoveComponent MonsterMove = self.MonsterMove;
                if (MonsterMove != null)
                {
                    MonsterMove.AttackFlag = false;
                }

                // 使用统一扩展方法设置攻击标志
                unit.SetAttackFlag(false);

                // 检查是否是黄风大圣且正在执行蓄力攻击
                HuangFengDaShengAttackComponent bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
                if (bossAttack != null && bossAttack.IsChargeAttacking)
                {
                    Debug.Log($"黄风大圣[{unit.Id}]正在执行蓄力攻击，忽略碰撞体退出");
                    return;
                }

                // 检查是否是黄风大圣且处于虚弱状态
                if (bossAttack != null && bossAttack.IsWeakened)
                {
                    Debug.Log($"黄风大圣[{unit.Id}]处于虚弱状态，忽略碰撞体退出");
                    return;
                }

                // 检查是否是黄风大圣且正在释放大招
                if (bossAttack != null && bossAttack.IsUltimateAttacking)
                {
                    Debug.Log($"黄风大圣[{unit.Id}]正在释放大招，忽略碰撞体退出");
                    return;
                }

                // 立即停止攻击动画，防止延迟攻击（只对普通攻击生效）
                if (unit.GetMonsterActionType() == MonsterActionType.Attack)
                {
                    unit.SetAIUnitActionType(UnitActionType.Idle);
                    unit.SetMonsterActionType(MonsterActionType.Walk);
                    Debug.Log($"怪物[{unit.Id}]离开攻击范围，立即停止攻击动画");
                }
            }
            else
            {
                return;
            }

            // 处理边界怪物碰撞退出
            // if (other.gameObject.CompareTag("Player"))
            // {
            //     Debug.Log("离开边界");
            //     return;
            // }
            //other.gameObject.tag

        }

        #endregion


    }
}