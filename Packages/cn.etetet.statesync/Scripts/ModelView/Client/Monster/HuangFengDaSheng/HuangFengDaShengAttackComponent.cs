using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class HuangFengDaShengAttackComponent : Entity, IAwake, IDestroy, IUpdate
    {
        // 基础攻击状态
        public bool IsCd;
        public EntityRef<Unit> Target;
        public bool AttackFlag;
        
        // 定时器管理
        public long AttackCdTimerId;        // 普通攻击CD定时器
        public long ChargeAttackCdTimerId;  // 蓄力攻击CD定时器
        public long ZhendaoWindowTimerId;   // 振刀窗口定时器
        
        // 攻击状态标志
        public bool IsNormalAttacking;      // 是否正在普通攻击
        public bool IsChargeAttacking;      // 是否正在蓄力攻击
        public bool IsUltimateAttacking;    // 是否正在使用大招
        public bool IsFinished;             // 当前攻击是否完成
        
        // CD状态标志
        public bool IsChargeAttackOnCd;     // 蓄力攻击是否在CD中
        
        // 攻击配置 - 分别设置不同攻击类型的CD时间
        public int NormalAttackCooldown = 2000;    // 普通攻击CD (2秒)
        public int ChargeAttackCooldown = 5000;    // 蓄力攻击CD (5秒)

        // 运行时状态：当前这次冲刺是否已结算伤害
        public bool _hasDealtDamageThisRush;
        
        // 攻击权重配置 (总和应为100)
        public int NormalAttackWeight = 60;   // 60%概率普通攻击
        public int ChargeAttackWeight = 50;   // 30%概率蓄力攻击

        // 大招相关
        public long UltimateAttackCdTimerId;

        // 大招-龙卷风集管理
        public bool HasActiveTornadoes;                    // 是否存在已生成的龙卷风集
        public List<EntityRef<Unit>> TornadoUnits = new List<EntityRef<Unit>>(4); // 当前龙卷风Unit引用
        public bool IsRefreshingTornadoes;                 // 是否正在刷新（销毁并重召）龙卷风，防止重复触发
        public float UltimateOriginX;                      // 大招释放点X（用于刷新判定）
        public float UltimateOriginY;                      // 大招释放点Y
        public float UltimateOriginZ;                      // 大招释放点Z
        public float TornadoRefreshDistance = 3f;          // 超过该距离则销毁旧龙卷风并在当前位置重召

        // 虚弱状态
        public bool IsWeakened;                            // 是否处于虚弱中
        
        // 沙尘暴效果管理
        public GameObject SandstormGameObject;             // 沙尘暴粒子效果GameObject引用
        
        // 测试状态设置
        public int TestState = 7; // 0-正常，1-只普通攻击，2-只蓄力攻击，3-只大招
    }
}