using UnityEngine;

namespace ET.Client
{
    //改变状态机
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.Unit))]
    public class ChangeAnimation_Event : AEvent<Scene, ChangeAnimation>
    {
        protected override async ETTask Run(Scene scene, ChangeAnimation args)
        {
            Unit unit = args.changeUnit;

            if (unit.ConfigId != 1001)
            {
                Debug.Log("unit 玩家 "+unit.unitActionType + " 方向： "+unit.Direction);
            }
            SkeletonAnimationComponent skeletonAnimationComponent = unit.GetComponent<SkeletonAnimationComponent>();
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
                        case Direction.Left:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.left_idle, args.changeLoop);
                            break;
                        case Direction.Right:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.right_idle, args.changeLoop);
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
                        case Direction.Left:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.left_walk, args.changeLoop);
                            break;
                        case Direction.Right:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.right_walk, args.changeLoop);
                            break;
                    }
                    break;
                case UnitActionType.Attack:
                    // 根据单位朝向选择对应的攻击动画
                    switch (unit.Direction)
                    {
                        
                        case Direction.Front:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.front_attack1, args.changeLoop);
                            break;
                        case Direction.Left:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.left_attack1, args.changeLoop);
                            break;
                        case Direction.Right:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.right_attack1, args.changeLoop);
                            break;
                        case Direction.Back:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.behind_attack1, args.changeLoop);
                            break;
                        default:
                            skeletonAnimationComponent.Play(SkeletonAnimationType.front_attack1, args.changeLoop);
                            break;
                    }
                    break;
                case UnitActionType.Pick:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.front_pick_short, args.changeLoop);
                    break;
                case UnitActionType.Zhaojia:
                    skeletonAnimationComponent.Play(SkeletonAnimationType.front_zhaojia, args.changeLoop);
                    break;
                case UnitActionType.Die:
                    if (unit.UnitType == UnitType.Monster)
                        return;
                    skeletonAnimationComponent.Play(SkeletonAnimationType.front_die, args.changeLoop);
                    break;
                case UnitActionType.Skill:
                    //skeletonAnimationComponent.Play(SkeletonAnimationType.front_skill, args.changeLoop);
                    break;
            }
        }
    }
}