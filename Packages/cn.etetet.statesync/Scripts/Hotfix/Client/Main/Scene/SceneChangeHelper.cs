namespace ET.Client
{
    public static class SceneChangeHelper
    {
        public static async ETTask SceneChangeTo(Scene root,string sceneName)
        {
            CurrentScenesComponent currentScenesComponent = root.GetComponent<CurrentScenesComponent>();
            
            // 先卸载当前场景
            currentScenesComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            Scene currentScene = CurrentSceneFactory.Create(IdGenerater.Instance.GenerateInstanceId(), sceneName, currentScenesComponent);
            currentScene.AddComponent<UnitComponent>();
            
            await EventSystem.Instance.PublishAsync(root, new SceneChangeStart());
            Unit unit = null;
            //注意这里的unit创建是客户端的
            switch (UnitConfigCategory.Instance.Get(1001).Type)
            {
                case 1:
                    //Log.Debug("获取锁等待组件1 开始等待");
                    //Log.Debug("获取锁等待组件1 结束等待");
                    unit = UnitFactory.Create(currentScenesComponent.Scene);
                    root.GetComponent<PlayerComponent>().MyId = unit.Id;
                    // EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreateGetView() {Unit = unit});
                    break;
                case 2:
                    Log.Debug("客户端创建怪物单位");
                    // unit = UnitFactory.CreateMonster(currentScene,m2CCreateMyUnit.Unit );
                    break;
                case 3:
                    Log.Debug("客户端创建能量单位");
                    // unit = UnitFactory.CreateEnergy(currentScene, m2CCreateMyUnit.Unit);
                    break;
                
            }
            await EventSystem.Instance.PublishAsync(currentScene, new SceneChangeFinish());
            await ETTask.CompletedTask;
        }
        
        public static bool IsChangeScene(CurrentScenesComponent currentScenesComponent, string sceneName)
        {
            if (currentScenesComponent.Scene == null)
            {
                return true;
            }

            return currentScenesComponent.Scene.Name != sceneName;
        }
    }
}

