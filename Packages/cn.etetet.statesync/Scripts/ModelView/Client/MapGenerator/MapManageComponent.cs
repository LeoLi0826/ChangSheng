using System;
using System.Collections.Generic;
using UnityEngine;


namespace ET.Client
{
    public struct RefreshMapChunk
    {
        public MapChunkControllerComponent unit;
    }
    
    
    [ComponentOf(typeof(Scene))]
    public class MapManageComponent: Entity, IAwake, IDestroy
    {

        public long TimerId;
        #region 通用
        public MapManage MapManage;
        public Mesh ShareMesh;

        public float chunkSizeOnWorld;
        public float mapSizeOnWorld; // 在世界中实际的地图整体尺寸
        
        public EntityRef<MapChunkControllerComponent> MapChunkController;
        public EntityRef<MapGeneratorComponent> mapGenerator;
        #endregion
        
        
        #region 九宫格地图生成器
        // 地图尺寸
        public int mapSize; // 一行或者一列有多少个地图块
        public int mapChunkSize; // 一个地图块有多少个格子
        public float cellSize; // 一个格子多少米

        // 地图的随机参数
        public float noiseLacunarity;  // 噪音间隙
        public int mapSeed;            // 地图种子
        public int spawnSeed;          // 随时地图对象的种子
        public float marshLimit;       // 沼泽的边界

        //地图的美术资源
        public MapGrid mapGrid; // 地图逻辑网格、顶点数据
        public Material mapMaterial;
        public Material marshMaterial;

        public Texture2D[] forestTextures; // 改为数组形式
        public Texture2D[] marshTextures;
        
        public int viewDinstance; // 玩家可视距离 单位是Chunk
        //public Transform viewer; // 观察者
        public Vector3 lastViewerPos = Vector3.one * -1;
        
        public float updateChunkTime = 1f;
        public float chunkSizeOnWord; // 在世界中实际的地图块尺寸 单位米
        
        //全部地图块
        public Dictionary<Vector2Int, EntityRef<MapChunkControllerComponent>> mapChunkDic; // 全部已有的地图块
        //显示的地图块
        public List<EntityRef<MapChunkControllerComponent>>  lastVisibleChunkList = new List<EntityRef<MapChunkControllerComponent>>();
        
       
        #endregion
        
       
    }
}
