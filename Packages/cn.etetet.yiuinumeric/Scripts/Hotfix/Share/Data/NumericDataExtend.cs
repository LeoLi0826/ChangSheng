#if UNITY_EDITOR
#define NUMERIC_CHECK_SYMBOLS //数值检查宏 离开Unity要使用就在unity设置中添加这个宏
#endif

using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 额外数值数据扩展
    /// </summary>
    [FriendOf(typeof(NumericDataComponent))]
    public static partial class NumericDataExtend
    {
        #region Private 禁止开放

        private static long GetByKey(this NumericData self, int key)
        {
            if (self == null)
            {
                Log.Error($"不可能NULL的数据");
                return 0;
            }

            if (self.NumericDic == null)
            {
                Log.Error($"不可能NULL的数据");
                return 0;
            }

            self.NumericDic.TryGetValue(key, out var value);
            return value;
        }

        private static void ChangeByKey(this NumericData self,
                                        int              numericType,
                                        long             value,
                                        bool             isPushEvent = true,
                                        bool             isAdd       = true,
                                        bool             check       = true)
        {
            #if NUMERIC_CHECK_SYMBOLS
            if (check && !numericType.CheckChangeNumeric()) return;
            #endif

            //这里一定是基础数据的改变
            //基础数据也会推送 根据需求监听
            //也可以根据需求取消推送
            if (self.ChangeValue(numericType, value, isPushEvent, isAdd))
            {
                //基础数据改变后刷新最终数据
                self.UpdateResult(numericType, isPushEvent);
            }
        }

        /// <summary>
        /// 修改目标值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="numericType">修改类型</param>
        /// <param name="value">值</param>
        /// <param name="isPushEvent">推送</param>
        /// <param name="isAdd">默认true = (+=)  false = 覆盖操作 (不明白的禁止传false)</param>
        private static bool ChangeValue(this NumericData self,
                                        int              numericType,
                                        long             value,
                                        bool             isPushEvent = true,
                                        bool             isAdd       = true)
        {
            //如果是+ 但是+为0 则不做任何操作
            if (isAdd && value == 0)
            {
                return false;
            }

            var oldValue = self.GetByKey(numericType);

            //如果是赋值 但是当前值与赋值相同则不做任何操作
            if (!isAdd && oldValue == value)
            {
                return false;
            }

            var newValue = isAdd ? oldValue + value : value;

            var numericEnum = (ENumericType)numericType;

            var limitConfig = NumericValueLimitConfigCategory.Instance.GetOrDefault(numericEnum);

            if (limitConfig != null)
            {
                var minLimitValue = limitConfig.Min.GetLimitValue(self);
                var maxLimitValue = limitConfig.Max.GetLimitValue(self);
                self.NumericDic[numericType] = Math.Clamp(newValue, minLimitValue, maxLimitValue);
            }
            else
            {
                self.NumericDic[numericType] = newValue;
            }

            var affectConfig = NumericValueAffectConfigCategory.Instance.GetOrDefault(numericEnum);

            var limitValue = self.NumericDic[numericType];

            if (affectConfig != null)
            {
                foreach (var affect in affectConfig.Affects)
                {
                    if (!affect.CheckChangeNumeric()) continue;

                    var uniqueId = affectConfig.GetAffectUniqueId(affect);
                    if (uniqueId == 0) continue;
                    var affectType    = (int)affect;
                    var affectCurrent = self.NumericDic.GetValueOrDefault(affectType, 0);
                    var affectNewValue = EventSystem.Instance.Invoke<NumericAffect, long>(uniqueId, new NumericAffect
                    {
                        Data              = self,
                        NumericType       = numericType,
                        Old               = oldValue,
                        New               = limitValue,
                        AffectNumericType = affectType,
                        AffectCurrent     = affectCurrent,
                    });

                    self.ChangeByKey(affectType, affectNewValue, isPushEvent, false);
                }
            }

            if (isPushEvent)
            {
                self.PushEvent(numericType, limitValue, oldValue);
            }

            return true;
        }

        /// <summary>
        /// 更新最终值 核心内部公式算法
        /// 只有基础数据发生变化时会自动更新
        /// 禁止外部调用
        /// 传入的ID 一定是 大于 NumericConst.Max
        /// </summary>
        private static void UpdateResult(this NumericData self, int numericType, bool isPushEvent)
        {
            //非成长无需计算
            if (numericType.IsNotGrowNumeric())
            {
                return;
            }

            var final     = numericType / 10;
            var bas       = final * 10 + 1;
            var add       = final * 10 + 2;
            var pct       = final * 10 + 3;
            var finalAdd  = final * 10 + 4;
            var finalPct  = final * 10 + 5;
            var resultAdd = final * 10 + 6;

            /*
            // 第1步 = 基础 + 基础增加 (1+2)
            var value1 = self.GetByKey(bas) + self.GetByKey(add);
            // 第2步 = * 百分比  (1+2)*3
            var value2 = value1 * (NumericConst.IntRate + self.GetByKey(pct)) / NumericConst.IntRate;
            // 第3步 = + 最终额外增加 (1+2)*3+4
            var value3 = value2 + self.GetByKey(finalAdd);
            // 第4步 = * 最终百分比 ((1+2)*3+4)*5
            var value4 = value3 * (NumericConst.IntRate + self.GetByKey(finalPct)) / NumericConst.IntRate;
            // 第5步 = + 结果增加 ((1+2)*3+4)*5+6
            var value5 = value4 + self.GetByKey(resultAdd);
            // 第6步 最终结果 向下取整 (C#中存在整除 所以可以不用取整)
            var result = (long)value5;

            var valueBasAdd        = self.GetByKey(bas) + self.GetByKey(add);
            var valueAfterPct      = valueBasAdd * (NumericConst.IntRate + self.GetByKey(pct)) / NumericConst.IntRate;
            var valueAfterFinalAdd = valueAfterPct + self.GetByKey(finalAdd);
            var valueAfterFinalPct = valueAfterFinalAdd * (NumericConst.IntRate + self.GetByKey(finalPct)) / NumericConst.IntRate;
            var finalResult        = valueAfterFinalPct + self.GetByKey(resultAdd);
            self.ChangeValue(final, finalResult, isPushEvent, false);

            */

            // 定点数: https://lib9kmxvq7k.feishu.cn/wiki/Gl1VwvLmlicjE8krUz0cxfcsnhc
            // ((((基础值 + 附加值) * 百分比因子) + 最终附加) * 最终百分比因子) + 结果附加

            var result =
                    (
                        (

                            // 1 + 2
                            self.GetByKey(bas) + self.GetByKey(add)
                        )

                        // * 3
                      * (NumericConst.IntRate + self.GetByKey(pct)) / NumericConst.IntRate

                        // + 4
                      + self.GetByKey(finalAdd)
                    )

                    // * 5
                  * (NumericConst.IntRate + self.GetByKey(finalPct)) / NumericConst.IntRate

                    // + 6
                  + self.GetByKey(resultAdd);

            //最终值 使用覆盖
            self.ChangeValue(final, result, isPushEvent, false);
        }

        #endregion
    }
}