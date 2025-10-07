using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 动画优先级定义
    /// </summary>
    public enum AnimationPriority
    {
        Base = 0,   // 基础
        Skill = 10, // 技能
    }

    [ComponentOf(typeof(Unit))]
    public class UnitAnimationComponent : Entity,IAwake<UnitSpine,string>,IDestroy
    {
        public UnitSpine UnitSpine;
        
        public EntityRef<Unit> m_unit;
        public Unit Unit => m_unit;
        
        public EntityRef<SpineComponent> m_spineComponent;
        public SpineComponent SpineComponent => this.m_spineComponent;
        
        public AnimationPriority CurrentPriority = AnimationPriority.Base;
        
        public string RequestedBaseAnimation;
        
        public bool IsUpdateQueued = false;
    }
}

