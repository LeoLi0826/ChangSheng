using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        private GameObject gameObject;

        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
            set
            {
                this.gameObject = value;
                this.Transform = value.transform;
                this.BindPointComponent = this.gameObject.GetComponent<BindPointComponent>();
            }
        }
        
        public Transform Transform { get; private set; }
        
        public BindPointComponent BindPointComponent{ get; private set; }

        /// <summary>
        /// 是否是对象池
        /// </summary>
        public bool IsPool;
    }
}