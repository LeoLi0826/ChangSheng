using System;
using System.Collections.Generic;
using ET.Client;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class PlayerBehaviourComponent : Entity, IAwake, IDestroy ,IUpdate
    {
        public EntityRef<Unit> MyUnit;
        public bool IsMoving { get; set; }
        //攻击标记
        public bool AttackFlag;
        //死亡标记
        public bool DieFlag;

        public bool PickFlag;
        
        // 振刀系统相关
        public bool ZhendaoWindowOpen;  // 振刀窗口是否开启
        public float LastZhendaoTime;   // 上次振刀时间
        public float ZhendaoCooldown = 2f; // 振刀冷却时间
        
        // 简化版振刀：按下C开启，持续100ms后自动关闭
        public bool IsParryActive;
        public long ParryEndTimestampMs;
        
        public PlayerBehaviour playerBehaviour;
        public EntityRef<AttackComponent> attackComponent;

        public GameObject player;
        
        public Rigidbody rigidbody;

        //速度
        public float speed;
        
        //输入
        public Vector3 input;
        public float inputX;
        public float inputZ;
                
        public float stopX;
        public float stopZ;

       
    }
}