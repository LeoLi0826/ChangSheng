using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ET
{
    //可能比较适合去碰到物品 去拣取等操作
    public class MonsterCollector : MonoBehaviour
    {
        private Rigidbody rigidbody1;
        
        public long unitId;

        private Transform unit;
        // 碰撞
        public UnityAction<Collision> action;

        public UnityAction<Collider> actionEenter;
        public UnityAction<Collider> actionStay;
        public UnityAction<Collider> actionEixt;

       
        // Start is called before the first frame update
        void Start()
        {
            this.rigidbody1 = GetComponent<Rigidbody>();
            if (this.rigidbody1 == null)
            {
                Debug.LogError("攻击 刚体没有找到刚体");
            }
            else
            {
                Debug.Log("找到攻击 刚体 存在");
            }
            //animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
           
             // 面向摄像机 转向
            // unit.rotation = Camera.main.transform.rotation;
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