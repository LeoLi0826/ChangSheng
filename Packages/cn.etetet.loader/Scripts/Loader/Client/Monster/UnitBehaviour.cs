using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Client
{
    public class UnitBehaviour : MonoBehaviour
    {
        
        private Transform unit;
        
        public float speed = 10.0f;
        private Rigidbody2D rigidbody;

        public long unitId;
        //private Animator animator;
        
        private float inputX,inputY;
        private float stopX,stopY;
        public float SpeedZ = 1.5f;
        
        //碰撞
        public UnityAction<Collision> action;

        public UnityAction<Collider> actionEenter;
        public UnityAction<Collider> actionStay;
        public UnityAction<Collider> actionEixt;

        //private Vector3 offset;
        
        // Start is called before the first frame update
        void Start()
        {
            unit = this.transform;
            //rigidbody = GetComponent<Rigidbody2D>();
            
            // 查找名为 AttackCollidler 的子物体
            // Transform unitTransform = transform.Find("unitCollidler");
            // if (unitTransform != null)
            // {
            //     // 获取碰撞组件
            //     Collider unitCollider = unitTransform.GetComponent<Collider>();
            //     if (unitCollider != null)
            //     {
            //         Debug.Log($"攻击组件 找到 attackCollider: {gameObject.name}");
            //         // 设置碰撞器为触发器
            //         unitCollider.isTrigger = true;
            //         unitCollider.tag = "Enemy";
            //         // 获取或添加刚体组件
            //         Rigidbody rb = unitTransform.gameObject.GetComponent<Rigidbody>();
            //         if (rb == null)
            //         {
            //             rb = unitTransform.gameObject.AddComponent<Rigidbody>();
            //         }
            //
            //         // 配置刚体属性
            //         rb.useGravity = false; // 关闭重力
            //         rb.isKinematic = true; // 设置为运动学刚体，不受物理影响
            //     }
            //     else
            //     {
            //         Debug.Log($"攻击组件 未找到 Collider: {gameObject.name}");
            //     }
            // }
            // else
            // {
            //     Debug.Log($"攻击组件 未找到 AttackCollidler: {gameObject.name}");
            // }
        }

         void Update()
        {
            // unit.rotation = Camera.main.transform.rotation;
            // //Camera.main.transform.position = this.transform.position + offset;
            // switch (this.unit.transform.tag)
            // {
            //     case "Energy":
            //         FallingZ();
            //         break;
            //     
            //     
            // }
        }
        //
        public void FallingZ()
        {
            unit.rotation = Quaternion.Euler(0, 0, 0);
            if (transform.position.z < 0)
            {
                transform.Translate(0, 0, SpeedZ * Time.deltaTime);
            }
            unit.rotation = Camera.main.transform.rotation;
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
