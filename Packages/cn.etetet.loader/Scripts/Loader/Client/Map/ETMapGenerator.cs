// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Sirenix.OdinInspector;
// using Random = UnityEngine.Random;
//
// namespace ET
// {
//     public class ETMapGenerator : MonoBehaviour
//     {
//
//         public MeshRenderer meshRenderer;
//         public MeshFilter meshFilter;
//
//         public int mapHeight;
//         public int mapWidth;
//         public float cellSize;
//
//         public float lacunarity;
//         public int mapSeed;
//         public int spawnSeed;
//
//         [Range(0f, 1f)]
//         public float limit;
//
//         MapGrid grid;
//         
//         public Texture2D groundTexture;
//         public Texture2D[] marshTextures;
//
//
//         public List<GameObject> mapObjects = new List<GameObject>();
//
//         
//         public void Awake()
//         {
//
//         }
//
//         /// <summary>
//         /// 生成地图
//         /// </summary>
//         [Button("生成地图")]
//         public void GenerateMap()
//         {
//             //生成Mesh
//             meshFilter.mesh = GenerateMapMesh(mapHeight, mapWidth, cellSize);
//             //生成网格
//             grid = new MapGrid(mapHeight, mapWidth, cellSize);
//             //生成噪声图
//             float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, lacunarity, mapSeed);
//             // 确认顶点的类型，以及计算顶点周围网格的贴图
//             int[,] cellTextureIndexMap = grid.CalculateCellTextureIndex(noiseMap, limit);
//
//             //基于网格的贴图索引数字 生成地图贴图
//             Texture2D mapTexture = GenerateMapTexture(cellTextureIndexMap, groundTexture, marshTextures);
//             meshRenderer.sharedMaterial.mainTexture = mapTexture;
//
//             //生成地图物体
//             //SpawnMapObject(grid,spawnSeed);
//         }
//
//         public GameObject testObj;
//
//         #region 测试代码
//
//
//
//         [Button("测试顶点")]
//         public void TestVertex()
//         {
//             print(grid.GetVertexByWorldPosition(testObj.transform.position).Position);
//         }
//
//         [Button("测试格子")]
//         public void TestCell(Vector2Int index)
//         {
//             print(grid.GetRightTopMapCell(index).Position);
//
//         }
//
//         #endregion
//
//
//         /// <summary>
//         // /// 生成地形Mash
//         // /// </summary>
//         private Mesh GenerateMapMesh(int height, int width, float cellSize)
//         {
//             Mesh mesh = new Mesh();
//             // 确定顶点在哪里
//             mesh.vertices = new Vector3[]
//             {
//                 new Vector3(0, 0, 0),
//                 new Vector3(0, 0, height * cellSize),
//                 new Vector3(width * cellSize, 0, height * cellSize),
//                 new Vector3(width * cellSize, 0, 0),
//             };
//
//             // 确定哪些点形成三角形
//             mesh.triangles = new int[]
//             {
//                 0, 1, 2,
//                 0, 2, 3
//             };
//
//             mesh.uv = new Vector2[]
//             {
//                 new Vector3(0, 0),
//                 new Vector3(0, 1),
//                 new Vector3(1, 1),
//                 new Vector3(1, 0),
//             };
//
//             //计算法线
//             mesh.RecalculateNormals();
//
//             return mesh;
//         }
//
//         // <summary>
//         // 生成噪声图
//         // </summary>
//         private float[,] GenerateNoiseMap(int width, int height, float lacunarity, int mapSeed)
//         {
//             // 应用种子
//             Random.InitState(mapSeed);
//             lacunarity += 0.1f;
//             // 这里的噪声图是为了顶点服务的
//             float[,] noiseMap = new float[width - 1, height - 1];
//             float offsetX = Random.Range(-10000f, 10000f);
//             float offsetY = Random.Range(-10000f, 10000f);
//             for (int x = 0; x < width - 1; x++)
//             {
//                 for (int y = 0; y < height - 1; y++)
//                 {
//                     noiseMap[x, y] = Mathf.PerlinNoise(x * lacunarity + offsetX, y * lacunarity + offsetY);
//                 }
//             }
//
//             return noiseMap;
//         }
//
//         private Texture2D GenerateMapTexture(int[,] cellTextureIndexMap, Texture2D groundTexture, Texture2D[] marshTextures)
//         {
//             // 地图宽高 以格子为单位
//             int mapWidth = cellTextureIndexMap.GetLength(0);
//             int mapHeight = cellTextureIndexMap.GetLength(1);
//             // 贴图都是矩形
//             int textureCellSize = groundTexture.width;
//             Texture2D mapTexture = new Texture2D(mapWidth * textureCellSize, mapHeight * textureCellSize, TextureFormat.RGB24, false);
//             // 遍历每一个格子
//             for (int y = 0; y < mapHeight; y++)
//             {
//                 int offsetY = y * textureCellSize;
//                 for (int x = 0; x < mapWidth; x++)
//                 {
//                     int offsetX = x * textureCellSize;
//                     int textureIndex = cellTextureIndexMap[x, y] - 1;
//
//
//                     // 绘制每个格子内的像素
//                     //访问每一个像素点
//                     for (int y1 = 0; y1 < textureCellSize; y1++)
//                     {
//                         for (int x1 = 0; x1 < textureCellSize; x1++)
//                         {
//                             //设置某个像素点点颜色
//                             //确定是僧了还是沼泽
//                             //这个地方是森林
//                             //这个地方是沼泽但是是透明的 这种情况是需要绘制groundTexture同位置的像素颜色
//                             if (textureIndex < 0)
//                             {
//                                 Color color = groundTexture.GetPixel(x1, y1);
//                                 mapTexture.SetPixel(x1 + offsetX, y1 + offsetY, color);
//                             }
//                             //是沼泽贴图的颜色
//                             else
//                             {
//                                 //||marshTextures[textureIndex].GetPixel(x1,y1).a ==0
//                                 Color color = marshTextures[textureIndex].GetPixel(x1, y1);
//                                 if (color.a == 0)
//                                 {
//                                     mapTexture.SetPixel(x1 + offsetX, y1 + offsetY, groundTexture.GetPixel(x1, y1));
//                                 }
//                                 else
//                                 {
//                                     mapTexture.SetPixel(x1 + offsetX, y1 + offsetY, color);
//
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//
//             mapTexture.filterMode = FilterMode.Point;
//             mapTexture.wrapMode = TextureWrapMode.Clamp;
//
//             mapTexture.Apply();
//             return mapTexture;
//         }
//
//
//         /// <summary>
//         /// 生成各种地图对象
//         /// </summary>
//         //     private async void SpawnMapObject(MapGrid mapGrid, int spawnSeed)
//         //     {
//         //         //清除之前的物体
//         //         for(int i=0;i<mapObjects.Count;i++)
//         //         {
//         //             DestroyImmediate(mapObjects[i].gameObject);
//         //         }
//         //         
//         //         
//         //         // 使用种子来进行随机生成
//         //         Random.InitState(spawnSeed);
//         //         int mapHeight = mapGrid.MapHeight;
//         //         int mapWidth = mapGrid.MapWidth;
//         //     
//         //         // 便利地图顶点
//         //         for (int x = 1; x < mapWidth; x++)
//         //         {
//         //             for (int y = 1; y < mapHeight; y++)
//         //             {
//         //                 MapVertex mapVertex = mapGrid.GetVertex(x, y);
//         //                 
//         //                 
//         //                 // 根据概率配置随机  注意 在这里读取物品配置的时候要搜索物品的地址 可能会造成性能上的问题
//         //                 List<MapConfig> configModels = FindMapConfig(mapVertex.VertexType);
//         //                 //joker老师的例子
//         //                 //List<MapObjectSpwanConfigModel> configModels = spawnConfig.SpawnConfigDic[mapVertex.VertexType];
//         //     
//         //                 // 我们确保整个配置概率值合为100
//         //                 int randValue = Random.Range(1, 101); // 实际命中数字是从1~100 
//         //                 float temp = 0;
//         //                 int spawnConfigIndex = 0; // 最终要生成的物品
//         //     
//         //                 // 30 20 50
//         //                 for (int i = 0; i < configModels.Count; i++)
//         //                 {
//         //                     temp += configModels[i].Probability;
//         //                     if (randValue < temp)
//         //                     {
//         //                         // 命中
//         //                         spawnConfigIndex = i;
//         //                         break;
//         //                     }
//         //                 }
//         //                 
//         //                 // 确定到底生成什么物品
//         //                 MapConfig spawnModel = configModels[spawnConfigIndex];
//         //                 if (spawnModel.IsEmpty == false)
//         //                 {
//         //                     // 实例化物品
//         //                     Vector3 offset = new Vector3(Random.Range(-cellSize / 2, cellSize / 2), 0, Random.Range(-cellSize / 2, cellSize / 2));
//         //                     
//         //                     // 参考AfterUnitCreate_CreateUnitView
//         //                     // ResourcesLoaderComponent resourcesLoaderComponent = scene.GetComponent<ResourcesLoaderComponent>();
//         //                     //GameObject player = await resourcesLoaderComponent.LoadAssetAsync<GameObject>(unitConfig.PrefabName);
//         //                     
//         //                      GameObject prefab = Resources.Load<GameObject>("Prefabs/" + spawnModel.Prefab);
//         //     
//         //                     GameObject go = GameObject.Instantiate(prefab, mapVertex.Position + offset, Quaternion.identity, transform); 
//         //                     mapObjects.Add(go);
//         //                 }
//         //             }
//         //         }
//         //         await ETTask.CompletedTask;
//         //     }
//         //     
//         //     public static List<MapConfig> FindMapConfig(MapVertexType type)
//         //     {
//         //         List<MapConfig> mapConfigs = new List<MapConfig>();
//         //         
//         //         for(int i =0;i<MapConfigCategory.Instance.GetAll().Count;i++)
//         //         {
//         //             if (MapConfigCategory.Instance.GetAll()[i].MapVertexType == (int)type)
//         //             {
//         //                 mapConfigs.Add(MapConfigCategory.Instance.GetAll()[i]);
//         //             }
//         //         }
//         //     
//         //         return mapConfigs;
//         //     }
//         //     
//         //     
//
//         public class prefabInfo
//         {
//             [LabelText("是否为空")]
//             public bool IsEmpty = false; 
//             [LabelText("预制体")]
//             public GameObject Prefab;
//             [LabelText("生成概率")]
//             public int Probability;
//             // public string prefabName;
//             // public int probability;
//             // public bool isEmpty;
//         }
//     }
//
// }
