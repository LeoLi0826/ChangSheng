using System;
using System.Collections.Generic;
using Unity.Mathematics;
using ET.Client;
using UnityEngine;

namespace ET
{
    [EntitySystemOf(typeof(PlayerBehaviourComponent))]
    [FriendOf(typeof(PlayerBehaviourComponent))]
    public static partial class PlayerBehaviourComponentSystem
    {

        [EntitySystem]
        private static void Destroy(this PlayerBehaviourComponent self)
        {
            
                
            
        }

        [EntitySystem]
        private static void Awake(this PlayerBehaviourComponent self)
        {

            Debug.Log("玩家移动组件初始化1 ！");
            //self.MyUnit = self.GetParent<Unit>();
            
            self.IsMoving = true;
            self.AttackFlag = false;
            self.DieFlag = false;
            self.PickFlag = false;
            //这个是玩家的朝向相关的组件
           
            self.playerBehaviour = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<PlayerBehaviour>();
            self.speed = self.playerBehaviour.speed;
            self.MyUnit = self.GetParent<Unit>();
            Unit unit = self.MyUnit;
            //Debug.Log("找到玩家组件物体111 ！！！unitID:"+unit.Id);
            self.player =  unit.GetComponent<GameObjectComponent>().GameObject;
            self.rigidbody = self.player.GetComponent<Rigidbody>();
            self.attackComponent = unit.GetComponent<AttackComponent>();


        }
        
        [EntitySystem]
        private static void Update(this ET.PlayerBehaviourComponent self)
        {
            Unit unit = UnitHelper.GetMyUnitFromClientScene(self.Root());
            // 简化版振刀：按下C直接开启100ms的招架窗口
            if (Input.GetKeyDown(KeyCode.C))
            {
                self.IsParryActive = true;
                self.ParryEndTimestampMs = TimeInfo.Instance.ClientNow() + 1000;
                self.StopMove();
                unit.SetUnitActionType(UnitActionType.Zhaojia, false);
            }

            // 到时自动关闭
            if (self.IsParryActive && TimeInfo.Instance.ClientNow() >= self.ParryEndTimestampMs)
            {
                self.IsParryActive = false;
            }
            self.Move();
            //self.UpdateMove();
        }

        public static void Move(this PlayerBehaviourComponent self)
        {
            //死亡时 
            if (self.DieFlag == true)
            {
                return;
            }
            
            if(self.speed != self.playerBehaviour.speed)
            {
                self.speed = self.playerBehaviour.speed;
            }
            
            Unit unit = self.MyUnit;
            if (self.IsMoving == true)
            {
                
                
                // 获取输入
                self.inputX = Input.GetAxis("Horizontal"); // 获取水平输入（X 轴）
                self.inputZ = Input.GetAxis("Vertical");   // 获取垂直输入（Z 轴）

                // 将输入转换为 3D 向量，并忽略 Y 轴
                self.input = new Vector3(self.inputX, 0, self.inputZ).normalized;
                
                // 检查是否有输入
                if (self.input != Vector3.zero)
                {
                    // 使用刚体移动，而不是直接修改位置
                    self.rigidbody.linearVelocity = self.input * self.speed;
                
                    // 记录停止时的输入
                    self.stopX = self.inputX;
                    self.stopZ = self.inputZ;
                
                    // 根据输入设置朝向
                    float angle = Mathf.Atan2(self.inputX, self.inputZ) * Mathf.Rad2Deg;
                    if (angle >= -45 && angle < 45)
                    {
                        unit.Direction = Direction.Back;  // 向前移动时，角色背面朝向
                    }
                    else if (angle >= 45 && angle < 135)
                    {
                        unit.Direction = Direction.Left;  // 向右移动时，角色左侧朝向
                    }
                    else if (angle >= -135 && angle < -45)
                    {
                        unit.Direction = Direction.Right; // 向左移动时，角色右侧朝向
                    }
                    else
                    {
                        unit.Direction = Direction.Front; // 向后移动时，角色正面朝向
                    }

                    unit.SetUnitActionType(UnitActionType.Walk);

                    AttackComponent attackComponent = self.attackComponent;
                    attackComponent.UpdateAttackColliderPosition(0.5f);
                }
                else
                {
                    // 没有输入时，停止移动
                    self.rigidbody.linearVelocity = Vector3.zero;
                    //unit.Direction = Direction.Front; // 向后移动时，角色正面朝向
                    unit.SetUnitActionType(UnitActionType.Idle);
                    // unit.SetUnitActionType(UnitActionType.Walk);
                    // 如果需要动画，可以取消注释
                    // animator.SetBool("IsMoving", false);
                }
            }
           
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                self.StopMove();
                unit.SetUnitActionType(UnitActionType.Attack, false);
                AttackComponent attackComponent = self.attackComponent;
                // 确保攻击碰撞体在正确位置
                attackComponent.UpdateAttackColliderPosition(0.5f);
                
                //开启攻击开关
                attackComponent.AttackStateChange(1);
                
                
            }
            

            
            // 按T键测试本地存储：增加金币并保存
            if (Input.GetKeyDown(KeyCode.T))
            {
                NumericComponent numeric = unit.GetComponent<NumericComponent>();
                if (numeric != null)
                {
                    long currentGold = numeric.GetAsLong(NumericType.Gold);
                    numeric.Set(NumericType.Gold, currentGold + 100);
                    Debug.Log($"[本地存储测试] 增加金币100，当前金币: {numeric.GetAsLong(NumericType.Gold)}");
                }
            }

