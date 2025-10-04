using Unity.Cinemachine;
using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent: Entity, IAwake
    {
        public Transform Global;
        public Transform Unit { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        
        public CinemachineCamera Camera { get; set; }
    }
}