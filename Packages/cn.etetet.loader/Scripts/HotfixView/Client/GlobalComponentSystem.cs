using Unity.Cinemachine;
using UnityEngine;

namespace ET
{
    [EntitySystemOf(typeof(GlobalComponent))]
    public static partial class GlobalComponentSystem
    {
        [EntitySystem]
        private static void Awake(this GlobalComponent self)
        {
            self.Global = GameObject.Find("/Global").transform;
            self.Unit = GameObject.Find("/Global/Unit").transform;
            self.Camera = GameObject.Find("/Global/Virtual Camera").GetComponent<CinemachineCamera>();
            self.GlobalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
        }
    }
}