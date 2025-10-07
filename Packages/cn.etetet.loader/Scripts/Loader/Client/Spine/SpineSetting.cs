using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ET.Client
{
    public enum SpineAnimationDirection
    {
        None = 0,
        Down = 1,
        Up = 2,
        Left = 3,
        Right = 4,
    }
    
    public class SpineAnimationData
    {
        [LabelText("播放名称")]
        public string Name;
        
        [LabelText("是否四方向动画")]
        public bool IsFourDirection;
        
        [HideIf("IsFourDirection")]
        [LabelText("动画名称")]
        public string AnimationName;
        
        [ShowIf("IsFourDirection")]
        [LabelText("四方向动画列表")]
        public Dictionary<SpineAnimationDirection, string> FourDirectionAnimationName;

        [LabelText("是否循环")]
        public bool IsLoop;
        
        public string GetAnimationName(Vector2 direction)
        {
            if (this.IsFourDirection)
            {
                //根据方向获取动画名称，方向是个一个向量，方向可以是八方向位移，但是动画只有4个
                if (direction.y > 0)
                {
                    return this.FourDirectionAnimationName[SpineAnimationDirection.Up];
                }
                if (direction.y < 0)
                {
                    return this.FourDirectionAnimationName[SpineAnimationDirection.Down];
                }
                if (direction.x > 0)
                {
                    return this.FourDirectionAnimationName[SpineAnimationDirection.Right];
                }
                if (direction.x < 0)
                {
                    return this.FourDirectionAnimationName[SpineAnimationDirection.Left];
                }
               
            }
            return this.AnimationName;
        }
        
        public string GetAnimationName(int direction)
        {
            return this.FourDirectionAnimationName[(SpineAnimationDirection)direction];
        }
    }
    
    [CreateAssetMenu]
    public class SpineSetting : SerializedScriptableObject
    {
        [LabelText("动画数据列表")]
        [ListDrawerSettings(ListElementLabelName = "Name")]
        public List<SpineAnimationData> SpineAnimationDataList = new();
    }
}

