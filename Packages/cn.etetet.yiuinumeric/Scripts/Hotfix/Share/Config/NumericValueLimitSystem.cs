using System.Collections.Generic;

namespace ET
{
    public static class NumericValueLimitSystem
    {
        public static long GetLimitValue(this NumericValueLimitData limitData, NumericData data)
        {
            switch (limitData)
            {
                case NumericValueLimitNumber limitNumber:
                    return limitNumber.Value;
                case NumericValueLimitNumeric limitNumeric:
                    return data.NumericDic.GetValueOrDefault((int)limitNumeric.Value, 0);
                default:
                    Log.Error($"未实现的NumericValueLimitData类型: {limitData.GetType()}");
                    return 0;
            }
        }
    }
}