            if (self.PickFlag == true)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    Debug.Log("捡起1:");
                    //如果碰到可以捡起物品的地方 才可以执行下一个动作
                    //if()
                    self.StopMove();
                    unit.SetUnitActionType(UnitActionType.Pick, false);
            
                    unit.Direction = Direction.Front;
                    AttackComponent attackComponent = self.attackComponent;
                    // 确保攻击碰撞体在正确位置
                    attackComponent.UpdateAttackColliderPosition(0.5f);
                
                    //开启捡起开关
                    //attackComponent.PickStateChange(1);
                
            
            
                    // attackComponent.AttackStateChange(1);
                    //
                    // // 确保攻击碰撞体在正确位置
                    // attackComponent.UpdateAttackColliderPosition(0.5f);
                }
            }
           
            // 面向摄像机 转向
           
            //self.player.transform.rotation = Camera.main.transform.rotation;
        }

        public static void MoveSfx(this PlayerBehaviourComponent self)
        {
            // if ()
            //     ;
            // AudioSourceHelper.PlaySfx(self.Root(), AudioClipType.ButtonClick);
        }

        //旧版
        public static void UpdateMove(this PlayerBehaviourComponent self)
        {
            //Log.Debug("进入 PlayerBehaviourComponentSystem的移动1！！！flag: " + !self.IsMoving);
            
            // if (player == null)
            // {
            //     Debug.Log("找不到玩家组件物体！！！");
            // }
            // else
            // {
            //     Debug.Log("找到玩家组件物体！！！");
            // }
            
            // if (!self.IsMoving)
            //     return;
            //
            // Unit unit = self.MyUnit;
            // //Debug.Log("找到玩家组件物体111 ！！！unitID:"+unit.Id);
            // self.player =  unit.GetComponent<GameObjectComponent>().GameObject;
            //
            // float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            // speed = speed == 0 ? 3 : speed;
            // speed =1;
            //
            // //这段代码计算了单位在每个逻辑帧中的位置增量。具体来说，它使用单位的移动方向（unit.MoveDir）和速度（speed）
            // //，并将其除以逻辑帧的定义（DefineCore.LogicFrame）来得到位置增量（deltaPos）。
            // //我估计这个是导致玩家移动卡顿的关键代码。
            // float3 deltaPos = unit.MoveDir * speed / DefineCore.LogicFrame;
            // Log.Debug("进入 移动2！！！moveDir: "+ unit.MoveDir + " unit.Position:" +unit.Position+"deltaPos: "+deltaPos);
            //
            // Vector2 position = self.player.transform.position;
            // //Vector3 right = self.UnitFacing.transform.right;
            // //Vector3 forward = self.UnitFacing.transform.forward;
            // position.x = position.x + deltaPos.x;//*right.x;
            // position.y = position.y + deltaPos.y;//*forward.y;
            
            
            // player.transform.position = position;
            // self.playerBehaviour.inputMove(position);
            
        }

        public static void StartMove(this PlayerBehaviourComponent self)
        {
            Log.Debug("我可以 移动了！！");
            self.IsMoving = true;
        }

        public static void StopMove(this PlayerBehaviourComponent self)
        {
            Log.Debug("我可以 不移动了！！");
            self.rigidbody.linearVelocity = Vector3.zero;
            self.IsMoving = false;
        }
       
        public static void ChangeToDie(this PlayerBehaviourComponent self)
        {
            Unit unit=self.MyUnit;
            unit.SetUnitActionType(UnitActionType.Die,false);
            self.DieFlag = true;
            
        }
    }
}