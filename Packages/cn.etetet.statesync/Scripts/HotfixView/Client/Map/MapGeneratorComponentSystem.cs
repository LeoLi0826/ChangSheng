using System;
using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace ET.Client
{
    [EntitySystemOf(typeof(MapGeneratorComponent))]
    [FriendOf(typeof(MapGeneratorComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapManageComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapChunkControllerComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionStatic))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionUnit))]
    public static partial class MapGeneratorComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MapGeneratorComponent self)
        {
            self.MapManage = GameObject.Find("MapManage").GetComponent<MapManage>();
            self.dataInit();

            self.MapResInit();

            Debug.Log("地块1：" + self.MapVertex1 + "地块2：" + self.MapVertex2);
            List<MapConfig> temps = MapConfigCategory.Instance.GetListByMapVertexType((int)self.MapVertex1);
            if (temps != null)
            {
                for (int i = 0; i < temps.Count; i++)
                {
                    self.forestSpawanWeightTotal += temps[i].Probability;
                }
            }


            //Debug.Log("森林概率权重： " + self.forestSpawanWeightTotal);
            temps = MapConfigCategory.Instance.GetListByMapVertexType((int)self.MapVertex2);
            if (temps != null)
            {
                for (int i = 0; i < temps.Count; i++)
                {
                    self.marshSpawanWeightTotal += temps[i].Probability;
                }
            }

            //Debug.Log("沼泽概率权重： " + self.marshSpawanWeightTotal);

            // 计算AI生成权重
            List<MapAIConfig> forestMapAIConfigs = MapAIConfigCategory.Instance.GetListByMapVertexType((int)self.MapVertex1);
            if (forestMapAIConfigs != null)
            {
                for (int i = 0; i < forestMapAIConfigs.Count; i++)
                {
                    self.AIforestSpawanWeightTotal += forestMapAIConfigs[i].Probability;
                }
            }


            //Debug.Log("森林AI概率权重： " + self.AIforestSpawanWeightTotal);

            List<MapAIConfig> marshMapAIConfigs = MapAIConfigCategory.Instance.GetListByMapVertexType((int)self.MapVertex2);
            if (marshMapAIConfigs != null)
            {
                for (int i = 0; i < marshMapAIConfigs.Count; i++)
                {
                    self.AImarshSpawanWeightTotal += marshMapAIConfigs[i].Probability;
                }
            }

            //Debug.Log("沼泽AI概率权重： " + self.AImarshSpawanWeightTotal);

            GenerateMapData(self);

            // 标记mapGrid已初始化
            self.IsMapGridInitialized = true;

            self.ShareMesh = self.GenerateMapMesh(self.mapChunkSize, self.mapChunkSize, self.cellSize);

            AstarPath.active?.Scan();

        }

        //地图类型初始化
        public static void MapResInit(this ET.Client.MapGeneratorComponent self)
        {
           // Debug.Log("地块开始： " + self.Root().CurrentScene().Name);
            //更新玩家所在地图，后面更新地图资源
            // self.MyUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
            //
            // Unit unit = self.MyUnit;
            // self.MapNum = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Map);
            //
            //这里 暂时稍微给写死了 后期可以改地图的初始地
            switch (self.Root().CurrentScene().Name)
            {
                case "Map1":
                    
                    self.MapNum = 1;
                    // AudioSourceHelper.StopAll(self.Root());
                    // AudioSourceHelper.PlayBgm(self.Root(), AudioClipType.HFS_Bgm,true,0.3f);
                    break;
                case "Map2":
                    self.MapNum = 2;
                    // AudioSourceHelper.StopAll(self.Root());
                    // AudioSourceHelper.PlayBgm(self.Root(), AudioClipType.HYS_Bgm,true,0.3f);
                    break;
                case "Map3":
                    self.MapNum = 3;
                    // AudioSourceHelper.StopAll(self.Root());
                    // AudioSourceHelper.PlayBgm(self.Root(), AudioClipType.HFL_Bgm,true,0.3f);
                    break;
                case "Map4":
                    self.MapNum = 4;
                    // AudioSourceHelper.StopAll(self.Root());
                    // AudioSourceHelper.PlayBgm(self.Root(), AudioClipType.HFS_Bgm,true,0.3f);
                    break;
                case "Map5":
                    self.MapNum = 5;
                    break;
                case "Map6":
                    self.MapNum = 6;
                    break;
                case "Map7":
                    self.MapNum = 7;
                    break;

            }

            self.MapVertex1 = (MapVertexType)(self.MapNum * 2 - 1);
            self.MapVertex2 = (MapVertexType)(self.MapNum * 2);
        }

        public static void dataInit(this ET.Client.MapGeneratorComponent self)
        {
            // self.FacingFlag = false;

            self.mapSize = self.MapManage.mapSize;
            self.mapChunkSize = self.MapManage.mapChunkSize;
            self.cellSize = self.MapManage.cellSize;
            self.noiseLacunarity = self.MapManage.noiseLacunarity;
            self.mapSeed = self.MapManage.mapSeed;
            self.spawnSeed = self.MapManage.spawnSeed;
            self.marshLimit = self.MapManage.marshLimit;
            self.mapMaterial = self.MapManage.mapMaterial;
            self.forestTextures = self.MapManage.forestTextures;
            self.marshTextures = self.MapManage.marshTextures;

            self.forestSpawanWeightTotal = self.MapManage.forestSpawanWeightTotal;
            self.marshSpawanWeightTotal = self.MapManage.marshSpawanWeightTotal;

            self.CurrentID = 1;//物体元素下标

            //Debug.Log("地图生成器 初始化: mapsize：" + self.mapSize + " Mapchunksize: " + self.mapChunkSize + "marshTextures的个数：" + self.marshTextures.Length);

            self.unitParent = GameObject.FindGameObjectWithTag("MapObj"); //<MapManage>();// map.GetComponent<MapManage>();

        }


        [EntitySystem]
        private static void Update(this MapGeneratorComponent self)
        {
            // if (self.FacingFlag == true)
            // {
            //     Debug.Log("我开始设置朝向: "+Camera.main.transform.rotation);
            //     foreach (var temp in self.mapObjects)
            //     {
            //         temp.transform.rotation = Camera.main.transform.rotation;
            //
            //     }
            //
            //     self.FacingFlag = false;
            // }
        }

        [EntitySystem]
        private static void LateUpdate(this ET.Client.MapGeneratorComponent self)
        {

        }

        /// <summary>
        /// 生成地图
        /// </summary>
        public static void GenerateMapData(this ET.Client.MapGeneratorComponent self)
        {
            Debug.Log($"生成地图 - MapNum: {self.MapNum}");
            // 应用地图种子，基于地图编号生成独特种子确保每个地图有不同的地形布局
            Random.InitState(self.mapSeed + self.MapNum);

            // 生成噪声图
            float[,] noiseMap = self.GenerateNoiseMap(self.mapSize * self.mapChunkSize, self.mapSize * self.mapChunkSize, self.noiseLacunarity);
            // 生成网格数据
            self.mapGrid = new MapGrid(self.mapSize * self.mapChunkSize, self.mapSize * self.mapChunkSize, self.cellSize);
            // 确定网格 格子的贴图索引
            self.mapGrid.CalculateMapVertexType(noiseMap, self.marshLimit, self.MapNum, self.MapVertex1, self.MapVertex2);
            // 初始化默认材质的尺寸
            //self.mapMaterial.mainTexture = self.forestTextures[self.GetRandomTextureIndex1()];
            self.mapMaterial.SetTextureScale("_MainTex", new Vector2(self.cellSize * self.mapChunkSize, self.cellSize * self.mapChunkSize));
            // 实例化一个沼泽材质
            self.marshMaterial = new Material(self.mapMaterial);
            self.marshMaterial.SetTextureScale("_MainTex", Vector2.one);

            Random.InitState(self.spawnSeed);


        }

        /// <summary>
        /// 生成地图块
        /// </summary>
        public static async ETTask GenerateMapChunk(this MapGeneratorComponent self, Vector2Int chunkIndex,
        Transform parent)
        {
            MapManageComponent mapManageComponent = self.Scene().GetComponent<MapManageComponent>();
            //Debug.Log("生成地图1");
            // 生成地图块物体
            GameObject mapChunkObj = new GameObject("Chunk_" + chunkIndex.ToString());

            MapChunkControllerComponent mapChunkController = mapManageComponent.AddChild<MapChunkControllerComponent>();
            mapChunkController.GameObject = mapChunkObj;

            // 生成Mesh
            mapChunkObj.AddComponent<MeshFilter>().mesh = self.ShareMesh;
            mapChunkObj.AddComponent<MeshCollider>();
            // 生成地图块的贴图
            self.GenerateMapHelper(chunkIndex, mapChunkController);

            // 确定坐标
            Vector3 position = new Vector3(chunkIndex.x * self.mapChunkSize * self.cellSize, 0, chunkIndex.y * self.mapChunkSize * self.cellSize);
            mapChunkObj.transform.position = position;
            //??????挂载的对象？
            mapChunkObj.transform.SetParent(parent);

            // 生成场景物体
            // SpawnMapObject(self.mapGrid, mapConfig, spawnSeed);
            // 生成场景物体数据

            self.GenerateMapChunkData(mapChunkController, chunkIndex, position);
            // List<MapObjectData> mapObjectModelList = self.GenerateMapChunkData(chunkIndex);
            // List<MapObjectData> mapAIObjectModelList = self.SpawnMapAIObject(chunkIndex);
            MapManageComponent manageComponent = self.Root().CurrentScene().GetComponent<MapManageComponent>();
            manageComponent.mapChunkDic.Add(chunkIndex, mapChunkController);

            //Debug.Log("之后的字典个数： count: "+mapChunkData.MapObjectDic.Count);


            // EventSystem.Instance.PublishAsync(mapChunkController.Scene(), new RefreshMapChunk()
            // { unit = mapChunkController }).Coroutine();

            //Debug.Log("生成地图4");
        }

        /// <summary>
        // /// 生成地形Mash
        // /// </summary>
        private static Mesh GenerateMapMesh(this ET.Client.MapGeneratorComponent self, int height, int width, float cellSize)
        {
            //Debug.Log("生成地形Mash1");
            Mesh mesh = new Mesh();
            // 确定顶点在哪里
            mesh.vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, height * self.cellSize),
                new Vector3(width * self.cellSize, 0, height * self.cellSize),
                new Vector3(width * self.cellSize, 0, 0),
            };

            // 确定哪些点形成三角形
            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            mesh.uv = new Vector2[]
            {
                new Vector3(0, 0),
                new Vector3(0, 1),
                new Vector3(1, 1),
                new Vector3(1, 0),
            };

            //计算法线
            mesh.RecalculateNormals();

            return mesh;
        }

        // <summary>
        // 生成噪声图
        // </summary>
        private static float[,] GenerateNoiseMap(this ET.Client.MapGeneratorComponent self, int width, int height, float lacunarity)
        {
            //Debug.Log("生成噪声图 width: " + width + " height: " + height + " lacunarity: " + lacunarity + " mapSeed: " + mapSeed);

            lacunarity += 0.1f;
            // 这里的噪声图是为了顶点服务的
            float[,] noiseMap = new float[width - 1, height - 1];
            float offsetX = Random.Range(-10000f, 10000f);
            float offsetY = Random.Range(-10000f, 10000f);
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    noiseMap[x, y] = Mathf.PerlinNoise(x * lacunarity + offsetX, y * lacunarity + offsetY);
                }
            }

            return noiseMap;
        }

        //生成地图
        public static void GenerateMapHelper(this ET.Client.MapGeneratorComponent self, Vector2Int chunkIndex, MapChunkControllerComponent mapChunk)
        {
            //Debug.Log("生成地图2");
            GameObject mapChunkObj = mapChunk.GameObject;
            Texture2D mapTexture;
            self.GenerateMapTexture(chunkIndex, (tex, isAllForest) =>
            {
                // Debug.Log("生成地图3");
                // 无论是全森林还是混合地块，都使用生成的纹理
                mapTexture = tex;
                Material material = new Material(isAllForest ? self.mapMaterial : self.marshMaterial);
                material.mainTexture = tex;
                mapChunkObj.AddComponent<MeshRenderer>().material = material;
                
                //Debug.Log($"地图块 {chunkIndex} 生成完成，类型: {(isAllForest ? "全森林" : "混合")}");

                //*****************************



            }).NoContext();
        }

        /// <summary>
        /// 分帧生成地图 - 优化版本，批量处理减少异步等待
        /// </summary>
        private static async ETTask GenerateMapTexture(this ET.Client.MapGeneratorComponent self, Vector2Int chunkIndex,
        System.Action<Texture2D, bool> callBack)
        {
            int cellOffsetX = chunkIndex.x * self.mapChunkSize + 1;
            int cellOffsetY = chunkIndex.y * self.mapChunkSize + 1;

            // 是不是一张完整的森林地图块
            bool isAllForest = true;
            // 检查是否只有森林类型的格子
            for (int y = 0; y < self.mapChunkSize; y++)
            {
                if (isAllForest == false) break;
                for (int x = 0; x < self.mapChunkSize; x++)
                {
                    MapCell cell = self.mapGrid.GetCell(x + cellOffsetX, y + cellOffsetY);
                    if (cell != null && cell.TextureIndex != 0)
                    {
                        isAllForest = false;
                        break;
                    }
                }
            }

            Texture2D mapTexture = null;
            
            // 无论是全森林还是混合地块，都生成纹理
            // 贴图都是矩形
            Texture2D currentForestTexture = self.forestTextures[self.GetRandomTextureIndex1()];
            int texturecellSize = currentForestTexture.width;
            // 整个地图块的宽高,正方形
            int textureSize = self.mapChunkSize * texturecellSize;
            mapTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);

            if (isAllForest)
            {
                // 全森林地块：从forestTextures中随机选择并拼凑
                Debug.Log($"生成全森林地块纹理，尺寸: {textureSize}x{textureSize}");
                
                // 预缓存森林纹理数据
                Dictionary<int, Color[]> forestPixelsCache = new Dictionary<int, Color[]>();

                // 批量处理参数
                int batchSize = 4; // 每批处理4行
                int processedRows = 0;

                // 按批次处理行，减少异步等待频率
                for (int y = 0; y < self.mapChunkSize; y += batchSize)
                {
                    // 每处理一批次后等待一帧
                    if (processedRows > 0)
                    {
                        await self.Root().GetComponent<TimerComponent>().WaitFrameAsync();
                    }

                    // 处理当前批次的行
                    int endY = Mathf.Min(y + batchSize, self.mapChunkSize);
                    for (int currentY = y; currentY < endY; currentY++)
                    {
                        int pixelOffsetY = currentY * texturecellSize;

                        // 批量处理当前行的所有格子
                        for (int x = 0; x < self.mapChunkSize; x++)
                        {
                            int pixelOffsetX = x * texturecellSize;
                            
                            // 为每个格子随机选择一个森林纹理
                            int textureIndex1 = self.GetRandomTextureIndex1();
                            
                            // 获取或缓存森林纹理数据
                            Color[] selectedForestPixels = null;
                            if (!forestPixelsCache.TryGetValue(textureIndex1, out selectedForestPixels))
                            {
                                selectedForestPixels = self.forestTextures[textureIndex1].GetPixels();
                                forestPixelsCache[textureIndex1] = selectedForestPixels;
                            }
                            
                            // 处理单个格子的像素 - 全森林地块，textureIndex2设为-1表示只有森林
                            ProcessTextureCellOptimized(mapTexture, selectedForestPixels, textureIndex1, forestPixelsCache, self.marshTextures,
                                -1, pixelOffsetX, pixelOffsetY, texturecellSize, self);
                        }
                    }

                    processedRows += (endY - y);
                }
            }
            else
            {
                // 混合地块：包含森林和沼泽
                //Debug.Log($"生成混合地块纹理，尺寸: {textureSize}x{textureSize}");
                
                // 预缓存纹理数据，避免重复获取
                Color[] forestPixels = currentForestTexture.GetPixels();
                Dictionary<int, Color[]> marshPixelsCache = new Dictionary<int, Color[]>();

                // 批量处理参数
                int batchSize = 4; // 每批处理4行
                int processedRows = 0;

                // 按批次处理行，减少异步等待频率
                for (int y = 0; y < self.mapChunkSize; y += batchSize)
                {
                    // 每处理一批次后等待一帧
                    if (processedRows > 0)
                    {
                        await self.Root().GetComponent<TimerComponent>().WaitFrameAsync();
                    }

                    // 处理当前批次的行
                    int endY = Mathf.Min(y + batchSize, self.mapChunkSize);
                    for (int currentY = y; currentY < endY; currentY++)
                    {
                        int pixelOffsetY = currentY * texturecellSize;

                        // 批量处理当前行的所有格子
                        for (int x = 0; x < self.mapChunkSize; x++)
                        {
                            int pixelOffsetX = x * texturecellSize;
                            int originalTextureIndex = self.mapGrid.GetCell(x + cellOffsetX, currentY + cellOffsetY).TextureIndex;
                            //如果是14号贴图，则从15-19里面随机选择
                            int textureIndex1 = self.GetRandomTextureIndex1();
                            int textureIndex2 = self.GetRandomTextureIndex2(originalTextureIndex);

                            // 批量处理像素 - 使用缓存的数据
                            ProcessTextureCellOptimized(mapTexture, forestPixels, textureIndex1, marshPixelsCache, self.marshTextures,
                                textureIndex2, pixelOffsetX, pixelOffsetY, texturecellSize, self);
                        }
                    }

                    processedRows += (endY - y);
                }
            }

            mapTexture.filterMode = FilterMode.Point;
            mapTexture.wrapMode = TextureWrapMode.Clamp;
            mapTexture.Apply();

            callBack?.Invoke(mapTexture, isAllForest);
        }


        /// <summary>
        /// 随机获取一个森林纹理
        /// </summary>

        private static int GetRandomTextureIndex1(this MapGeneratorComponent self)
        {

            if (self.forestTextures == null )
            {
                Debug.Log("森林纹理 为空 ");
                return 0;
            }
            if(self.forestTextures.Length == 0)
            {
                Debug.Log("森林纹理 为空 长度为0");
                return 0;
            }
            return UnityEngine.Random.Range(0, self.forestTextures.Length);
        }

        /// <summary>
        /// 根据TextureIndex获取实际的纹理索引，支持权重为15时的随机选择
        /// </summary>
        private static int GetRandomTextureIndex2(this MapGeneratorComponent self, int originalTextureIndex)
        {
            // 如果原始索引是15（权重为15），则在14-19范围内随机选择
            if (originalTextureIndex == 15)
            {
                // 确保随机范围在可用纹理范围内
                int minIndex = 14;
                int maxIndex = Mathf.Min(19, self.marshTextures.Length - 1);

                // 如果maxIndex小于minIndex，则使用原始逻辑
                if (maxIndex < minIndex)
                {
                    return originalTextureIndex - 1;
                }

                return Random.Range(minIndex, maxIndex + 1);
            }

            // 其他情况使用原始逻辑
            return originalTextureIndex - 1;
        }

        /// <summary>
        /// 优化的纹理单元格处理，使用预缓存的纹理数据
        /// </summary>
        private static void ProcessTextureCellOptimized(Texture2D targetTexture, Color[] forestPixels, int textureIndex1,
            Dictionary<int, Color[]> marshPixelsCache, Texture2D[] marshTextures,
            int textureIndex2, int pixelOffsetX, int pixelOffsetY, int textureCellSize, MapGeneratorComponent self)
        {
            // 获取或缓存沼泽纹理数据
            Color[] marshPixels = null;
            if (textureIndex2 >= 0 && textureIndex2 < marshTextures.Length)
            {
                if (!marshPixelsCache.TryGetValue(textureIndex2, out marshPixels))
                {
                    marshPixels = marshTextures[textureIndex2].GetPixels();
                    marshPixelsCache[textureIndex2] = marshPixels;
                }
            }

            // 创建当前单元格的像素数组
            Color[] cellPixels = new Color[textureCellSize * textureCellSize];
            Color[] selectedForestPixels = new Color[textureCellSize * textureCellSize];
            // 森林纹理 - 根据textureIndex1选择对应的森林纹理
            if (textureIndex1 >= 0 && textureIndex1 < self.forestTextures.Length)
            {
                // 使用指定的森林纹理索引
                selectedForestPixels = self.forestTextures[textureIndex1].GetPixels();

                //color = selectedForestPixels[cellPixelIndex];


            }
            else
            {
                Debug.Log("森林纹理 超出索引");
                return;
            }

            // 批量处理像素
            for (int y1 = 0; y1 < textureCellSize; y1++)
            {
                for (int x1 = 0; x1 < textureCellSize; x1++)
                {
                    int cellPixelIndex = y1 * textureCellSize + x1;
                    Color color;

                    if (textureIndex2 < 0)
                    {
                        // 森林纹理
                        //color = forestPixels[cellPixelIndex];
                        color = selectedForestPixels[cellPixelIndex];
                    }
                    else
                    {
                        // 沼泽纹理
                        if (marshPixels != null)
                        {
                            color = marshPixels[cellPixelIndex];

                            if (color.a == 0)
                            {
                                color = selectedForestPixels[cellPixelIndex];
                            }
                        }
                        else
                        {
                            color = selectedForestPixels[cellPixelIndex];

                        }
                    }

                    cellPixels[cellPixelIndex] = color;
                }
            }

            // 批量设置像素到目标纹理
            targetTexture.SetPixels(pixelOffsetX, pixelOffsetY, textureCellSize, textureCellSize, cellPixels);
        }
        public static void GenerateMapChunkData(this MapGeneratorComponent self, MapChunkControllerComponent mapChunkController, Vector2Int chunkIndex, Vector3 regionPos)
        {
            mapChunkController.ChunkIndex = chunkIndex;
            mapChunkController.CentrePosition = regionPos + new Vector3((self.mapChunkSize * self.cellSize) / 2, 0, (self.mapChunkSize * self.cellSize) / 2);
            mapChunkController.ForestVertexList = new List<MapVertex>(self.mapChunkSize * self.mapChunkSize);
            mapChunkController.MarhshVertexList = new List<MapVertex>(self.mapChunkSize * self.mapChunkSize);
            mapChunkController.MapVertex1 = self.MapVertex1;
            mapChunkController.MapVertex2 = self.MapVertex2;

            int offsetX = chunkIndex.x * self.mapChunkSize;
            int offsetY = chunkIndex.y * self.mapChunkSize;

            // 预缓存配置数据，避免重复查询
            Dictionary<int, MapConfig> configCache = new Dictionary<int, MapConfig>();
            Dictionary<int, MapAIConfig> aiConfigCache = new Dictionary<int, MapAIConfig>();

            // 批量处理顶点
            for (int x = 1; x < self.mapChunkSize; x++)
            {
                for (int y = 1; y < self.mapChunkSize; y++)
                {
                    MapVertex mapVertex = self.mapGrid.GetVertex(x + offsetX, y + offsetY);

                    // 添加空值检查
                    if (mapVertex == null)
                    {
                        //Debug.LogWarning($"GetVertex返回null: x={x + offsetX}, y={y + offsetY}");
                        continue; // 跳过这个顶点
                    }

                    if (mapVertex.VertexType == self.MapVertex1)
                    {
                        mapChunkController.ForestVertexList.Add(mapVertex);
                    }
                    else if (mapVertex.VertexType == self.MapVertex2)
                    {
                        mapChunkController.MarhshVertexList.Add(mapVertex);
                    }

                    // 根据权重获取地图对象配置ID
                    int configID = self.GetMapObjectConfigIDForWeight(mapVertex.VertexType);

                    // 使用缓存的配置数据
                    MapConfig objectConfig = null;
                    if (configID != -1)
                    {
                        if (!configCache.TryGetValue(configID, out objectConfig))
                        {
                            objectConfig = MapConfigCategory.Instance.Get(configID);
                            configCache[configID] = objectConfig;
                        }
                    }

                    if (objectConfig != null && objectConfig.IsEmpty == 0)
                    {
                        //位置
                        Vector3 position = mapVertex.Position + new Vector3(
                            Random.Range(-self.MapManage.cellSize / 2, self.MapManage.cellSize / 2),
                            0,
                            Random.Range(-self.MapManage.cellSize / 2, self.MapManage.cellSize / 2));
                        //下标
                        mapVertex.MapObjectID = self.CurrentID;

                        MapRegionStatic mapRegionStatic = mapChunkController.AddChild<MapRegionStatic>();
                        mapRegionStatic.MapVertexType = mapVertex.VertexType;
                        mapRegionStatic.ConfigId = configID;
                        mapRegionStatic.Position = position;
                        mapChunkController.MapRegionStaticList.Add(mapRegionStatic);
                        self.CurrentID += 1;
                    }
                }
            }
            //这个地图快里的森林点位的数量
            //Debug.Log("森林点：" + mapChunkController.ForestVertexList.Count + "沼泽点：" + mapChunkController.MarhshVertexList.Count);

            // 批量生成AI数据
            GenerateAIDataBatch(self, mapChunkController, aiConfigCache);
        }

        /// <summary>
        /// 批量生成AI数据，减少重复的配置查询
        /// </summary>
        private static void GenerateAIDataBatch(MapGeneratorComponent self, MapChunkControllerComponent mapChunkData, Dictionary<int, MapAIConfig> aiConfigCache)
        {
            // 生成森林AI数据
            if (mapChunkData.ForestVertexList.Count > self.MapManage.GenerateAIMinVertexCountOnChunk)
            {
                for (int i = 0; i < self.MapManage.MaxAIOnChunk; i++)
                {
                    int configID = self.GetAIMapObjectConfigIDForWeight(self.MapVertex1);
                    // 使用缓存的AI配置数据
                    MapAIConfig config = null;
                    if (configID != -1)
                    {
                        if (!aiConfigCache.TryGetValue(configID, out config))
                        {
                            config = MapAIConfigCategory.Instance.Get(configID);
                            aiConfigCache[configID] = config;
                        }
                    }

                    if (config != null && config.IsEmpty == 0)
                    {
                        MapRegionUnit mapRegionUnit = mapChunkData.AddChild<MapRegionUnit>();
                        mapRegionUnit.MapVertexType = self.MapVertex1;
                        mapRegionUnit.ConfigId = configID;
                        mapRegionUnit.UnitId = IdGenerater.Instance.GenerateId();
                        mapRegionUnit.Position = mapChunkData.GetAIRandomPoint(mapRegionUnit.MapVertexType);
                        mapRegionUnit.NextUpdateTime = 0;
                        mapChunkData.MapRegionUnitList.Add(mapRegionUnit);
                        self.CurrentID += 1;
                    }
                }
            }

            // 生成沼泽AI数据
            if (mapChunkData.MarhshVertexList.Count > self.MapManage.GenerateAIMinVertexCountOnChunk)
            {
                for (int i = 0; i < self.MapManage.MaxAIOnChunk; i++)
                {
                    int configID = self.GetAIMapObjectConfigIDForWeight(self.MapVertex2);

                    // 使用缓存的AI配置数据
                    MapAIConfig config = null;
                    if (configID != -1)
                    {
                        if (!aiConfigCache.TryGetValue(configID, out config))
                        {
                            config = MapAIConfigCategory.Instance.Get(configID);
                            aiConfigCache[configID] = config;
                        }
                    }

                    if (config != null && config.IsEmpty == 0)
                    {
                        MapRegionUnit mapRegionUnit = mapChunkData.AddChild<MapRegionUnit>();
                        mapRegionUnit.MapVertexType = self.MapVertex2;
                        mapRegionUnit.ConfigId = configID;
                        mapRegionUnit.UnitId = IdGenerater.Instance.GenerateId();
                        mapRegionUnit.Position = mapChunkData.GetAIRandomPoint(mapRegionUnit.MapVertexType);
                        mapRegionUnit.NextUpdateTime = 0;
                        mapChunkData.MapRegionUnitList.Add(mapRegionUnit);
                        self.CurrentID += 1;
                    }
                }
            }
        }

        //根据权重 获取地图元素
        private static int GetMapObjectConfigIDForWeight(this MapGeneratorComponent self, MapVertexType type)
        {
            List<MapConfig> configModels = MapConfigCategory.Instance.GetListByMapVertexType((int)type);
            if (configModels == null || configModels.Count == 0)
            {
                return -1;
            }

            // 确定权重的总和
            int weightTotal = type == self.MapVertex1 ? self.forestSpawanWeightTotal : self.marshSpawanWeightTotal;

            int randValue = Random.Range(1, weightTotal + 1); // 实际命中数字是从1~weightTotal
            float temp = 0;
            int spawnConfigIndex = 0; // 最终要生成的物品

            for (int i = 0; i < configModels.Count; i++)
            {
                temp += configModels[i].Probability;
                if (randValue < temp)
                {
                    spawnConfigIndex = i;
                    break;
                }
            }

            // Debug.Log("生成的id："+configModels[spawnConfigIndex].Id);
            return configModels[spawnConfigIndex].Id;
        }
        //根据权重 获取ai元素
        private static int GetAIMapObjectConfigIDForWeight(this MapGeneratorComponent self, MapVertexType type)
        {
            List<MapAIConfig> configModels = MapAIConfigCategory.Instance.GetListByMapVertexType((int)type);
            if (configModels == null || configModels.Count == 0)
            {
                return -1;
            }

            // 确定权重的总和
            int weightTotal = type == self.MapVertex1 ? self.AIforestSpawanWeightTotal : self.AImarshSpawanWeightTotal;

            int randValue = Random.Range(1, weightTotal + 1); // 实际命中数字是从1~weightTotal
            float temp = 0;
            int spawnConfigIndex = 0; // 最终要生成的物品

            for (int i = 0; i < configModels.Count; i++)
            {
                temp += configModels[i].Probability;
                if (randValue < temp)
                {
                    spawnConfigIndex = i;
                    break;
                }
            }

            //Debug.Log("权重总和： " + weightTotal + "ID： " + configModels[spawnConfigIndex].Id);
            // Debug.Log("生成的id："+configModels[spawnConfigIndex].Id);
            return configModels[spawnConfigIndex].Id;
        }
    }
}
