using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace ET.Client
{
    [EntitySystemOf(typeof(MapChunkControllerComponent))]
    [FriendOf(typeof(MapChunkControllerComponent))]
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionUnit))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionStatic))]
    public static partial class MapChunkControllerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MapChunkControllerComponent self)
        {

        }
        [EntitySystem]
        private static void Update(this ET.Client.MapChunkControllerComponent self)
        {

        }
        [EntitySystem]
        private static void LateUpdate(this ET.Client.MapChunkControllerComponent self)
        {

        }

        public static void SetActive(this ET.Client.MapChunkControllerComponent self, bool active)
        {
            if (self.isActive != active)
            {
                self.isActive = active;
                if (self.isActive)
                {
                    //Debug.Log("我显示了");


                    foreach (var entityRef in self.MapRegionStaticList)
                    {
                        MapRegionStatic mapRegionStatic = entityRef;
                        self.SetActiveMapObject(mapRegionStatic, true).NoContext();
                    }

                    foreach (var entityRef in self.MapRegionUnitList)
                    {
                        MapRegionUnit mapRegionUnit = entityRef;
                        if (mapRegionUnit.NextUpdateTime <= 0)
                        {
                            UnitFactory.CreateMapObject(self.Scene(), mapRegionUnit);
                        }
                    }
                }
                else
                {
                    //Debug.Log("我关闭显示了");
                    foreach (var entityRef in self.MapRegionStaticList)
                    {
                        MapRegionStatic mapRegionStatic = entityRef;
                        self.SetActiveMapObject(mapRegionStatic, false).NoContext();
                    }
                    
                    UnitComponent unitComponent = self.Scene().GetComponent<UnitComponent>();
                    foreach (var entityRef in self.MapRegionUnitList)
                    {
                        MapRegionUnit mapRegionUnit = entityRef;
                        Unit unit = unitComponent.GetChild<Unit>(mapRegionUnit.UnitId);
                        mapRegionUnit.Position = unit.Position;
                        unit.Dispose();
                    }
                }
            }
        }

        #region 生成地图元素

        public static async ETTask SetActiveMapObject(this ET.Client.MapChunkControllerComponent self, MapRegionStatic mapRegionStatic, bool active)
        {
            if (active)
            {
                MapConfig mapConfig = MapConfigCategory.Instance.Get(mapRegionStatic.ConfigId);
                if (mapConfig.UnitID == 0)
                {
                    return;
                }
                UnitConfig unitConfig = UnitConfigCategory.Instance.Get(mapConfig.UnitID);
                GameObject gameObject = await YIUIGameObjectPool.Inst.Get(unitConfig.PrefabName);
                gameObject.transform.position = mapRegionStatic.Position;
                gameObject.transform.rotation = Camera.main.transform.rotation;
                
                // 查找物体下的img子对象
                Transform imgTransform = gameObject.transform.Find("img");
                if (imgTransform != null)
                {
                    // 50%概率绕Y轴旋转180度
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        imgTransform.Rotate(0, 180, 0);
                    }
                }
                
                mapRegionStatic.GameObject = gameObject;
            }
            else
            {
                if (mapRegionStatic.GameObject != null)
                {
                    YIUIGameObjectPool.Inst.Put(mapRegionStatic.GameObject);
                    mapRegionStatic.GameObject = null;
                }
            }
        }
        #endregion


        public static Vector3 GetAIRandomPoint(this ET.Client.MapChunkControllerComponent self, MapVertexType mapVertexType)
        {
            self.currentRecursionDepth++;
            //Debug.Log($"AI生成物1 递归深度: {self.currentRecursionDepth}, 地形类型: {mapVertexType}");

            if (self.currentRecursionDepth > self.maxRecursionDepth)
            {
                self.currentRecursionDepth = 0;
                //Debug.LogError($"AI生成物2 递归次数过多，返回默认位置");
                return Vector3.zero;
            }

            List<MapVertex> verticeList = null;

            //桶通过地图类型 获取获取类型顶点
            if (mapVertexType == self.MapVertex1)
            {
                //Debug.Log("AI生成物3： position2: " );
                verticeList = self.ForestVertexList;
                //    Debug.Log($"AI生成物4: {verticeList?.Count ?? 0}");
            }
            else if (mapVertexType == self.MapVertex2)
            {
                //Debug.Log("AI生成物5： position3: " );
                verticeList = self.MarhshVertexList;
                //Debug.Log($"AI生成物6 沼泽顶点数量: {verticeList?.Count ?? 0}");
            }

            if (verticeList == null || verticeList.Count == 0)
            {
                //Debug.LogWarning($"AI生成物7 没有可用的{mapVertexType}顶点列表用于AI生成");
                self.currentRecursionDepth = 0;
                return Vector3.zero;
            }

            int index = Random.Range(0, verticeList.Count);
            Vector3 selectedPosition = verticeList[index].Position;
            //Debug.Log($"AI生成物8 选中的顶点位置: {selectedPosition}, 索引: {index}");

            // if (NavMesh == null)
            // {
            //     
            // }
            // 确定AI怪物的生成位置
            if (NavMesh.SamplePosition(selectedPosition, out NavMeshHit hitInfo, 1, NavMesh.AllAreas))
            {
                //Debug.Log($"AI生成物9 找到有效NavMesh位置: {hitInfo.position}");
                self.currentRecursionDepth = 0;
                return hitInfo.position;
            }
            else
            {
                //Debug.LogWarning($"AI生成物10 无法在NavMesh上找到有效位置，尝试在更大范围内寻找");
                // 尝试在更大的范围内寻找位置
                if (NavMesh.SamplePosition(selectedPosition, out hitInfo, 5, NavMesh.AllAreas))
                {
                    //Debug.Log($"AI生成物11 在更大范围内找到有效NavMesh位置: {hitInfo.position}");
                    self.currentRecursionDepth = 0;
                    return hitInfo.position;
                }
                else
                {
                    //Debug.LogWarning($"AI生成物12 即使在更大范围内也无法找到有效NavMesh位置，返回顶点位置");
                    // 如果还是找不到，返回顶点位置
                    self.currentRecursionDepth = 0;
                    return selectedPosition;
                }
            }
        }
    }
}
