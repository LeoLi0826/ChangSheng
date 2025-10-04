using System;

using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(PlayerCollisionComponent))]
    [FriendOf(typeof(PlayerCollisionComponent))]
    public static partial class PlayerCollisionComponentSystem
    {
        [EntitySystem]
        private static void Awake(this PlayerCollisionComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            self.unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            BoxCollider boxCollider = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<BoxCollider>();

            if (self.unitConfig.PrefabName == "LeoPlayer")
            {
                Log.Debug("碰撞委托注册1");
                self.playerBehaviour = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<PlayerBehaviour>();
                
                self.playerBehaviour.SetOnCollisionEnter(self.MyOnCollisionEnter);
                self.playerBehaviour.SetOnCollisionStay(self.MyOnCollisionStay);
                
                //触发器
                self.playerBehaviour.SetOnTriggerEnter(self.MyTriggerEnter);
                self.playerBehaviour.SetOnTriggerExit(self.MyTriggerExit);
            }
        }
        [EntitySystem]
        private static void Update(this PlayerCollisionComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this PlayerCollisionComponent self)
        {

        }
        
        
        //碰撞器相关
        #region 碰撞器相关
        public static void MyOnCollisionEnter(this PlayerCollisionComponent self,Collision collision)
        {
            Debug.Log("ET碰撞： name:" + collision.gameObject.name);
            
            // 处理边界墙碰撞
            if (collision.gameObject.CompareTag("Collider"))
            {
                Debug.Log("碰到边界墙，停止移动");
                self.playerBehaviour.isMove = false;
            }
        }

        public static void MyOnCollisionStay(this PlayerCollisionComponent self, Collision collision)
        {
            // 持续碰撞时保持停止状态
            if (collision.gameObject.CompareTag("Collider"))
            {
                self.playerBehaviour.isMove = false;
            }
        }

        #endregion


        #region 触发器相关
        private static void MyTriggerEnter(this PlayerCollisionComponent self, Collider other)
        {
            //Debug.Log("ET触发器 进入1： name:" + other.gameObject.name);
            long unitId;

            switch (other.gameObject.tag)
            {
                //钓鱼
                case "Lake":
                    Debug.Log("ET触发器 进入2：  Lake");

                    //unitId = other.gameObject.GetComponent<UnitBehaviour>().unitId;

                    self.DynamicEvent(new GetResource() {TagType = 2} ).NoContext();
                    break;
                //资源类
                case "Resource":
                    Debug.Log("ET触发器 进入3：  id:"+other.gameObject.GetComponent<UnitBehaviour>().unitId);

                     unitId = other.gameObject.GetComponent<UnitBehaviour>().unitId;

                    self.DynamicEvent(new GetResource() {UnitId = unitId ,TagType = 3} ).NoContext();
                    break;
                //能量类
                case "Energy":
                    Debug.Log("ET触发器 进入4：  id:"+other.gameObject.GetComponent<UnitBehaviour>().unitId);

                     unitId = other.gameObject.GetComponent<UnitBehaviour>().unitId;

                    self.DynamicEvent(new GetResource() {UnitId = unitId,TagType = 4} ).NoContext();
                    break;
            }
        }
        
        //暂时不需要
        public static void MyTriggerStay(this PlayerCollisionComponent self, Collider other)
        {
            //Debug.Log("ET触发器 保持  name:" +other.gameObject.name+" id:"+other.gameObject.GetComponent<UnitBehaviour>().unitId);
            
            long unitId = other.gameObject.GetComponent<UnitBehaviour>().unitId;
            
            UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
            
            Unit unit =unitComponent.Get(unitId);
           
            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            
            
            if(unitConfig == null)
            {
                Debug.Log("unit为空");
            }
            else
            {
                Debug.Log("unit存在 name:" + unitConfig.Id + " Name: " + unitConfig.Name + " PrefabName: " + unitConfig.PrefabName);
            }
        }

        private static void MyTriggerExit(this PlayerCollisionComponent self, Collider other)
        {
            //Debug.Log("ET触发器 退出： name:" +other.gameObject.name);
            
            // 处理边界碰撞退出
            if (other.gameObject.CompareTag("Collider"))
            {
                Debug.Log("离开边界");
                return;
            }

            switch (other.gameObject.tag)
            {
                //钓鱼
                case "Lake":
                    Debug.Log("ET触发器 进入2：  id:");
                    self.DynamicEvent(new OutResource()).NoContext();
                    break;
                //资源类
                case "Resource":
                    Debug.Log("ET触发器 进入3：  id:"+other.gameObject.GetComponent<UnitBehaviour>().unitId);

                    self.DynamicEvent(new OutResource()).NoContext();
                    Debug.Log("ET触发器 退出 name:" + other.gameObject.name + " id:" + other.gameObject.GetComponent<UnitBehaviour>().unitId);

                    break;
                //能量类
                case "Energy":
                    Debug.Log("ET触发器 进入4：  id:"+other.gameObject.GetComponent<UnitBehaviour>().unitId);
                    self.DynamicEvent(new OutResource()).NoContext();
                    Debug.Log("ET触发器 退出 name:" + other.gameObject.name + " id:" + other.gameObject.GetComponent<UnitBehaviour>().unitId);
                    break;
            }
        }

        #endregion
       

    }
}