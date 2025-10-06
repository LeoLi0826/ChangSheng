using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    [FriendOfAttribute(typeof(ET.Client.GameObjectComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MonsterMoveComponent))]
    public class AfterMapUnitCreate_CreateUnitView : AEvent<Scene, AfterMapUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterMapUnitCreate args)
        {
            Unit unit = args.Unit;
            // Unit View层
            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            GlobalComponent globalComponent = scene.Root().GetComponent<GlobalComponent>();

            GameObjectComponent gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            await gameObjectComponent.LoadPoolAsset(unitConfig.PrefabName,globalComponent.Unit);
            
            gameObjectComponent.Transform.rotation = Camera.main.transform.rotation;
            gameObjectComponent.Transform.position = unit.Position;
           
            //这里需要发送各种数据给ui面板？
            //赋值位置
            switch (unit.UnitType)
            {
                // leoplayer 相关设置
                case UnitType.Player:
                    unit.Position = new Vector3(45f, 0, 25f);
                    break;
                case UnitType.Monster:
                    if (unitConfig.Id == 1082)
                    {
                        unit.AddComponent<HuangFengDaShengAttackComponent>();
                    }
                    else
                    {
                        unit.AddComponent<MonsterAttackComponent>(); 
                    }
                   
                    unit.AddComponent<EnemyBehaviourComponent>();
                  
                    AISkeletonAnimationComponent skeleton = unit.AddComponent<AISkeletonAnimationComponent>();
                    unit.Direction = Direction.Front;
                    Debug.Log($"[怪物重新显现] 单位ID:{unit.Id} 重新显现时设置Walk动画状态"); 
                    unit.SetAIUnitActionType(UnitActionType.Walk, true);
                    
                    unit.AddComponent<MonsterMoveComponent, Vector3>(gameObjectComponent.Transform.position);
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