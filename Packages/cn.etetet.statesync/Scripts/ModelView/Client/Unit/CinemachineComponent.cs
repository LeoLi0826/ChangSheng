using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class CinemachineComponent: Entity, IAwake,IDestroy
    {
        public Transform Follow { get; set; }

        public Transform Head { get; set; }
    }
}

