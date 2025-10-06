using System;
using System.Collections.Generic;
using Unity.Mathematics;
using ET.Client;
using UnityEngine;

namespace ET
{
    [EntitySystemOf(typeof(EnemyBehaviourComponent))]
    [FriendOf(typeof(EnemyBehaviourComponent))]
    public static partial class EnemyBehaviourComponentSystem
    {

        [EntitySystem]
        private static void Destroy(this EnemyBehaviourComponent self)
        {
            Debug.Log($"EnemyBehaviourComponent销毁，清理Timer资源");
            TimerComponent timer = self.Root().GetComponent<TimerComponent>();
            timer.Remove(ref self.timerID);
        }

        [EntitySystem]
        private static void Awake(this EnemyBehaviourComponent self)
        {
//            Debug.Log("敌人移动组件初始化！");
            self.speed = 10f;
            self.IsMoving = true;
            self.AttackFlag = false;
            self.DieFlag = false;

            self.MyUnit = self.GetParent<Unit>();
            Unit unit = self.MyUnit;
            self.player = unit.GetComponent<GameObjectComponent>().GameObject;
            //这里和玩家不一样
            self.rigidbody = self.player.GetComponent<Rigidbody>();
            // self.attackComponent = unit.GetComponent<AttackComponent>();

             self.timer = self.Root().GetComponent<TimerComponent>();
            
            
        }

        [EntitySystem]
        private static void Update(this ET.EnemyBehaviourComponent self)
        {
            self.Move();
            //self.UpdateMove();
        }

        public static void Move(this EnemyBehaviourComponent self)
        {
            //死亡时 
            if (self.DieFlag == true)
            {
                return;
            }

            Unit unit = self.MyUnit;
            if (self.IsMoving == true)
            {
                #region 移动逻辑 得配合A*寻路 暂时注释

                // // 获取输入
                // self.inputX = Input.GetAxis("Horizontal");
                // self.inputZ = Input.GetAxis("Vertical");
                //
                // self.input = new Vector3(self.inputX, 0, self.inputZ).normalized;
                //
                // if (self.input != Vector3.zero)
                // {
                //     // 使用刚体移动，而不是直接修改位置
                //     self.rigidbody.velocity = self.input * self.speed;
                //     self.stopX = self.inputX;
                //     self.stopZ = self.inputZ;
                //
                //     // 设置朝向
                //     float angle = Mathf.Atan2(self.inputX, self.inputZ) * Mathf.Rad2Deg;
                //     if (angle >= -45 && angle < 45)
                //     {
                //         unit.Direction = Direction.Back;
                //     }
                //     else if (angle >= 45 && angle < 135)
                //     {
                //         unit.Direction = Direction.Left;
                //     }
                //     else if (angle >= -135 && angle < -45)
                //     {
                //         unit.Direction = Direction.Right;
                //     }
                //     else
                //     {
                //         unit.Direction = Direction.Front;
                //     }
                //
                //     unit.SetUnitActionType(UnitActionType.Walk);
                //     self.attackComponent.UpdateAttackColliderPosition(0.5f);


                #endregion

                // }
                // else
                {
                    // unit.Direction = Direction.Front;
                    // //self.rigidbody.velocity = Vector3.zero;
                    //
                    // unit.SetAIUnitActionType(UnitActionType.Idle);

                }
            }
        }

        public static void ChangeAngry(this EnemyBehaviourComponent self)
        {
            Debug.Log("怪物行为攻击： flag:" +self.AngryFlag);
            TimerComponent timer = self.timer;
            if (self.AngryFlag == false)
            { 
                //启动攻击计时器
                self.AngryFlag = true; 
                
                self.timerID = timer.NewRepeatedTimer(2000, TimerInvokeType.EnemyAngry, self);
                
            }
            // else
            // {
            //     self.AngryFlag = false
            //     timer.Remove(ref  self.timerID);
            //
            // }

            
            //self.AngryFlag = !self.AngryFlag;
            
                //距离超过10 结束仇恨
            
        }
        
        
        public static void StartMove(this EnemyBehaviourComponent self)
        {
            Debug.Log("敌人开始移动！");
            self.IsMoving = true;
        }

        public static void StopMove(this EnemyBehaviourComponent self)
        {
            Debug.Log("敌人停止移动！");
            //self.rigidbody.velocity = Vector3.zero;
            self.IsMoving = false;
        }

        public static void ChangeToDie(this EnemyBehaviourComponent self)
        {
            Unit unit = self.MyUnit;
            unit.SetUnitActionType(UnitActionType.Die, false);
            self.DieFlag = true;
            
            // 清理怪物相关定时器
            if (self.timerID != 0)
            {
                TimerComponent timer = self.timer;
                timer?.Remove(ref self.timerID);
                Debug.Log("清理怪物仇恨定时器");
            }
            
            // 停止移动
            self.StopMove();
            
            // 隐藏血条UI（通过设置HP为0，血条系统会自动隐藏）
            Debug.Log($"怪物 {unit.Id} 死亡，开始清理相关资源");
        }

    }
}