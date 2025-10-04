using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


namespace ET.Client
{
    [Event(SceneType.Current)]
    [FriendOfAttribute(typeof(ET.Client.GameObjectComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapManageComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapChunkControllerComponent))]
    public class MapChunkRefresh : AEvent<Scene, RefreshMapChunk>
    {
        protected override async ETTask Run(Scene scene, RefreshMapChunk args)
        {
            MapChunkControllerComponent chunk = args.unit;
            if (chunk != null)
            {
                MapManageComponent mapManageComponent = chunk.GetParent<MapManageComponent>();
                
                //    Debug.Log("刷新地图块23：：地图块为x: " + chunkIndex.x + " 地图块为y: " + chunkIndex.y);
                // if (unit.GetComponent<MapChunkControllerComponent>() == null)
                // {
                //     Debug.Log("刷新地图块23：：没有找到 MapChunkControllerComponent");
                // }
                // else
                // {
                //     Debug.Log("刷新地图块23：：找到 MapChunkControllerComponent");
                // }

                // if (chunk == null)
                // {
                //     Debug.Log("刷新地图块23：：没有找到 chunk");
                // }
                // else
                // {
                //     Debug.Log("刷新地图块23：：找到 chunk x: " + chunk.ChunkIndex.x + " y: " + chunk.ChunkIndex.y);
                // }

                chunk.SetActive(true);

                mapManageComponent.lastVisibleChunkList.Add(chunk);
            }

            await ETTask.CompletedTask;
        }
    }
}
