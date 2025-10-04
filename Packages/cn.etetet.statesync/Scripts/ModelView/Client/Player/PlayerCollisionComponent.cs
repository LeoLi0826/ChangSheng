using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf]
    public class PlayerCollisionComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public PlayerBehaviour playerBehaviour;
        public UnitConfig unitConfig;
    }
}