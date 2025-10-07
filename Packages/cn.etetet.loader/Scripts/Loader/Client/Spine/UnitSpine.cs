using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ET.Client
{
    public class UnitSpine : SerializedMonoBehaviour
    {
        public SpineSetting Setting;
        
        private Dictionary<string, SpineAnimationData> AnimationDataDict = new();

        private void Awake()
        {
            if (this.Setting != null)
            {
                foreach (SpineAnimationData spineAnimationData in this.Setting.SpineAnimationDataList)
                {
                    this.AnimationDataDict.Add(spineAnimationData.Name,spineAnimationData);
                }
            }
        }
        
        public SpineAnimationData GetAnimationData(string name)
        {
            this.AnimationDataDict.TryGetValue(name, out SpineAnimationData spineAnimationData);
            return spineAnimationData;
        }
    }
}

