using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ET
{
    public class MonsterBehaviour : MonoBehaviour
    {
        public float speed = 10.0f; // 移动速度
        private Rigidbody rigidbody1;

        //private Animator animator;

        public float inputX, inputZ; // 只保留 X 和 Z 轴输入
        private Vector3 input;
        private float stopX, stopZ; // 只记录 X 和 Z 轴停止时的输入

        public long unitId;

        private Transform unit;
        // 碰撞
        public UnityAction<Collision> action;

        public UnityAction<Collider> actionEenter;
        public UnityAction<Collider> actionStay;
        public UnityAction<Collider> actionEixt;

        public bool isMove;
        // Start is called before the first frame update
        void Start()
        {
            isMove = true;
            unit = GameObject.Find("img").transform;
            this.rigidbody1 = GetComponent<Rigidbody>();
            if (this.rigidbody1 == null)
            {
                Debug.LogError("没有找到刚体");
            }
            else
            {
                Debug.Log("找到 刚体 存在");
            }
            //animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            
             // 面向摄像机 转向
             unit.rotation = Camera.main.transform.rotation;
        }

        public void inputMove(Vector3 position)
        {
            // 计算移动方向
            Vector3 direction = (position - transform.position).normalized;
            
            // 使用刚体移动
            rigidbody1.linearVelocity = direction * speed;
            
            // 记录输入
            inputX = direction.x;
            inputZ = direction.z;
            input = new Vector3(inputX, 0, inputZ).normalized;
        }

        // 实物碰撞 碰撞器  
        #region 实物碰撞

        public void SetOnCollisionEnter(UnityAction<Collision> action)
        {
            this.action = action;
        }

        public void SetOnCollisionStay(UnityAction<Collision> action)
        {
            this.action = action;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Log.Debug("我碰到了： " + collision.gameObject.name);
            this.action?.Invoke(collision);
        }

        #endregion

        // 碰撞检测 Trigger 触发器
        #region 碰撞检测 Trigger
        public void SetOnTriggerEnter(UnityAction<Collider> action)
        {
            this.actionEenter = action;
        }
        private void OnTriggerEnter(Collider other)
        {
            this.actionEenter?.Invoke(other);
        }

        public void SetOnTriggerStay(UnityAction<Collider> action)
        {
            this.actionStay = action;
        }
        private void OnTriggerStay(Collider other)
        {
            this.actionStay?.Invoke(other);
        }

        public void SetOnTriggerExit(UnityAction<Collider> action)
        {
            this.actionEixt = action;
        }
        public void OnTriggerExit(Collider other)
        {
            this.actionEixt?.Invoke(other);
        }

        #endregion
    }
}