using UnityEngine;

namespace ET.Client
{
    //改变状态机
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Unit))]
    public class ChangeAIAnimation_Event : AEvent<Scene, ChangeAIAnimation>
    {
        protected override async ETTask Run(Scene scene, ChangeAIAnimation args)
        {
            Unit unit = args.changeUnit;
            
            //Debug.Log("unit 敌人 "+unit.unitActionType + " 方向： "+unit.Direction);
            
            AISkeletonAnimationComponent skeletonAnimationComponent = unit.GetComponent<AISkeletonAnimationComponent>();
            switch (unit.unitActionType)
            {
                case UnitActionType.Idle:
                    switch (unit.Direction)
                    {
                        case Direction.Front:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.front_idle, args.changeLoop);
                            break;
                        case Direction.Back:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.behind_idle, args.changeLoop);
                            break;
                      
                    }
                    break;
                case UnitActionType.Walk:
                    // 根据单位朝向选择对应的行走动画
                    switch (unit.Direction)
                    {
                        case Direction.Front:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.front_walk, args.changeLoop);
                            break;
                        case Direction.Back:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.behind_walk, args.changeLoop);
                            break;
                       
                    }
                    break;
                case UnitActionType.Attack:
                    // 根据单位朝向选择对应的攻击动画
                    switch (unit.Direction)
                    {
                        
                        case Direction.Front:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.front_attack, args.changeLoop);
                            break;
                        case Direction.Left:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.left_attack, args.changeLoop);
                            break;
                      
                    }
                    break;
                case UnitActionType.Weak:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.front_weak, args.changeLoop);
                    break;
                case UnitActionType.Die:
                    if (unit.UnitType == UnitType.Monster)
                        return;
                    skeletonAnimationComponent.Play(SkeletonAnimationType.front_die, args.changeLoop);
                    break;
                case UnitActionType.Skill:
                    //skeletonAnimationComponent.Play(SkeletonAnimationType.front_skill, args.changeLoop);
                    break;
                
                
                //黄风大圣
                case UnitActionType.HuangFengAttack1:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuanfeng_1, args.changeLoop);
                    break;
                case UnitActionType.HuangFengAttack2:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuanfeng_2, args.changeLoop);
                    break;
                case UnitActionType.HuangFengAttack3:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuanfeng_3, args.changeLoop);
                    break;
                case UnitActionType.HuangFengXuLi_1:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuli_1, args.changeLoop);
                    break;
                case UnitActionType.HuangFengXuLi_2:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuli_2, args.changeLoop);
                    break;
                case UnitActionType.HuangFengXuLi_3:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.xuli_3, args.changeLoop);
                    break;
            }
            await ETTask.CompletedTask;
        }
    }
}