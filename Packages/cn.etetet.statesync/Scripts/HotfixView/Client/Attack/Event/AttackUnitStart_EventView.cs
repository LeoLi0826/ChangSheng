using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.StateSync)]
    [FriendOfAttribute(typeof(ET.PlayerBehaviourComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public class AttackUnitStart_EventView : AEvent<Scene, AttackUnitStart>
    {
        protected override async ETTask Run(Scene scene, AttackUnitStart args)
        {
            if (args.TargetUnit == null || args.TargetUnit.IsDisposed)
                return;
            if (!args.TargetUnit.IsAlive())
                return;

            // 简化版振刀：玩家处于招架窗口则免疫本次伤害
            if (args.TargetUnit.UnitType == UnitType.Player)
            {
                PlayerBehaviourComponent playerBehaviour = args.TargetUnit.GetComponent<PlayerBehaviourComponent>();
                if (playerBehaviour != null && playerBehaviour.IsParryActive)
                {
                    Debug.Log($"振刀成功，免疫伤害");
                    // 若攻击者是黄风大圣且当前处于蓄力冲刺阶段，进入虚弱6秒
                    Unit attacker = args.AttackUnit;
                    if (attacker != null && !attacker.IsDisposed)
                    {
                        var hfd = attacker.GetComponent<HuangFengDaShengAttackComponent>();
                        if (hfd != null && hfd.IsChargeAttacking)
                        {
                            EnterWeakState(attacker, hfd).NoContext();
                        }
                    }
                    return;
                }
            }
            Debug.Log($"扣血 {args.Damage}");
            NumericComponent numeric = args.TargetUnit.GetComponent<NumericComponent>();
            if (args.TargetUnit.UnitType == UnitType.Monster)
            {
                var hfd = args.TargetUnit.GetComponent<HuangFengDaShengAttackComponent>();
                if (hfd != null && hfd.IsWeakened)
                {
                    args.Damage *= 3;
                    Debug.Log($"黄风大圣虚弱，伤害翻倍 {args.Damage}");
                }
            }
            numeric[NumericType.Hp] -= args.Damage;
            Debug.Log($"扣血 {args.Damage}，当前HP: {numeric[NumericType.Hp]}");
            if (numeric[NumericType.Hp] <= 0)
            {
                numeric[NumericType.IsAlive] = 0;
                if (args.TargetUnit.UnitType == UnitType.Player)
                {
                    args.TargetUnit.GetComponent<PlayerBehaviourComponent>()?.ChangeToDie();
                }
                else
                {
                    args.TargetUnit.Dispose();

                }
            }
            await ETTask.CompletedTask;
        }

        private static async ETTask EnterWeakState(Unit boss, HuangFengDaShengAttackComponent comp)
        {
            // 停止当前所有攻击状态
            comp.IsNormalAttacking = false;
            comp.IsChargeAttacking = false;
            comp.IsUltimateAttacking = false;
            comp.IsFinished = true;
            comp.IsWeakened = true;

            // 设定虚弱动作并保持6秒
            boss.SetAIUnitActionType(UnitActionType.Weak, false);
            boss.SetMonsterActionType(MonsterActionType.Idle);

            // 6秒虚弱
            var timer = boss.Root().GetComponent<TimerComponent>();
            await timer.WaitAsync(6000);

            if (boss.IsDisposed || comp.IsDisposed)
                return;

            // 恢复到战斗状态
            boss.SetAIUnitActionType(UnitActionType.Attack, false);
            boss.SetMonsterActionType(MonsterActionType.Attack);
            comp.IsWeakened = false;
        }
    }
}