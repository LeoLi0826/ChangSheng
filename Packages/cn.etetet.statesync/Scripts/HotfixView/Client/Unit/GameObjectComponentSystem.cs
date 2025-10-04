using System;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(GameObjectComponent))]
    public static partial class GameObjectComponentSystem
    {
         [EntitySystem]
        private static void Awake(this GameObjectComponent self)
        {

        }
        
        
        [EntitySystem]
        private static void Destroy(this GameObjectComponent self)
        {
            if (self.IsPool)
            {
                self.IsPool = false;
                //对象池回收
                YIUIGameObjectPool.Inst.Put(self.GameObject);
            }
            else
            {
                //非对象池销毁
                GameObject.Destroy(self.GameObject);
            }
        }
        
        /// <summary>
        /// 直接转向方向
        /// </summary>
        /// <param name="dir"></param>
        public static void LookAtForward(this GameObjectComponent self, Vector2 dir)
        {
            self.Transform.localEulerAngles = dir.x > 0 ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
        }

        /// <summary>
        /// 新建一个游戏对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        public static void NewGameObject(this GameObjectComponent self, string name)
        {
            self.GameObject = new GameObject(name);
        }

        /// <summary>
        /// 场景查找游戏对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="path"></param>
        public static void FindGameObject(this GameObjectComponent self, string path)
        {
            self.GameObject = GameObject.Find(path);
        }

        /// <summary>
        /// 对象池加载游戏对象
        /// </summary>
        /// <param name="self"></param>
        /// <param name="path"></param>
        public static async ETTask<GameObject> LoadPoolAsset(this GameObjectComponent self, string path, Transform parent = null)
        {
            EntityRef<GameObjectComponent> selfRef = self;
            self.IsPool = true;
            self.GameObject = await YIUIGameObjectPool.Inst.Get(path, parent);
            self = selfRef;
            GameObjectEntityRef gameObjectEntityRef = self.GameObject.GetComponent<GameObjectEntityRef>()??self.GameObject.AddComponent<GameObjectEntityRef>();
            gameObjectEntityRef.Entity = self.Parent;
            return self.GameObject;
        }
    }
}