using Spine.Unity;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    [FriendOfAttribute(typeof(ET.Client.GameObjectComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    public class AfterUnitCreate_CreateUnitView : AEvent<Scene, AfterUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterUnitCreate args)
        {
            Unit unit = args.Unit;
            // Unit View层
            
            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            GlobalComponent globalComponent = scene.Root().GetComponent<GlobalComponent>();
            
            GameObjectComponent gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            await gameObjectComponent.LoadPoolAsset(unitConfig.PrefabName,globalComponent.Unit);
            
            gameObjectComponent.Transform.rotation = Camera.main.transform.rotation;
            gameObjectComponent.Transform.position = unit.Position;
            SkeletonAnimation skeletonAnimation = gameObjectComponent.GameObject.GetComponentInChildren<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                unit.AddComponent<SpineComponent, SkeletonAnimation>(skeletonAnimation);
                unit.AddComponent<UnitAnimationComponent, UnitSpine, string>(gameObjectComponent.GameObject.GetComponent<UnitSpine>(),"idle");
            }
            switch (unit.UnitType)
            {
                case UnitType.Player:
                    unit.AddComponent<CinemachineComponent>();
                    unit.AddComponent<Pathfinding2DComponent, GameObject>(gameObjectComponent.GameObject);
                    unit.AddComponent<Move2DComponent>();
                    unit.AddComponent<InputSystemComponent>();
                    
                    EventSystem.Instance.Publish(scene, new AfterUnitCreateGetView() { Unit = unit });
                    unit.Position = new Vector3(45f, 0, 25f);
                    break;
                case UnitType.Monster:
                    if (unit.GetComponent<EnemyBehaviourComponent>() == null)
                    {
                        unit.AddComponent<EnemyBehaviourComponent>();
                    }
                    
                    unit.AddComponent<MonsterCollisionComponent>();
                    break;
            }

            EventSystem.Instance.Publish(scene,new AfterUnitViewCreate()
            {
                Unit = unit,
            });
            await ETTask.CompletedTask;
        }

    }
}