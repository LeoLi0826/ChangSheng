using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [ChildOf(typeof(MapChunkControllerComponent))]
    public class MapRegionUnit : Entity,IAwake
    {
        public MapVertexType MapVertexType;
        public int ConfigId;
        public long UnitId;
        public float3 Position = float3.zero;
        public long NextUpdateTime;
        public long LastTimer = 0;
    }

    [ChildOf(typeof(MapChunkControllerComponent))]
    public class MapRegionStatic : Entity, IAwake,IDestroy
    {
        public MapVertexType MapVertexType;
        public int ConfigId;
        public float3 Position = float3.zero;
        public GameObject GameObject;
    }
    
    
    
}

