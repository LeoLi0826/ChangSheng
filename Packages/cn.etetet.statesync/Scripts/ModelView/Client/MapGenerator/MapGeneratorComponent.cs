using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class MapGeneratorComponent : Entity, IAwake
    {
        public MapManage MapManage;
        
        public EntityRef<Unit> MyUnit;
        public int MapNum;
        public MapVertexType MapVertex1;
        public MapVertexType MapVertex2;

        
        public GameObject unitParent;
        //public bool FacingFlag;
        //public List<GameObject> mapObjects = new List<GameObject>();

        // 整个地图是方的，就是地图块、格子、贴图都是正方形
        public int mapSize; // 一行或者一列有多少个地图块

        public Mesh ShareMesh;
        
        public EntityRef<MapChunkControllerComponent> mapChunk;
       
        public int mapChunkSize; // 一个地图块有多少个格子
        public float cellSize; // 一个格子多少米
        public float mapSizeOnWorld; // 在世界中实际的地图整体尺寸
        public float chunkSizeOnWorld; // 在世界中实际的地图块尺寸 单位米
        
        public float noiseLacunarity; // 噪音间隙
        public int mapSeed; // 地图种子
        public int spawnSeed; // 随时地图对象的种子
        public float marshLimit; // 沼泽的边界
        public MapGrid mapGrid; // 地图逻辑网格、顶点数据
        public Material mapMaterial;
        public Material marshMaterial;

        public long CurrentID; //生成物体元素的下标

        public Texture2D[] forestTextures; // 改为数组形式
        public Texture2D[] marshTextures;
        public int forestSpawanWeightTotal;
        public int marshSpawanWeightTotal;
        
        public int AIforestSpawanWeightTotal;
        public int AImarshSpawanWeightTotal;

        // 标记mapGrid是否已初始化
        public bool IsMapGridInitialized = false;
        
        
    }
}