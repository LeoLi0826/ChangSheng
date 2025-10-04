using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class MapManage : MonoBehaviour
    {
        
        #region 运行时的变量
        [SerializeField] public MeshCollider meshCollider;
        public Vector3 lastViewerPos = Vector3.one * -1;

        [Tooltip("观察者")]
        public Transform viewer;         
        public float updateChunkTime = 1f;
        public bool canUpdateChunk = true;
        [Tooltip("在世界中实际的地图整体尺寸")]
        public float mapSizeOnWorld;    // 在世界中实际的地图整体尺寸
        [Tooltip("在世界中实际的地图块尺寸 单位米")]
        public float chunkSizeOnWorld;  // 在世界中实际的地图块尺寸 单位米
       #endregion
        
        #region 运行时的逻辑
        [Tooltip(" 地图逻辑网格、顶点数据")]
        public MapGrid mapGrid;        // 地图逻辑网格、顶点数据
        public Material marshMaterial;
        public Mesh chunkMesh;
        public int forestSpawanWeightTotal;
        public int marshSpawanWeightTotal;
        #endregion
        
        
        // 地图尺寸
        [Tooltip("一行或者一列有多少个地图块")]
        public int mapSize; // 一行或者一列有多少个地图块
        [Tooltip("一个地图块有多少个格子")]
        public int mapChunkSize; // 一个地图块有多少个格子
        [Tooltip("一个格子多少米")]
        public float cellSize; // 一个格子多少米
        
        // 地图的随机参数
        [Tooltip("噪音间隙")]
        public float noiseLacunarity;  // 噪音间隙
        [Tooltip("地图种子")]
        public int mapSeed;            // 地图种子
        [Tooltip("随时地图对象的种子")]
        public int spawnSeed;          // 随时地图对象的种子
        [Tooltip("沼泽的边界")]
        public float marshLimit;       // 沼泽的边界

        //地图的美术资源
        //public MapGrid mapGrid; // 地图逻辑网格、顶点数据
        public Material mapMaterial;
       
        public Texture2D[] forestTextures; // 改为数组形式
        public Texture2D[] marshTextures;
        
        [Tooltip("玩家可视距离 单位是Chunk")]
        public int viewDinstance; // 玩家可视距离 单位是Chunk
        
        //这个在MapManage里面设置
        //public Dictionary<Vector2Int, MapChunkController> mapChunkDic; // 全部已有的地图块

        public float chunkSizeOnWord; // 在世界中实际的地图块尺寸 单位米
        //public List<MapChunkController> lastVisibleChunkList = new List<MapChunkController>();

        // public virtualCameraSet virtualCameraSet;

        // public GameObject TopConfiner;
        // public GameObject BottomConfiner;
        // public GameObject LeftConfiner;
        // public GameObject RightConfiner;
        
        //AI怪物生成数量限制
        [Tooltip("AI数量限制")]
        public int MaxAIOnChunk;
        //地图块森林/沼泽生成AI的最小顶点数
        [Tooltip("地图块森林/沼泽生成AI的最小顶点数")]
        public int GenerateAIMinVertexCountOnChunk;

        void Start()
        {
            // 初始化地图生成器
            //mapGenerator = new MapGenerator(mapSize, mapChunkSize, cellSize, noiseLacunarity, mapSeed, spawnSeed, marshLimit, mapMaterial,
            //    forestTextures, marshTextures);
            //mapGenerator.GenerateMapData();
            //mapChunkDic = new Dictionary<Vector2Int, MapChunkController>();
            chunkSizeOnWord = mapChunkSize * cellSize;
            Log.Debug("我开始管理map");
            // virtualCameraSet.Camerainit(chunkSizeOnWord*mapSize);
        }

        // Update is called once per frame
        void Update()
        {
            //UpdateVisibleChunk();
        }

        //根据观察者的位置来刷新哪些地图块可以看到
        // public void UpdateVisibleChunk()
        // {
        //     // 如果观察者没有移动过，不需要刷新
        //     if (viewer.position == lastViewerPos) return;
        //     // 如果时间未到 不允许刷新
        //     if (canUpdateChunk == false) return;
        //
        //     // 当前玩家所在的地图块 
        //     Vector2Int currChunkIndex = GetMapChunkIndexByWorldPosition(viewer.position);
        //
        //     // 关闭不在视野范围内的地图块
        //     for (int i = lastVisibleChunkList.Count - 1; i >= 0; i--)
        //     {
        //         Vector2Int chunkIndex = lastVisibleChunkList[i].ChunkIndex;
        //         if (Mathf.Abs(chunkIndex.x - currChunkIndex.x) > viewDinstance
        //             || Mathf.Abs(chunkIndex.y - currChunkIndex.y) > viewDinstance)
        //         {
        //             lastVisibleChunkList[i].SetActive(false);
        //             lastVisibleChunkList.RemoveAt(i);
        //         }
        //     }
        //
        //     int startX = currChunkIndex.x - viewDinstance;
        //     int startY = currChunkIndex.y - viewDinstance;
        //     // 显示在视野范围内的地图块
        //     for (int x = 0; x < 2 * viewDinstance + 1; x++)
        //     {
        //         for (int y = 0; y < 2 * viewDinstance + 1; y++)
        //         {
        //             canUpdateChunk = false;
        //             Invoke("RestCanUpdateChunkFlag", updateChunkTime);
        //             Vector2Int chunkIndex = new Vector2Int(startX + x, startY + y);
        //             // ֮之前加载过
        //             if (mapChunkDic.TryGetValue(chunkIndex, out MapChunkController chunk))
        //             {
        //                 // 这个地图是不是已经在显示列表
        //                 if (lastVisibleChunkList.Contains(chunk) == false)
        //                 {
        //                     lastVisibleChunkList.Add(chunk);
        //                     chunk.SetActive(true);
        //                 }
        //             }
        //             // ֮之前没有加载
        //             else
        //             {
        //                 chunk = GenerateMapChunk(chunkIndex);
        //                 if (chunk != null)
        //                 {
        //                     chunk.SetActive(true);
        //                     lastVisibleChunkList.Add(chunk);
        //                 }
        //             }
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// 通过世界地图的坐标获取地图的索引
        // /// </summary>
        // public Vector2Int GetMapChunkIndexByWorldPosition(Vector3 worldPostion)
        // {
        //     int x = Mathf.Clamp(Mathf.RoundToInt(worldPostion.x / chunkSizeOnWord), 1, mapSize);
        //     int y = Mathf.Clamp(Mathf.RoundToInt(worldPostion.z / chunkSizeOnWord), 1, mapSize);
        //     return new Vector2Int(x, y);
        // }
        //
        // /// <summary>
        // /// 生成地图块
        // /// </summary>
        // public MapChunkController GenerateMapChunk(Vector2Int index)
        // {
        //     // 检查坐标合理性
        //     if (index.x > mapSize || index.y > mapSize) return null;
        //
        //     if (index.x < 0 || index.y < 0) return null;
        //     MapChunkController chunk = mapGenerator.GenerateMapChunk(index, transform);
        //     mapChunkDic.Add(index, chunk);
        //     return chunk;
        // }
        //
        //
        // private void RestCanUpdateChunkFlag()
        // {
        //     canUpdateChunk = true;
        // }
    }
}
