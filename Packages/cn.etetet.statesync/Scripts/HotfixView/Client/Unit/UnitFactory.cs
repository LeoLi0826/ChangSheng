using UnityEngine;

namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.Unit))]
    [FriendOfAttribute(typeof(ET.Client.MapRegionUnit))]
    public static class UnitFactory
    {
        public static async ETTask<Unit> CreateLongJuanFeng(Scene currentScene, Vector3 pos)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChild<Unit>();
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            unit.UnitType = UnitType.LongJuanFeng;
            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            numericComponent.SetNoEvent(NumericType.MaxHp, 1);
            numericComponent.SetNoEvent(NumericType.Hp, numericComponent[NumericType.MaxHp]);
            unit.AddComponent<ObjectWait>();
            await EventSystem.Instance.PublishAsync(unit.Root(), new AfterUnitCreate_LongJuanFeng()
            {
                Unit = unit,
                X = pos.x,
                Y = pos.y,
                Z = pos.z,
            });
            return unit;
        }
        
        //创建怪物
        public static void CreateMapObject(Scene currentScene, MapRegionUnit mapRegionUnit)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            MapAIConfig mapAIConfig = MapAIConfigCategory.Instance.Get(mapRegionUnit.ConfigId);
            if (mapAIConfig.UnitID == 0)
            {
                return;
            }
            Unit unit = unitComponent.AddChildWithId<Unit,int>(mapRegionUnit.UnitId,mapAIConfig.UnitID);
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.ConfigId);
            unit.unitActionType = UnitActionType.Idle;
            unit.UnitType = (UnitType)unitConfig.Type;
            unit.Position = mapRegionUnit.Position;
            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            //numericComponent.SetNoEvent(NumericType.DamageValue,unitConfig.DamageValue);
            
            if (unit.ConfigId == 1082) //黄风大圣的话
            {
                Log.Debug("unitInfo.ConfigId: "+ unit.ConfigId+" MaxHp: "+unitConfig.MaxHP);
                numericComponent.SetNoEvent(NumericType.MaxHp, 500);
               
            }
            else
            {
                numericComponent.SetNoEvent(NumericType.MaxHp, unitConfig.MaxHP > 0 ? unitConfig.MaxHP : 100);
                
            }
            
            //numericComponent.SetNoEvent(NumericType.MaxHp, unitConfig.MaxHP > 0 ? unitConfig.MaxHP : 100);
            numericComponent.SetNoEvent(NumericType.Hp, numericComponent[NumericType.MaxHp]);
            //numericComponent.SetNoEvent(NumericType.MaxBagCapacity,20);

            unit.AddComponent<ObjectWait>();
            EventSystem.Instance.Publish(unit.Scene(), new AfterMapUnitCreate() { Unit = unit });
        }
    }
}
 
