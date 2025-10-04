using ET.Client;
using UnityEngine;

namespace ET
{
    [Invoke(TimerInvokeType.MapUpdate)]
    [FriendOfAttribute(typeof(ET.Client.MapManageComponent))]
    public class MapUpdateTimerHandler : ATimer<MapManageComponent>
    {
        protected override void Run(MapManageComponent self)
        {
            // 添加安全检查，防止访问已销毁的Entity
            if (self == null || self.IsDisposed)
            {
                Log.Debug("MapUpdateTimerHandler: self已销毁，跳过执行");
                return;
            }
            
            try
            {
                var root = self.Root();
                if (root == null || root.IsDisposed)
                {
                    Log.Debug("MapUpdateTimerHandler: Root已销毁，跳过执行");
                    return;
                }
                
                var currentScene = root.CurrentScene();
                if (currentScene == null || currentScene.IsDisposed)
                {
                    Log.Debug("MapUpdateTimerHandler: CurrentScene已销毁，跳过执行");
                    return;
                }
                
                var mapComponent = currentScene.GetComponent<MapManageComponent>();
                if (mapComponent != null && !mapComponent.IsDisposed)
                {
                    //Debug.Log("执行地图块更新功能");
                    mapComponent.UpdateVisibleChunk().NoContext();
                }
                else
                {
                    Log.Debug("MapUpdateTimerHandler: MapManageComponent不存在或已销毁");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"MapUpdateTimerHandler执行失败：{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}