using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;

namespace ET.Client
{
    [EntitySystemOf(typeof(MapManageComponent))]
    [FriendOf(typeof(MapManageComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapChunkControllerComponent))]
    public static partial class MapManageComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MapManageComponent self)
        {
            self.DataInit();
            
            self.chunkSizeOnWorld =  self.mapChunkSize * self.cellSize;
            self.mapSizeOnWorld =  self.chunkSizeOnWorld *  self.mapSize;
            self.ShareMesh = self.GenerateGroundMesh(self.mapSizeOnWorld,self.mapSizeOnWorld);
            // 生成地面碰撞体的网格
            self.MapManage.meshCollider.sharedMesh = self.ShareMesh;
            // self.MapManage.BakeNavMesh();
            
            self.CreateVisibleChunk().NoContext();
            self.UpdateVisibleChunk().NoContext();
            AstarPath.active?.Scan();
        }

        public static void DataInit(this MapManageComponent self)
        {
            //获取地图的参数
            self.NewGenerateMap().NoContext();
            self.mapChunkDic = new Dictionary<Vector2Int, EntityRef<MapChunkControllerComponent>> { };
            self.lastVisibleChunkList = new List<EntityRef<MapChunkControllerComponent>> { };
            self.viewDinstance = self.MapManage.viewDinstance;
            self.updateChunkTime = self.MapManage.updateChunkTime;
            self.chunkSizeOnWord = self.MapManage.chunkSizeOnWord;
            self.mapSize = self.MapManage.mapSize;
            self.mapChunkSize = self.MapManage.mapChunkSize;
            self.cellSize = self.MapManage.cellSize;
            self.noiseLacunarity = self.MapManage.noiseLacunarity;
            self.mapSeed = self.MapManage.mapSeed;
            self.spawnSeed = self.MapManage.spawnSeed;
            self.marshLimit = self.MapManage.marshLimit;
            self.mapMaterial = self.MapManage.mapMaterial;
            self.marshMaterial = self.MapManage.marshMaterial;
            self.forestTextures = self.MapManage.forestTextures;
            self.marshTextures = self.MapManage.marshTextures;
            Debug.Log("地图管理器 初始化: mapsize：" + self.mapSize + " Mapchunksize: " + self.mapChunkSize);
            self.lastViewerPos = self.MapManage.lastViewerPos;
            self.MapManage = self.MapManage;
            
        }

        [EntitySystem]
        private static void Update(this ET.Client.MapManageComponent self)
        {

        }
        [EntitySystem]
        private static void LateUpdate(this ET.Client.MapManageComponent self)
        {

        }

        //数据刷新 查找地图生成器
        public static async ETTask NewGenerateMap(this MapManageComponent self)
        {
            Debug.Log("地图生成器：开始生成地图");

            //生成地图
            self.MapManage = GameObject.Find("MapManage").GetComponent<MapManage>();//<MapManage>();// map.GetComponent<MapManage>();
            if (self.MapManage == null)
            {
                Debug.Log("地图生成器：没有 找到MapManage");

            }
            else
            {
                Debug.Log("地图生成器： 找到了MapManage");
            }
            
            //await self.SpawnMapObject();

            await ETTask.CompletedTask;

        } 


        #region 根据观察者的位置来刷新哪些地图块可以看到
        //根据观察者的位置来刷新哪些地图块可以看到
        public static async ETTask UpdateVisibleChunk(this MapManageComponent self)
        {
            // 检查组件是否已销毁
            if (self.IsDisposed)
            {
                Log.Debug("UpdateVisibleChunk: MapManageComponent已销毁，取消执行");
                return;
            }
            
            self.TimerId = self.Root().GetComponent<TimerComponent>().NewOnceTimer(
                tillTime: TimeInfo.Instance.ClientNow() + 500, 
                TimerInvokeType.MapUpdate, self);

            if (self.MapManage.viewer == null)
            {
                return;
            }
            if (self.MapManage.viewer.position == self.MapManage.lastViewerPos)
                return;
            Vector2Int currChunkIndex = self.GetMapChunkIndexByWorldPosition(self.MapManage.viewer.position);
            for (int i = self.lastVisibleChunkList.Count - 1; i >= 0; i--)
            {
                MapChunkControllerComponent mapChunkControllerComponent = self.lastVisibleChunkList[i];
                Vector2Int chunkIndex = mapChunkControllerComponent.ChunkIndex;
                if (Mathf.Abs(chunkIndex.x - currChunkIndex.x) > self.viewDinstance
                    || Mathf.Abs(chunkIndex.y - currChunkIndex.y) > self.viewDinstance)
                {
                    mapChunkControllerComponent.SetActive(false);
                    self.lastVisibleChunkList.RemoveAt(i);
                }
            }
            int startX = currChunkIndex.x - self.viewDinstance;
            int startY = currChunkIndex.y - self.viewDinstance;
            // 显示在视野范围内的地图块
            for (int x = 0; x < 2 * self.viewDinstance + 1; x++)
            {
                for (int y = 0; y < 2 * self.viewDinstance + 1; y++)
                {
                    Vector2Int chunkIndex = new Vector2Int(startX + x, startY + y);
                    if (self.mapChunkDic.TryGetValue(chunkIndex, out EntityRef<MapChunkControllerComponent> unitTemp))
                    {
                        MapChunkControllerComponent chunk = unitTemp;
                        if (self.lastVisibleChunkList.Contains(unitTemp) == false)
                        {
                            self.lastVisibleChunkList.Add(unitTemp);
                            chunk.SetActive(true);
                        }
                    }
                    else
                    {
                        //await self.GenerateMapChunk(chunkIndex);
                    }
                    
                    // 检查组件是否已销毁
                    if (self.IsDisposed)
                    {
                        Log.Debug("UpdateVisibleChunk: MapManageComponent在循环中已销毁，取消执行");
                        return;
                    }
                    await self.Root().GetComponent<TimerComponent>().WaitFrameAsync();
                }
            }
        }

        public static async ETTask CreateVisibleChunk(this MapManageComponent self)
        {
            // 检查组件是否已销毁
            if (self.IsDisposed)
            {
                Log.Debug("CreateVisibleChunk: MapManageComponent已销毁，取消执行");
                return;
            }
            
            // Log.Debug("开始创造地图");
            Vector2Int currChunkIndex = new Vector2Int(0, 0);// self.GetMapChunkIndexByWorldPosition(self.MapManage.viewer.position);
            int startX = currChunkIndex.x - self.viewDinstance;
            int startY = currChunkIndex.y - self.viewDinstance;
            // 显示在视野范围内的地图块
            for (int x = 0; x < 2 * 10 + 1; x++)
            {
                for (int y = 0; y < 2 * 10; y++)
                {
                    Vector2Int chunkIndex = new Vector2Int(startX + x, startY + y);
                    
                    //Log.Debug("开始创造地图1");
                    await self.GenerateMapChunk(chunkIndex);
                    
                    // 检查组件是否已销毁
                    if (self.IsDisposed)
                    {
                        Log.Debug("CreateVisibleChunk: MapManageComponent在循环中已销毁，取消执行");
                        return;
                    }
                    await self.Root().GetComponent<TimerComponent>().WaitFrameAsync();
                }
            }
        }

        #endregion

        /// <summary>
        /// 通过世界地图的坐标获取地图的索引
        /// </summary>
        public static Vector2Int GetMapChunkIndexByWorldPosition(this MapManageComponent self, Vector3 worldPostion)
        {
            int x = Mathf.Clamp(Mathf.FloorToInt(worldPostion.x / self.chunkSizeOnWord), 1, self.mapSize);
            int y = Mathf.Clamp(Mathf.FloorToInt(worldPostion.z / self.chunkSizeOnWord), 1, self.mapSize);
            return new Vector2Int(x, y);
        }

       
        /// <summary>
        /// 生成地图块
        /// </summary>
        public static async ETTask GenerateMapChunk(this MapManageComponent self, Vector2Int index)
        {
            // 检查组件是否已销毁
            if (self.IsDisposed)
            {
                Log.Debug("GenerateMapChunk: MapManageComponent已销毁，取消执行");
                return;
            }
            
            if (index.x > self.mapSize || index.y > self.mapSize) return;
            if (index.x < 0 || index.y < 0)
                return;
            MapGeneratorComponent mapGenerator = self.Root().CurrentScene().GetComponent<MapGeneratorComponent>(); 
            await mapGenerator.GenerateMapChunk(index, self.MapManage.transform);
        }
        
        /// <summary>
        /// 生成地面Mesh AI导航所需
        /// </summary>
        private static Mesh GenerateGroundMesh(this MapManageComponent self,float height, float wdith)
        {
            Mesh mesh = new Mesh();
            // 确定顶点在哪里
            mesh.vertices = new Vector3[]
            {
                new Vector3(0,0,0),
                new Vector3(0,0,height),
                new Vector3(wdith,0,height),
                new Vector3(wdith,0,0),
            };
            // 确定哪些点形成三角形
            mesh.triangles = new int[]
            {
                0,1,2,
                0,2,3
            };
            mesh.uv = new UnityEngine.Vector2[]
            {
                new UnityEngine.Vector3(0,0),
                new UnityEngine.Vector3(0,1),
                new UnityEngine.Vector3(1,1),
                new UnityEngine.Vector3(1,0),
            };
            return mesh;
        }

        [EntitySystem]
        private static void Destroy(this MapManageComponent self)
        {
            try
            {
                Log.Debug("开始销毁MapManageComponent");
                
                // 清理定时器
                if (self.TimerId > 0)
                {
                    var timerComponent = self.Root()?.GetComponent<TimerComponent>();
                    if (timerComponent != null)
                    {
                        timerComponent.Remove(ref self.TimerId);
                        Log.Debug("已清理MapUpdateTimer定时器");
                    }
                }
                
                // 清理地图块
                if (self.mapChunkDic != null)
                {
                    Log.Debug($"开始清理{self.mapChunkDic.Count}个地图块");
                    foreach (var chunkRef in self.mapChunkDic.Values)
                    {
                        MapChunkControllerComponent chunk = chunkRef;
                        if (chunk != null && !chunk.IsDisposed)
                        {
                            if (chunk.GameObject != null)
                            {
                                UnityEngine.Object.Destroy(chunk.GameObject);
                            }
                            chunk.Dispose();
                        }
                    }
                    self.mapChunkDic.Clear();
                }
                
                // 清理可见块列表
                if (self.lastVisibleChunkList != null)
                {
                    self.lastVisibleChunkList.Clear();
                    Log.Debug("已清理可见块列表");
                }
                
                Log.Debug("MapManageComponent销毁完成");
            }
            catch (System.Exception ex)
            {
                Log.Error($"销毁MapManageComponent时发生错误：{ex.Message}");
            }
        }
    }
}
