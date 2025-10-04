using System;

using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(ForgeGetComponent))]
    [FriendOf(typeof(ForgeGetComponent))]
    public static partial class ForgeGetComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.ForgeGetComponent self)
        {
            // Slot PlayerBehaviour = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject.GetComponent<PlayerBehaviour>();
            //     
            // PlayerBehaviour.SetOnCollisionEnter2D(self.MyOnCollisionEnter2D);
        }
        [EntitySystem]
        private static void Update(this ET.Client.ForgeGetComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Client.ForgeGetComponent self)
        {

        }
        
        
        // public static void MyOnCollisionEnter2D(this ForgeGetComponent self,PointerEventData collision)
        // {
        //     Debug.Log("ET碰撞： name:" +collision.gameObject.name);
        //     
        //     // Debug.Log("ET碰撞： id:" +collision.gameObject.GetComponent<UnitBehaviour>().unitId);
        //     // long unitId = collision.gameObject.GetComponent<UnitBehaviour>().unitId;
        //     //
        //     // UnitComponent unitComponent = self.Root().CurrentScene().GetComponent<UnitComponent>();
        //     //
        //     // Unit unit =unitComponent.Get(unitId);
        //     //
        //     // UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
        //     // if(unitConfig == null)
        //     // {
        //     //     Debug.Log("unit为空");
        //     // }
        //     // else
        //     // {
        //     //     Debug.Log("unit存在 name:" + unitConfig.Id + " Name: " + unitConfig.Name + " PrefabName: " + unitConfig.PrefabName);
        //     // }
        // }

    }
}