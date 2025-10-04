using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;

namespace ET.Client
{
    [EntitySystemOf(typeof(OperaComponent))]
    [FriendOf(typeof(OperaComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapManageComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapChunkControllerComponent))]
    [FriendOfAttribute(typeof(ET.Client.MapGeneratorComponent))]
    public static partial class OperaComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OperaComponent self)
        {
            self.mapMask = LayerMask.GetMask("Map");
            // self.MyUnit = self.GetParent<Unit>();
            self.MyUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Scene());

        }

        [EntitySystem]
        private static void Update(this OperaComponent self)
        {

            self.Move();
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, self.mapMask))
                {
                    //单机模式：直接在客户端处理寻路
                    self.HandlePathfindingOffline(hit.point);
                }
            }

            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     self.Test1().Coroutine();
            // }
            //     
            // if (Input.GetKeyDown(KeyCode.W))
            // {
            //     self.Test2().Coroutine();
            // }

            //
            if (Input.GetKeyDown(KeyCode.R))
            {
                CodeLoader.Instance.Reload();
                return;
            }

            #region 地图切换



            // //换地图
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     C2M_TransferMap c2MTransferMap = C2M_TransferMap.Create();
            //     c2MTransferMap.Num = 1;
            //     self.Root().GetComponent<ClientSenderComponent>().Call(c2MTransferMap).Coroutine();
            //     
            //     //打开相关ui界面
            //     YIUIMgrComponent.Inst.Root.OpenPanelAsync<PlayerUIPanelComponent>().Coroutine();
            //     YIUIMgrComponent.Inst.Root.OpenPanelAsync<QuickWindowPanelComponent>().Coroutine();
            //
            // }

            //换地图（单机模式）
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                try
                {
                    Log.Debug("开始切换到Map1场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map1").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map1场景失败：{ex.Message}");
                }
            }

            //换地图（单机模式）
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                try
                {
                    Log.Debug("开始切换到Map2场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map2").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map2场景失败：{ex.Message}");
                }
            }

            //换地图（单机模式）
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                try
                {
                    Log.Debug("开始切换到Map3场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map3").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map3场景失败：{ex.Message}");
                }
            }

            //换地图
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                try
                {
                    Log.Debug("开始切换到Map4场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map4").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map3场景失败：{ex.Message}");
                }
            }
            //换地图
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                try
                {
                    Log.Debug("开始切换到Map5场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map5").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map3场景失败：{ex.Message}");
                }
            }
            //换地图
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                try
                {
                    Log.Debug("开始切换到Map6场景");
                    
                    // 清理当前场景的Map相关组件，避免NullReferenceException
                    Scene currentScene = self.Root().CurrentScene();
                    if (currentScene != null)
                    {
                        var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
                        if (mapManageComponent != null)
                        {
                            Log.Debug("清理MapManageComponent");
                            mapManageComponent.Dispose();
                        }
                        
                        var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
                        if (mapGeneratorComponent != null)
                        {
                            Log.Debug("清理MapGeneratorComponent");
                            mapGeneratorComponent.Dispose();
                        }
                    }
                    
                    // 执行场景切换
                    SceneChangeHelper.SceneChangeTo(self.Root(), "Map6").NoContext();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"切换到Map6场景失败：{ex.Message}");
                }
            }
            // //换地图
            // if (Input.GetKeyDown(KeyCode.Alpha7))
            // {
            //     try
            //     {
            //         Log.Debug("开始切换到Map7场景");
            //         
            //         // 清理当前场景的Map相关组件，避免NullReferenceException
            //         Scene currentScene = self.Root().CurrentScene();
            //         if (currentScene != null)
            //         {
            //             var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
            //             if (mapManageComponent != null)
            //             {
            //                 Log.Debug("清理MapManageComponent");
            //                 mapManageComponent.Dispose();
            //             }
            //             
            //             var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
            //             if (mapGeneratorComponent != null)
            //             {
            //                 Log.Debug("清理MapGeneratorComponent");
            //                 mapGeneratorComponent.Dispose();
            //             }
            //         }
            //         
            //         // 执行场景切换
            //         SceneChangeHelper.SceneChangeTo(self.Root(), "Map7").Coroutine();
            //     }
            //     catch (System.Exception ex)
            //     {
            //         Log.Error($"切换到Map7场景失败：{ex.Message}");
            //     }
            // }
            // //换地图
            // if (Input.GetKeyDown(KeyCode.Alpha8))
            // {
            //     try
            //     {
            //         Log.Debug("开始切换到Map8场景");
            //         
            //         // 清理当前场景的Map相关组件，避免NullReferenceException
            //         Scene currentScene = self.Root().CurrentScene();
            //         if (currentScene != null)
            //         {
            //             var mapManageComponent = currentScene.GetComponent<MapManageComponent>();
            //             if (mapManageComponent != null)
            //             {
            //                 Log.Debug("清理MapManageComponent");
            //                 mapManageComponent.Dispose();
            //             }
            //             
            //             var mapGeneratorComponent = currentScene.GetComponent<MapGeneratorComponent>();
            //             if (mapGeneratorComponent != null)
            //             {
            //                 Log.Debug("清理MapGeneratorComponent");
            //                 mapGeneratorComponent.Dispose();
            //             }
            //         }
            //         
            //         // 执行场景切换
            //         SceneChangeHelper.SceneChangeTo(self.Root(), "Map8").Coroutine();
            //     }
            //     catch (System.Exception ex)
            //     {
            //         Log.Error($"切换到Map8场景失败：{ex.Message}");
            //     }
            // }
            #endregion


            //打开
            if (Input.GetKeyDown(KeyCode.P))
            {
                //打开相关ui界面
               
                self.Scene().YIUIRoot().OpenPanelAsync<PlayerUIPanelComponent>().NoContext();
                self.Scene().YIUIRoot().OpenPanelAsync<QuickWindowPanelComponent>().NoContext();

            }

            //创建敌人
            if (Input.GetKeyDown(KeyCode.N))
            {
                //AdventureComponentSystem 逻辑代码在这里
                Log.Debug("我按下了N 创建敌人～");
                //打开相关ui界面
                //先发送信息到服务端 把背包里的新物品卖掉 
                // C2M_NewResource C2M_newResource = C2M_NewResource.Create();
                // //C2M_sell.ItemInfo = temp.ToMessage(true);
                // //C2M_newResource.UnitId = self.Root().GetComponent<PlayerComponent>().MyId ;;
                // self.Root().GetComponent<ClientSenderComponent>().Send(C2M_newResource);


                self.Root().GetComponent<AdventureComponent>().StartAdventure(1).NoContext();
            }


            if (Input.GetKeyDown(KeyCode.N))
            {

                //AdventureComponentSystem 逻辑代码在这里
                Log.Debug("我按下了N 创建能量～");
                //打开相关ui界面
                //先发送信息到服务端 把背包里的新物品卖掉 
                // C2M_NewResource C2M_newResource = C2M_NewResource.Create();
                // //C2M_sell.ItemInfo = temp.ToMessage(true);
                // //C2M_newResource.UnitId = self.Root().GetComponent<PlayerComponent>().MyId ;;
                // self.Root().GetComponent<ClientSenderComponent>().Send(C2M_newResource);

                //创建资源
                self.Root().GetComponent<AdventureComponent>().StartAdventure(2).NoContext();
            }

            //测试生成法宝到物品栏
            if (Input.GetKeyDown(KeyCode.J))
            {
                // 从1001-1050中随机选择一个数值
                int randomConfigId = UnityEngine.Random.Range(1002, 1017);
                //获取法宝那边 也得改

                //固定获取 宝莲灯
                Test1(self, 1008).NoContext();
            }
            //测试生成材料到物品栏
            if (Input.GetKeyDown(KeyCode.H))
            {
                // 从1001-1050中随机选择一个数值
                int randomConfigId = UnityEngine.Random.Range(1053, 1070);
                Test1(self, randomConfigId).NoContext();
            }


            if (Input.GetKeyDown(KeyCode.M))
            {
                //法力减少测试
                // self.DynamicEvent(self.Root(),new ManaReduceRefresh() { date = 30 }).NoContext();
            
               //self.Root().GetComponent<QuickWindowPanelComponent>().ManaReduce(30);
            
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //法力增加测试
                // self.DynamicEvent(self.Root(),new ManaAddRefresh() { date = 500 }).NoContext();
                //生命减少测试
               //self.Root().DynamicEvent(self.Root(),new LifeReduceRefresh() { date = 30 }).Coroutine();
            
                //self.Root().GetComponent<QuickWindowPanelComponent>().LifeReduce(10);
            
            }

            //天劫将领测试
            if (Input.GetKeyDown(KeyCode.Y))
            {
                // self.DynamicEvent(self.Root(), new CalamityAdd() { day = 30 }).NoContext();

                //self.Root().GetComponent<QuickWindowPanelComponent>().LifeReduce(10);

            }

            //天劫将领测试
            if (Input.GetKeyDown(KeyCode.U))
            {
                // self.DynamicEvent(self.Root(), new CalamityReduce() { day = 30 }).NoContext();

                //self.Root().GetComponent<QuickWindowPanelComponent>().LifeReduce(10);

            }
            
            //移动
            //self.Handel();


            //每帧把当前帧收集的操作发送给服务端，随后清除
            // if (self.OperateInfos.Count == 0)
            //     return;
            // self.OperateInfosTemp.Clear();
            // self.OperateInfosTemp.AddRange(self.OperateInfos);
            // C2Room_Operation c2RoomOperation = C2Room_Operation.Create();
            // c2RoomOperation.OperateInfos = self.OperateInfosTemp;
            // self.Root().GetComponent<ClientSenderComponent>().Send(c2RoomOperation);
            // self.OperateInfos.Clear();
        }

        private static async ETTask Test1(this OperaComponent self, int configId)
        {
            // 客户端直接处理物品获取逻辑，不需要发送到服务器
            Log.Debug("客户端直接获取物品，ConfigId: " + configId);

            // 获取当前玩家的背包组件
            ItemContainerComponent itemContainerComponent = self.Root().GetComponent<ItemContainerComponent>();
            ItemContainerHelper.AddItem(itemContainerComponent, ItemContainerType.Backpack, configId,1);

            //背包刷新
            // await self.DynamicEvent(new EventBagItemReFresh());
            // 快捷栏刷新
            // await self.DynamicEvent(new EventQuickItemReFresh());
            // 快捷法宝栏刷新
            // await self.DynamicEvent(new EventQuickItemFunctionReFresh());


            await ETTask.CompletedTask;
        }

        private static async ETTask Test2(this OperaComponent self)
        {
            Log.Debug($"Croutine 2 start2");
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(1, 20000, 3000))
            {
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }
            Log.Debug($"Croutine 2 end2");
            await ETTask.CompletedTask;
        }

        /// <summary>
        /// 单机模式地图切换，不需要服务器通信
        /// </summary>
        private static async ETTask TransferMapOffline(this OperaComponent self, int mapNum)
        {
            try
            {
                Log.Debug($"开始单机地图切换到地图{mapNum}");

                // 安全检查
                Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
                if (unit == null || unit.IsDisposed)
                {
                    Log.Warning("MyUnit已被销毁，无法切换地图");
                    return;
                }
                var numericComponent = unit?.GetComponent<NumericComponent>();
                if (numericComponent == null || numericComponent.IsDisposed)
                {
                    Log.Warning("NumericComponent已被销毁，无法切换地图");
                    return;
                }

                // 更新玩家数值（模拟服务端逻辑）
                numericComponent.Set(NumericType.Map, mapNum);
                numericComponent.Set(NumericType.AdventureState, mapNum);

                Log.Debug($"已更新玩家地图数值：Map={mapNum}, AdventureState={mapNum}");

                // 清理当前地图
                Scene currentScene = self.Root().CurrentScene();
                var mapManageComponent = currentScene?.GetComponent<MapManageComponent>();
                if (mapManageComponent != null)
                {
                    Log.Debug("清理当前地图数据");

                    // 清理地图块字典
                    if (mapManageComponent.mapChunkDic != null)
                    {
                        foreach (var chunkRef in mapManageComponent.mapChunkDic.Values)
                        {
                            MapChunkControllerComponent chunk = chunkRef;
                            if (chunk != null && !chunk.IsDisposed)
                            {
                                chunk.SetActive(false);
                                if (chunk.GameObject != null)
                                {
                                    UnityEngine.Object.Destroy(chunk.GameObject);
                                }
                                chunk.Dispose();
                            }
                        }
                        mapManageComponent.mapChunkDic.Clear();
                    }

                    // 清理可见块列表
                    if (mapManageComponent.lastVisibleChunkList != null)
                    {
                        mapManageComponent.lastVisibleChunkList.Clear();
                    }
                }

                // 重新初始化地图生成器
                var mapGeneratorComponent = currentScene?.GetComponent<MapGeneratorComponent>();
                if (mapGeneratorComponent != null)
                {
                    Log.Debug("重新初始化地图生成器");
                    
                    // 设置新的地图编号
                    mapGeneratorComponent.MapNum = mapNum;
                    mapGeneratorComponent.MapVertex1 = (MapVertexType)(mapNum * 2 - 1);
                    mapGeneratorComponent.MapVertex2 = (MapVertexType)(mapNum * 2);
                    
                    // 重新计算权重总和（重要！）
                    mapGeneratorComponent.forestSpawanWeightTotal = 0;
                    mapGeneratorComponent.marshSpawanWeightTotal = 0;
                    mapGeneratorComponent.AIforestSpawanWeightTotal = 0;
                    mapGeneratorComponent.AImarshSpawanWeightTotal = 0;
                    
                    // 重新计算森林权重
                    var temps = MapConfigCategory.Instance.GetListByMapVertexType((int)mapGeneratorComponent.MapVertex1);
                    if (temps != null)
                    {
                        for (int i = 0; i < temps.Count; i++)
                        {
                            mapGeneratorComponent.forestSpawanWeightTotal += temps[i].Probability;
                        }
                    }
                    
                    // 重新计算沼泽权重
                    temps = MapConfigCategory.Instance.GetListByMapVertexType((int)mapGeneratorComponent.MapVertex2);
                    if (temps != null)
                    {
                        for (int i = 0; i < temps.Count; i++)
                        {
                            mapGeneratorComponent.marshSpawanWeightTotal += temps[i].Probability;
                        }
                    }
                    
                    // 重新计算AI权重
                    var forestMapAIConfigs = MapAIConfigCategory.Instance.GetListByMapVertexType((int)mapGeneratorComponent.MapVertex1);
                    if (forestMapAIConfigs != null)
                    {
                        for (int i = 0; i < forestMapAIConfigs.Count; i++)
                        {
                            mapGeneratorComponent.AIforestSpawanWeightTotal += forestMapAIConfigs[i].Probability;
                        }
                    }
                    
                    var marshMapAIConfigs = MapAIConfigCategory.Instance.GetListByMapVertexType((int)mapGeneratorComponent.MapVertex2);
                    if (marshMapAIConfigs != null)
                    {
                        for (int i = 0; i < marshMapAIConfigs.Count; i++)
                        {
                            mapGeneratorComponent.AImarshSpawanWeightTotal += marshMapAIConfigs[i].Probability;
                        }
                    }
                    
                    // 重新生成地图数据
                    MapGeneratorComponentSystem.GenerateMapData(mapGeneratorComponent);
                    
                    Log.Debug($"地图生成器已切换到地图{mapNum}，地形类型：{mapGeneratorComponent.MapVertex1}, {mapGeneratorComponent.MapVertex2}");
                }

                // 重新初始化地图管理器
                if (mapManageComponent != null)
                {
                    Log.Debug("重新初始化地图管理器");
                    mapManageComponent.DataInit();
                    await mapManageComponent.CreateVisibleChunk();
                    mapManageComponent.UpdateVisibleChunk().NoContext();
                }

                // 打开相关UI界面
                self.Root().YIUIRoot().OpenPanelAsync<PlayerUIPanelComponent>().NoContext();
                // self.Root().YIUIRoot().OpenPanelAsync<QuickWindowPanelComponent>().NoContext();

                Log.Debug($"单机地图切换完成，当前地图：{mapNum}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"单机地图切换失败：{ex.Message}\n{ex.StackTrace}");
            }

            await ETTask.CompletedTask;
        }

        /// <summary>
        /// 单机模式寻路处理，不需要服务器通信
        /// </summary>
        private static void HandlePathfindingOffline(this OperaComponent self, Vector3 targetPosition)
        {
            try
            {
                // 安全检查
                Unit unit = self.MyUnit;
                if (unit == null || unit.IsDisposed)
                {
                    Log.Warning("MyUnit已被销毁，无法处理寻路");
                    return;
                }

                // 获取玩家行为组件
                var playerBehaviour = unit.GetComponent<PlayerBehaviourComponent>();
                if (playerBehaviour != null && !playerBehaviour.IsDisposed)
                {
                    // 直接在客户端处理移动
                    Log.Debug($"单机模式寻路到目标位置：{targetPosition}");

                    // 这里可以实现具体的寻路逻辑
                    // 比如设置目标位置，启动移动等
                    unit.Position = targetPosition; // 简单的瞬移，可以根据需要实现平滑移动
                }
                else
                {
                    Log.Warning("PlayerBehaviourComponent不存在，无法处理寻路");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"单机寻路处理失败：{ex.Message}");
            }
        }
    }

    [FriendOf(typeof(OperaComponent) )]
    //[FriendOf(typeof(ET.MoveComponent) )]
    
   // [FriendOf(typeof(ET.Move2DComponent) )]
    public static partial class OperaComponentSystem
    {
        public static void OnMove(this OperaComponent self, Vector2 v2)
        {
            // Log.Info($"press joystick: {v2}");
            // // C2M_JoystickMove c2mJoystickMove = new C2M_JoystickMove() { MoveForward = new float3(v2.x, 0, v2.y) };
            // // self.ClientScene().GetComponent<PlayerSessionComponent>().Session.Send(c2mJoystickMove);
            // OperateInfo operateInfo = OperateInfo.Create();
            // operateInfo.OperateType = (int)EOperateType.Move;
            // operateInfo.InputType = (int)EInputType.KeyDown;
            // operateInfo.Vec3 = new float3(v2.x,v2.y,0);
            
            // self.OperateInfos.Add(operateInfo);
        }
        public static void StopMove(this OperaComponent self)
        {
            // C2M_JoystickMove c2mJoystickMove = new C2M_JoystickMove() { MoveForward = new float3(v2.x, 0, v2.y) };
            // self.ClientScene().GetComponent<PlayerSessionComponent>().Session.Send(c2mJoystickMove);
            // OperateInfo operateInfo = OperateInfo.Create();
            // operateInfo.OperateType = (int)EOperateType.Move;
            // operateInfo.InputType = (int)EInputType.KeyUp;
            // self.OperateInfos.Add(operateInfo);
        }
        
        
        
        
        [EntitySystem]
        private static void LateUpdate(this OperaComponent self)
        {
           
        }

        public static void Handel(this OperaComponent self)
        {
            // if (self.OperateInfos == null || self.OperateInfos.Count == 0)
            // {
            //     //Log.Error($"reveice null operate info");
            //     return;
            // }
            //
            // PlayerComponent playerComponent = self.Root().GetComponent<PlayerComponent>();
            //
            // Unit unit = self.Root().CurrentScene().GetComponent<UnitComponent>().Get(playerComponent.MyId);
            //
            // if (unit == null)
            // {
            //     Log.Debug("房间的unit 获取失败！！");
            // }
            // else
            // {
            //     Log.Debug("房间的unit 获取成功！！");
            // }
            //
            // if (unit == null)
            // {
            //     Log.Error($"cant not find unit, player id : {playerComponent.MyId}");
            //     return;
            // }
            //
            // foreach (OperateInfo operateInfo in self.OperateInfos)
            // {
            //     EOperateType operateType = (EOperateType)operateInfo.OperateType;
            //     switch (operateType)
            //     {
            //         case EOperateType.Move:
            //         {
            //             Debug.Log("我开始移动了，我进入了移动");
            //             //收到移动消息，往前移动，如果有地形，需要判定前方位置是否可以移动。
            //             //移动逻辑挪到移动组件处理
            //             // float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            //             // speed = speed == 0 ? 3 : speed;
            //             // float3 v3 = unit.Position + operateInfo.Vec3 * speed / DefineCore.LogicFrame;
            //             // unit.Position = v3;
            //             if ((EInputType)operateInfo.InputType == EInputType.KeyUp)
            //             {
            //                 Debug.Log("我停止移动了123");
            //                 //unit.GetComponent<PlayerBehaviourComponent>().StopMove();
            //             }
            //             else
            //             {
            //                 Debug.Log("我开始移动了123 ");
            //                 unit.MoveDir = operateInfo.Vec3;
            //                 //unit.GetComponent<PlayerBehaviourComponent>().StartMove();
            //             }
            //             //这里被注释了
            //             // Log.Debug("房间的unit 发送给客户端！！");
            //             // Room2C_JoystickMove m2CJoystickMove = Room2C_JoystickMove.Create();
            //             // m2CJoystickMove.Position = unit.Position;
            //             // m2CJoystickMove.MoveForward = unit.Forward; 
            //             // m2CJoystickMove.Id = unit.Id;
            //             //
            //             // MapMessageHelper.Broadcast(unit, m2CJoystickMove);
            //
            //             break;
            //         }
            //         case EOperateType.Attack:
            //         {
            //
            //             break;
            //         }
            //         case EOperateType.Skill1:
            //         {
            //             //主动技能1
            //             // if (unit?.GetComponent<SkillComponent>()?.SpellSkill(ESkillAbstractType.ActiveSkill) == true)
            //             // {
            //             //     OperateReplyInfo info = OperateReplyInfo.Create();
            //             //     info.OperateType = (int)operateType;
            //             //     info.Status = 0;
            //             //     room2COperation.OperateInfos.Add(info);
            //             // }
            //             break;
            //         }
            //         case EOperateType.Skill2:
            //         {
            //             //主动技能2
            //             // if (unit?.GetComponent<SkillComponent>()?.SpellSkill(ESkillAbstractType.ActiveSkill, 1) == true)
            //             // {
            //             //     OperateReplyInfo info = OperateReplyInfo.Create();
            //             //     info.OperateType = (int)operateType; info.Status = 0;
            //             //     room2COperation.OperateInfos.Add(info);
            //             // }
            //             break;
            //         }
            //     }
            // }
        }


        public static void JoyMove2D(this OperaComponent self, Vector3 moveDir)
        {
            Debug.Log("我开始移动了");
            
            //在6.0 巡逻机器人(一)里 有一个判定是否是玩家本人的算法 可能不需要？
            PlayerComponent playerComponent = self.Root().GetComponent<PlayerComponent>();
            
            Unit unit = self.Root().CurrentScene().GetComponent<UnitComponent>().Get(playerComponent.MyId);
            
            Vector3 unitPos = unit.Position;
            // unitPos. y = 0;
            unitPos.z = 0;
            Debug.Log("移动前位置： "+unitPos);
        
            //预测的参数如果延迟大 就增加，如果延迟小 就减少
            // int preTime = 1;
            
            // Vector3 newPos = new Vector3(unit.Position.x+moveDir.x * preTime, unit.Position.y +moveDir.y*preTime ,0);//unitPos + (moveDir * 4f);
            // Debug.Log("移动后位置： "+newPos);
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            Debug.Log("速度： "+speed);
            speed = 10;
            //unit.Forward = moveDir;
            unit.MoveDir = moveDir;
            unit.GetComponent<PlayerBehaviourComponent>().StartMove();

            //float3 deltaPos = moveDir * speed / DefineCore.LogicFrame;
            //unit.Position += deltaPos;
            
            List<float3> path = new List<float3>
            {
                unit.Position,
                // newPos
            };
            //在moveHelp里面
            //unit.MoveToAsync2D(unit.Position).Coroutine();
            // path = null;
            
            
            //服务器方面
            //发给服务器的移动
            // C2M_PathfindingResult c2MPathfindingResult = C2M_PathfindingResult.Create();
            // c2MPathfindingResult.Position = newPos;
            // //Debug.Log("移动目标ID： "+c2MPathfindingResult.RpcId);
            //         
            // //把点击点目的地点发送给服务器 让服务器来处理
            // self.Root().GetComponent<ClientSenderComponent>().Send(c2MPathfindingResult);
            
        }
        public static void Stop(this OperaComponent self)
        {
            Debug.Log("我移动停止了！");
            PlayerComponent playerComponent = self.Root().GetComponent<PlayerComponent>();

            Unit unit = self.Root().CurrentScene().GetComponent<UnitComponent>().Get(playerComponent.MyId);

            bool ans = false;
            //unit.GetComponent<Move2DComponent>().Stop(ans);
            Debug.Log("Stop了吗？： " + ans);
            
            //
            Vector3 unitPos = unit.Position;
            
            //服务器相关
            C2M_PathfindingResult c2MPathfindingResult = C2M_PathfindingResult.Create();
            c2MPathfindingResult.Position = unitPos;
                    
            //把点击点目的地点发送给服务器 让服务器来处理
            self.Root().GetComponent<ClientSenderComponent>().Send(c2MPathfindingResult);
            
        }


        public static void Move(this OperaComponent self)
        {
        
            Unit unit = self.MyUnit;
            float inputX = Input.GetAxis("Horizontal"); // 获取水平输入（X 轴）
            float inputZ = Input.GetAxis("Vertical"); // 获取垂直输入（Z 轴）
            if (inputX <= 0.01f && inputZ <= 0.01f)
            {
                return;
            }
            // 将输入转换为 3D 向量，并忽略 Y 轴
            float3 input = new float3(inputX, 0, inputZ);
            unit.FindPathMoveToAsync(unit.Position+input).NoContext();
        }
    }
}