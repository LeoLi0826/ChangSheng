﻿using System;

namespace ET
{
    [EntitySystemOf(typeof(ConditionMgr))]
    [FriendOf(typeof(ConditionMgr))]
    public static partial class ConditionMgrSystem
    {
        [EntitySystem]
        private static void Awake(this ConditionMgr self)
        {
            self.m_Conditions.Clear();
            var types = CodeTypes.Instance.GetTypes(typeof(ConditionAttribute));
            foreach (var type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(ConditionAttribute), false);
                if (attrs.Length >= 2)
                {
                    Log.Error($"有多个相同特性 只允许有一个 ConditionAttribute");
                }

                var conditionAttribute = (ConditionAttribute)attrs[0];
                var conditionType = conditionAttribute.ConditionType;
                if (self.m_Conditions.ContainsKey(conditionType))
                {
                    Log.Error($"错误 条件类型重复 {conditionType} 一个条件只能有一个实现");
                    continue;
                }

                var conditionInfo = new ConditionInfo();
                conditionInfo.ConditionType = conditionType;
                conditionInfo.Condition = (ICondition)Activator.CreateInstance(type);
                conditionInfo.Listener = conditionAttribute.Listener;
                conditionInfo.CheckValueType = conditionAttribute.CheckValueType;
                self.m_Conditions.Add(conditionType, conditionInfo);
            }
        }

        [EntitySystem]
        private static void Destroy(this ConditionMgr self)
        {
        }
    }
}