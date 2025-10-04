using ET.Client;
using UnityEngine;

namespace ET
{
    [Invoke(TimerInvokeType.EnemyAngry)]
    [FriendOfAttribute(typeof(EnemyBehaviourComponent))]
    public class EnemyAngryHandler : ATimer<EnemyBehaviourComponent>
    {
        protected override void Run(EnemyBehaviourComponent self)
        {
            // 防护检查：确保组件和Unit未被销毁
            if (self == null || self.IsDisposed)
            {
                Debug.LogWarning("EnemyBehaviourComponent已被销毁，跳过Timer执行");
                return;
            }

            Unit unit = self.MyUnit;
            if (unit == null || unit.IsDisposed)
            {
                Debug.LogWarning("怪物Unit已被销毁，跳过Timer执行");
                return;
            }

            if (self.AngryFlag == true)
            {
                Debug.Log("怪物行为攻击： 311");
                self.StopMove();

                unit.Direction = Direction.Front;
                //被攻击 仇恨启动 开始攻击
                unit.SetAIUnitActionType(UnitActionType.Attack, false);
            }
        }
    }
}