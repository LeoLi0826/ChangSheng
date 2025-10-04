using Unity.Cinemachine;
using UnityEngine;

namespace ET.Client
{
    //只是简单给view赋值
    [Event(SceneType.Current)]
    [FriendOfAttribute(typeof(ET.Client.GameObjectComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapManageComponent))]
    public class AfterUnitCreate_GetView : AEvent<Scene, AfterUnitCreateGetView>
    {
        protected override async ETTask Run(Scene scene, AfterUnitCreateGetView args)
        {
            Unit unit = args.Unit;
            // Unit View层
            
            GameObject go = unit.GetComponent<GameObjectComponent>().GameObject ;
            if (go == null)
            {
                Log.Debug("我没有找到：");

            }
            else
            {
                Log.Debug("我赋值给view！name："+go.transform.name);

            }
            

            
            
            if (scene.GetComponent<MapManageComponent>() == null)
            {
                //scene.AddComponent<MapManageComponent>();
                if (scene.GetComponent<MapManageComponent>() == null)
                {
                    Log.Debug("我没找到：mamanage组件");
                }else
                    Log.Debug("我找到2：mamanage组件");

            }
            else
            {
                Log.Debug("我找到：mamanage组件");
                scene.GetComponent<MapManageComponent>().MapManage.viewer = go.transform;

            }
         
            await ETTask.CompletedTask;
        }

    }
}