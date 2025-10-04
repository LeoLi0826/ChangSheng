using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// 网格，主要包含顶点和格子
    /// </summary>
    public class MapGrid
    {
        // 顶点数据
        public Dictionary<Vector2Int, MapVertex> vertexDic = new Dictionary<Vector2Int, MapVertex>();

        // 格子数据
        public Dictionary<Vector2Int, MapCell> cellDic = new Dictionary<Vector2Int, MapCell>();

        public MapGrid(int mapHeight, int mapWidth, float cellSize)
        {
            MapHeight = mapHeight;
            MapWidth = mapWidth;
            CellSize = cellSize;

            // 生成顶点数据
            for (int x = 1; x < mapWidth; x++)
            {
                for (int z = 1; z < mapHeight; z++)
                {
                    AddVertex(x, z);
                    AddCell(x, z);
                }
            }

            // 增加一行一列
            for (int x = 1; x <= mapWidth; x++)
            {
                AddCell(x, mapHeight);
            }

            for (int z = 1; z < mapWidth; z++)
            {
                AddCell(mapWidth, z);
            }


            #region 测试代码

            //foreach (var item in vertexDic.Values)
            //{
            //    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    temp.transform.position = item.Position;
            //    temp.transform.localScale = Vector3.one * 0.25f;
            //}
            //foreach (var item in cellDic.Values)
            //{
            //    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    temp.transform.position = item.Position-new Vector3(0,0.49f,0);
            //    temp.transform.localScale = new Vector3(CellSize, 1,CellSize);
            //}

            #endregion
        }

        public int MapHeight { get; private set; }
        public int MapWidth { get; private set; }
        public float CellSize { get; private set; }


        #region 顶点

        private void AddVertex(int x, int y)
        {
            vertexDic.Add(new Vector2Int(x, y)
                , new MapVertex()
                {
                    Position = new Vector3(x * CellSize, 0, y * CellSize)
                });
        }

        /// <summary>
        /// 获取顶点，如果找不到返回Null
        /// </summary>
        public MapVertex GetVertex(Vector2Int index)
        {
            MapVertex vertex = null;
            vertexDic.TryGetValue(index, out vertex);
            return vertex;
        }

        public MapVertex GetVertex(int x, int y)
        {
            return GetVertex(new Vector2Int(x, y));
        }

        /// <summary>
        /// 通过世界坐标获取顶点
        /// </summary>
        public MapVertex GetVertexByWorldPosition(Vector3 position)
        {
            int x = Mathf.Clamp(Mathf.RoundToInt(position.x / CellSize), 1, MapWidth);
            int y = Mathf.Clamp(Mathf.RoundToInt(position.z / CellSize), 1, MapHeight);
            return GetVertex(x, y);
        }


        /// <summary>
        /// 设置顶点类型
        /// </summary>
        private void SetVertexType(Vector2Int vertexIndex, int MapNum,MapVertexType mapVertexType)
        {
            MapVertex vertex = GetVertex(vertexIndex);
            if (vertex.VertexType != mapVertexType)
            {
                vertex.VertexType = mapVertexType;
                // 只有沼泽需要计算
                if (vertex.VertexType == (MapVertexType)(MapNum*2))
                {
                    // 计算附近的贴图权重

                    MapCell tempCell = GetLeftBottomMapCell(vertexIndex);
                    if (tempCell != null) tempCell.TextureIndex += 1;

                    tempCell = GetRightBottomMapCell(vertexIndex);
                    if (tempCell != null) tempCell.TextureIndex += 2;

                    tempCell = GetLeftTopMapCell(vertexIndex);
                    if (tempCell != null) tempCell.TextureIndex += 4;

                    tempCell = GetRightTopMapCell(vertexIndex);
                    if (tempCell != null) tempCell.TextureIndex += 8;
                }
            }
        }

        /// <summary>
        /// 设置顶点类型
        /// </summary>
        private void SetVertexType(int x, int y,int MapNum, MapVertexType mapVertexType)
        {
            SetVertexType(new Vector2Int(x, y),MapNum, mapVertexType);
        }

        #endregion

        #region 格子

        private void AddCell(int x, int y)
        {
            float offset = CellSize / 2;
            cellDic.Add(new Vector2Int(x, y),
                new MapCell()
                {
                    Position = new Vector3(x * CellSize - offset, 0, y * CellSize - offset)
                });
        }

        /// <summary>
        /// 获取格子，如果没有找到会返回Null
        /// </summary>
        public MapCell GetCell(Vector2Int index)
        {
            MapCell cell = null;
            cellDic.TryGetValue(index, out cell);
            return cell;
        }

        public MapCell GetCell(int x, int y)
        {
            return GetCell(new Vector2Int(x, y));
        }

        /// <summary>
        /// 获取左下角格子
        /// </summary>
        public MapCell GetLeftBottomMapCell(Vector2Int vertexIndex)
        {
            return GetCell(vertexIndex);
        }

        /// <summary>
        /// 获取右下角格子
        /// </summary>
        public MapCell GetRightBottomMapCell(Vector2Int vertexIndex)
        {
            return GetCell(vertexIndex.x + 1, vertexIndex.y);
        }

        /// <summary>
        /// 获取左上角格子
        /// </summary>
        public MapCell GetLeftTopMapCell(Vector2Int vertexIndex)
        {
            return GetCell(vertexIndex.x, vertexIndex.y + 1);
        }

        /// <summary>
        /// 获取右上角格子
        /// </summary>
        public MapCell GetRightTopMapCell(Vector2Int vertexIndex)
        {
            return GetCell(vertexIndex.x + 1, vertexIndex.y + 1);
        }

        #endregion

        /// <summary>
        /// 计算格子贴图的索引数字（支持自定义两种地块类型）
        /// </summary>
        public void CalculateMapVertexType(float[,] noiseMap, float limit, int MapNum,MapVertexType type1, MapVertexType type2)
        {
            int Width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1); 

            for (int x = 1; x < Width; x++)
            {
                for (int z = 1; z < height; z++)
                {
                    // 基于噪声中的值确定这个顶点的类型
                    // 大于边界是沼泽，否则是森林
                    if (noiseMap[x, z] >= limit)
                    {
                        SetVertexType(x, z,MapNum, type2);
                    }
                    else
                    {
                        SetVertexType(x, z,MapNum, type1);
                    }
                }
            }
        }
        /// <summary>
        /// 计算格子贴图的索引数字（兼容旧接口，默认森林/沼泽）
        /// </summary>
        // public void CalculateMapVertexType(float[,] noiseMap, float limit)
        // {
        //     CalculateMapVertexType(noiseMap, limit, MapVertexType.Forest, MapVertexType.Marsh);
        // }
    }

    /// <summary>
    /// 顶点类型
    /// </summary>
    public enum MapVertexType
    {
        empty,          //空
        Forest,         //森林
        Marsh,          //沼泽
        HuoYanShan1,    //火焰山1
        HuoYanShan2,    //火焰山2
        HuangFengLing1, //黄风岭1
        HuangFengLing2, //黄风岭2
        
    }

    /// <summary>
    /// 地图顶点
    /// </summary>
    public class MapVertex
    {
        public Vector3 Position;
        public MapVertexType VertexType;
        public long MapObjectID;
    }

    /// <summary>
    /// 地图格子
    /// </summary>
    public class MapCell
    {
        public Vector3 Position;
        public int TextureIndex;
    }

}