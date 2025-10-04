namespace ET.Client
{
    public static class UnitHelper
    {
        //获取PlayUnit
        public static Unit GetMyUnitFromClientScene(Scene root)
        {
            PlayerComponent playerComponent = root.GetComponent<PlayerComponent>();
            Scene currentScene = root.GetComponent<CurrentScenesComponent>().Scene;
            //Log.Debug("unit帮助类 MyId: "+playerComponent.MyId);
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
        
        public static Unit GetMyUnitFromCurrentScene(Scene currentScene)
        {
            PlayerComponent playerComponent = currentScene.Root().GetComponent<PlayerComponent>();
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
        
        public static NumericComponent GetMyUnitNumericComponent(Scene currentScene)
        {
            PlayerComponent playerComponent = currentScene.Root().GetComponent<PlayerComponent>();
            if ( null == playerComponent )
            {
                return null;
            }
            return currentScene.GetComponent<UnitComponent>()?.Get(playerComponent.MyId)?.GetComponent<NumericComponent>();
        }
    }
}

