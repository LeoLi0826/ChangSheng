// using System.Collections;
// using System.Collections.Generic;
// using ET.Client;
// using Sirenix.Utilities.Editor;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace ET
// {
//     // <summary>
//     /// 地图块数据
//     /// </summary>
//     public class MapChunkData
//     {
//         public List<MapChunkMapObjectModel> MapObjectList = new List<MapChunkMapObjectModel>();
//     }
//
//     public class MapChunkMapObjectModel
//     {
//         //public GameObject Prefab;
//         public Vector3 Position;
//         public int ConfigID;
//     }
//
//     public class MapChunkController : MonoBehaviour
//     {
//         public Vector2Int ChunkIndex { get; private set; }
//         public Vector3 CentrePosition { get; private set; }
//
//         private MapChunkData mapChunkData;
//         private List<GameObject> mapObjectList;
//         private bool isActive = false;
//
//         public void Init(Vector2Int chunkIndex, Vector3 centrePosition, List<MapChunkMapObjectModel> MapObjectList)
//         {
//             Debug.Log("MapChunkController初始化1");
//             ChunkIndex = chunkIndex;
//             CentrePosition = centrePosition;
//             mapChunkData = new MapChunkData();
//             mapChunkData.MapObjectList = MapObjectList;
//             mapObjectList = new List<GameObject>(MapObjectList.Count);
//             Debug.Log("MapChunkController初始化2 count: "+mapChunkData.MapObjectList.Count);
//
//             //GameObjectPoolHelper.InitPoolWithPathAsync("Item_test", "Item_test", 30).Coroutine();
//             Debug.Log("MapChunkController初始化3");
//
//         }
//
//         public void SetActive(bool active)
//         {
//             if (isActive != active)
//             {
//                 isActive = active;
//                 gameObject.SetActive(isActive);
//
//                 List<MapChunkMapObjectModel> ObjectList = mapChunkData.MapObjectList;
//                 Debug.Log("对象池1 ObjectList Count:  "+mapChunkData.MapObjectList.Count); 
//                 // 从对象池获取所有物体
//                 if (isActive)
//                 {
//                     for (int i = 0; i < ObjectList.Count; i++)
//                     {
//                         //有父物体吗？
//                         if(ObjectList[i] == null)
//                         {
//                             Debug.Log("对象池1 列表没有该物体");   
//                         }
//                         else
//                         {
//                             Debug.Log("对象池1 列表有：name： "+ObjectList[i].ConfigID+ " 位置： "+ObjectList[i].Position);
//                         }
//                         UnitInfo unitInfo = UnitInfo.Create();
//                         unitInfo.ConfigId = spawnModel.Id;
//                         CanvasScaler.Unit unit = UnitFactory.CreateMapRes(self.Root().CurrentScene(), unitInfo );
//
//                         
//                         GameObject go = GameObjectPoolHelper.GetObjectFromPool(ObjectList[i].Prefab.name);
//                         if (go == null)
//                         {
//                             Debug.Log("对象池2 列表没有该物体");   
//                         }
//                         else
//                         {
//                             Debug.Log("对象池2 列表有：name： "+ObjectList[i].Prefab.name+ " 位置： "+ObjectList[i].Position);
//                         }
//                         //GameObject go = PoolManager.Instance.GetGameObject(ObjectList[i].Prefab, transform);
//                         
//                         go.transform.position = ObjectList[i].Position;
//                         
//                         go.transform.rotation = Camera.main.transform.rotation;
//                         mapObjectList.Add(go);
//                     }
//                 }
//                 // 把所有物体放回对象池
//                 else
//                 {
//                     for (int i = 0; i < ObjectList.Count; i++)
//                     {
//                         Debug.Log("对象池 放回物体 name： "+ObjectList[i].Prefab.name + " 位置： "+ObjectList[i].Position);   
//                         GameObjectPoolHelper.ReturnObjectToPool(mapObjectList[i]);
//                         //PoolManager.Instance.PushGameObject(mapObjectList[i]);
//                     }
//                 
//                     mapObjectList.Clear();
//                 }
//             }
//         }
//     }
// }
