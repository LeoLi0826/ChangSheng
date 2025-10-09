using System.Collections.Generic;
using UnityEngine;
using YIUIFramework;

namespace ET.Client
{
    [GM(EGMType.Speed, 2, "移动-属性修改")]
    public class GM_ChangeSpeed : IGMCommand
    {
        public List<GMParamInfo> GetParams()
        {
            return new()
            {
                new GMParamInfo(EGMParamType.Long, "基础值", "0"),
                new GMParamInfo(EGMParamType.Long, "增加值", "0"),
                new GMParamInfo(EGMParamType.Long, "百分比", "0"),
                new GMParamInfo(EGMParamType.Long, "最终加成", "0"),
                new GMParamInfo(EGMParamType.Long, "最终百分比", "0"),
            };
        }

        public async ETTask<bool> Run(Scene clientScene, ParamVo paramVo)
        {
            var baseValue = paramVo.Get<long>();
            var addValue = paramVo.Get<long>(1);
            var pctValue = paramVo.Get<long>(2);
            var finalAddValue = paramVo.Get<long>(3);
            var finalPctValue = paramVo.Get<long>(4);
            Unit unit = UnitHelper.GetMyUnitFromClientScene(clientScene);
            if (unit != null)
            {
                NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
                if (numericComponent != null)
                {
                    numericComponent.Set(NumericType.SpeedBase,baseValue);
                    numericComponent.Set(NumericType.SpeedAdd,addValue);
                    numericComponent.Set(NumericType.SpeedPct,pctValue);
                    numericComponent.Set(NumericType.SpeedFinalAdd,finalAddValue);
                    numericComponent.Set(NumericType.SpeedFinalPct,finalPctValue);
                }
            }
            await ETTask.CompletedTask;
            return true;
        }
    }
}