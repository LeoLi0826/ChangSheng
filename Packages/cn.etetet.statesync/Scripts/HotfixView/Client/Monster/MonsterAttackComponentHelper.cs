using UnityEngine;

namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.Client.MonsterAttackComponent))]
    [FriendOfAttribute(typeof(ET.Client.HuangFengDaShengAttackComponent))]
    public static class MonsterAttackComponentHelper
    {
        /// <summary>
        /// 统一设置攻击标志
        /// 支持MonsterAttackComponent和HuangFengDaShengAttackComponent
        /// </summary>
        public static void SetAttackFlag(this Unit unit, bool flag)
        {
            // 先尝试小怪物攻击组件
            var normalAttack = unit.GetComponent<MonsterAttackComponent>();
            if (normalAttack != null)
            {
                normalAttack.AttackFlag = flag;
                Debug.Log($"设置小怪物攻击标志: {unit.Id}, Flag: {flag}");
                return;
            }

            // 再尝试黄风大圣攻击组件
            var bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (bossAttack != null)
            {
                bossAttack.AttackFlag = flag;
                Debug.Log($"设置黄风大圣攻击标志: {unit.Id}, Flag: {flag}");
                return;
            }

            Debug.LogWarning($"未找到任何攻击组件来设置AttackFlag: {unit.Id}");
        }

        /// <summary>
        /// 统一获取攻击标志
        /// </summary>
        public static bool GetAttackFlag(this Unit unit)
        {
            // 先尝试小怪物攻击组件
            var normalAttack = unit.GetComponent<MonsterAttackComponent>();
            if (normalAttack != null)
            {
                return normalAttack.AttackFlag;
            }

            // 再尝试黄风大圣攻击组件
            var bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (bossAttack != null)
            {
                return bossAttack.AttackFlag;
            }

            return false;
        }

        /// <summary>
        /// 统一设置攻击目标
        /// </summary>
        public static void SetAttackTarget(this Unit unit, Unit target)
        {
            // 先尝试小怪物攻击组件
            var normalAttack = unit.GetComponent<MonsterAttackComponent>();
            if (normalAttack != null)
            {
                normalAttack.Target = target;
                Debug.Log($"设置小怪物攻击目标: {unit.Id} -> {target?.Id}");
                return;
            }

            // 再尝试黄风大圣攻击组件
            var bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (bossAttack != null)
            {
                bossAttack.Target = target;
                Debug.Log($"设置黄风大圣攻击目标: {unit.Id} -> {target?.Id}");
                return;
            }

            Debug.LogWarning($"未找到任何攻击组件来设置Target: {unit.Id}");
        }

        /// <summary>
        /// 统一获取攻击目标
        /// </summary>
        public static Unit GetAttackTarget(this Unit unit)
        {
            // 先尝试小怪物攻击组件
            var normalAttack = unit.GetComponent<MonsterAttackComponent>();
            if (normalAttack != null)
            {
                return normalAttack.Target;
            }

            // 再尝试黄风大圣攻击组件
            var bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (bossAttack != null)
            {
                return bossAttack.Target;
            }

            return null;
        }

        /// <summary>
        /// 检查是否有任何攻击组件
        /// </summary>
        public static bool HasAttackComponent(this Unit unit)
        {
            return unit.GetComponent<MonsterAttackComponent>() != null ||
                   unit.GetComponent<HuangFengDaShengAttackComponent>() != null;
        }

        /// <summary>
        /// 获取攻击组件类型名称（用于调试）
        /// </summary>
        public static string GetAttackComponentType(this Unit unit)
        {
            if (unit.GetComponent<MonsterAttackComponent>() != null)
            {
                return "MonsterAttackComponent";
            }

            if (unit.GetComponent<HuangFengDaShengAttackComponent>() != null)
            {
                return "HuangFengDaShengAttackComponent";
            }

            return "None";
        }
        
        /// <summary>
        /// 统一调用攻击方法
        /// 支持MonsterAttackComponent和HuangFengDaShengAttackComponent
        /// </summary>
        public static void CallAttack(this Unit unit, Unit target)
        {
            // 先尝试小怪物攻击组件
            var normalAttack = unit.GetComponent<MonsterAttackComponent>();
            if (normalAttack != null)
            {
                normalAttack.Attack(target);
                Debug.Log($"调用小怪物攻击: {unit.Id} -> {target?.Id}");
                return;
            }
            
            // 再尝试黄风大圣攻击组件
            var bossAttack = unit.GetComponent<HuangFengDaShengAttackComponent>();
            if (bossAttack != null)
            {
                bossAttack.Attack(target);
                Debug.Log($"调用黄风大圣攻击: {unit.Id} -> {target?.Id}");
                return;
            }
            
            Debug.LogWarning($"未找到任何攻击组件来调用Attack方法: {unit.Id}");
        }
    }
}