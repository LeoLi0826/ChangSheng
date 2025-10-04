using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ChildOf(typeof(MapManageComponent))]
    public class MapChunkControllerComponent : Entity, IAwake, IUpdate, ILateUpdate
    {
        public Vector2Int ChunkIndex;// { get; private set; }
        public List<EntityRef<MapRegionUnit>> MapRegionUnitList = new List<EntityRef<MapRegionUnit>>();
        public List<EntityRef<MapRegionStatic>> MapRegionStaticList = new List<EntityRef<MapRegionStatic>>();
        public List<MapVertex> ForestVertexList;
        public List<MapVertex> MarhshVertexList;

        public GameObject GameObject;
        
        
        public Vector3 CentrePosition;// { get; private set; }
        
        public bool isActive = false;

        //递归数据
        public int maxRecursionDepth = 10;
        public int currentRecursionDepth = 0;
        
        public MapVertexType MapVertex1;
        public MapVertexType MapVertex2;

    }

}