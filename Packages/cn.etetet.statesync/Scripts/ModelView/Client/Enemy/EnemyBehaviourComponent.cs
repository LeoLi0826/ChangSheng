using System;
using System.Collections.Generic;
using ET.Client;
using Unity.Mathematics;
using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class EnemyBehaviourComponent : Entity, IAwake, IDestroy ,IUpdate
    {
        public EntityRef<Unit> MyUnit;
        public bool IsMoving { get; set; }
        //攻击标记
        public bool AttackFlag;
        //死亡标记
        public bool DieFlag;
        public bool AngryFlag;
        
        public PlayerBehaviour playerBehaviour;
        public EntityRef<AttackComponent> attackComponent;
        public EntityRef<TimerComponent> timer;
        public long timerID;

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