using Unity.Cinemachine;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(CinemachineComponent))]
    public static partial class CinemachineComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.CinemachineComponent self)
        {
            GlobalComponent globalComponent = self.Root().GetComponent<GlobalComponent>();
            GameObjectComponent gameObjectComponent = self.GetParent<Unit>().GetComponent<GameObjectComponent>();

            self.Head = gameObjectComponent.Transform;
            GameObject go = new GameObject("CameraFollow");
            self.Follow = go.transform;
            Transform followTransform = self.Follow;
            followTransform.SetParent(globalComponent.Unit, true);
            followTransform.position = self.Head.position;
            followTransform.rotation = gameObjectComponent.GameObject.transform.rotation;
            globalComponent.Camera.LookAt = followTransform;
            globalComponent.Camera.Follow = followTransform;
        }
        
        
        [EntitySystem]
        private static void Destroy(this ET.Client.CinemachineComponent self)
        {
            GameObject.Destroy(self.Follow.gameObject);
            self.Head = null;
            self.Follow = null;
        }
    }
}

