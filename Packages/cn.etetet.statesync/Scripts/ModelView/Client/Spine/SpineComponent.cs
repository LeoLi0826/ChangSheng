using System.Collections.Generic;
using Spine;
using Spine.Unity;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class SpineComponent: Entity, IAwake<SkeletonAnimation>, IDestroy
    {
        public SkeletonAnimation SkeletonAnimation;
        
        public string CurrentAnimationName;
        
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
    }
}

