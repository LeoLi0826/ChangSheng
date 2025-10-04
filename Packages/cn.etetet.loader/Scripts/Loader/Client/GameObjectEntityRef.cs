using UnityEngine;

namespace ET
{
    public struct EntityViewTriggerEnter
    {
        public EntityRef<Entity> Entity;
    }

    public class GameObjectEntityRef: MonoBehaviour
    {
        private EntityRef<Entity> entityRef;
        
#if UNITY_EDITOR && ENABLE_VIEW
        public GameObject gameObjectRef;
#endif
        
        public Entity Entity
        {
            get
            {
                return this.entityRef;
            }
            set
            {
                this.entityRef = value;

#if UNITY_EDITOR && ENABLE_VIEW
                this.gameObjectRef = value.ViewGO;
#endif
            }
        }
        
        
        private void OnMouseDown()
        {
            Entity entity = this.entityRef;
            EventSystem.Instance.Publish(entity.Scene(),new EntityViewTriggerEnter()
            {
                Entity = entity,
            });
        }
    }
}