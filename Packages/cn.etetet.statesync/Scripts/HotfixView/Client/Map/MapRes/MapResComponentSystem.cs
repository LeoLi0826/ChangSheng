using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using System.Linq;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace ET.Client
{
    [EntitySystemOf(typeof(MapResComponent))]
    [FriendOf(typeof(MapResComponent))]
    public static partial class MapResComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MapResComponent self)
        {
        }

        
        
        
        [EntitySystem]
        private static void Update(this ET.Client.MapResComponent self)
        {

        }
        [EntitySystem]
        private static void LateUpdate(this ET.Client.MapResComponent self)
        {

        }
        
        //对象池初始化
        public static async ETTask Run(this ET.Client.MapResComponent self)
        {
            // 1, //玩家
            // 2, //动物
            // 3, //敌人
            // 4, //环境元素
            // 5, //其他
            var unitConfigs = UnitConfigCategory.Instance.GetAll()
                    .Values.Where(c => c.Id >= 1000 ).ToList();//可根据类型筛选
            
            foreach (var unitConfig in unitConfigs)
            {
                //注意 在网格设置资源的时候 这里可能会出现问题
                if (unitConfig.Name == null)
                {
                    Debug.LogError("unitConfig is null");
                    return;
                
                }
                else
                {
                    Log.Debug("unit 初始化name:"+unitConfig.Name);
                }

                if (unitConfig.PrefabName == "")
                {
                    Debug.Log("预制体为空： 名字为"+unitConfig.Name);
                    continue ;
                }
                else
                {
                    Log.Debug("unit 初始化预制体名字1:"+unitConfig.PrefabName);
                
                }
                // await GameObjectPoolHelper.InitPoolWithPathAsync(unitConfig.PrefabName, 
                //     unitConfig.PrefabName, 1);
            }
            // await GameObjectPoolHelper.InitPoolWithPathAsync("ljf_pre", 
            //     "ljf_pre", 1);
            // //Sandstorm_Particles1
            // await GameObjectPoolHelper.InitPoolWithPathAsync("Sandstorm_Particles1", 
            //     "Sandstorm_Particles1", 1);
        }
    }
}
