using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System;

namespace ET.Client
{
    public enum BindPoint
    {
        HpBar,
        BattleWillBar,
        UnitCenter,
        UnitFoot,
        UnitHead,
    }
    
    public class BindPointComponent : SerializedMonoBehaviour
    {
        [NonSerialized, OdinSerialize]
        [DictionaryDrawerSettings(KeyLabel = "类型", ValueLabel = "绑定点")]
        public Dictionary<BindPoint,Transform> m_BindPoints = new Dictionary<BindPoint,Transform>();

        public Transform GetBindPoint(BindPoint bindPoint)
        {
            if (m_BindPoints.TryGetValue(bindPoint, out var trans))
            {
                return trans;
            }
            return null;
        }
    }
}

