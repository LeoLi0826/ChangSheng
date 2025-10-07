
using Unity.Mathematics;

namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.Unit))]
    public static partial class UnitFactory
    {
        //玩家创建 
        public static Unit Create(Scene currentScene)
        {
            
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), 1001);
            unitComponent.Add(unit);
            Log.Debug("玩家创建1 客户端用户创建：");

            
            // unit.Position = unitInfo.Position;
            //unit.Forward = unitInfo.Forward;
            
            
            unit.UnitType = UnitType.Player;
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            numericComponent.SetNoEvent(NumericType.SpeedBase,10000);

            
            
            //读取配表 更新数据
            NumbericInit2(unit, numericComponent);

            HandBookInit(unit, unit.GetComponent<C_HandBookComponent>());

            //unit.AddComponent<MoveComponent>();
            // 增加战意
            // unit.AddComponent<BattleWillComponent>();

            unit.AddComponent<ObjectWait>();

            //AfterUnitCreate_CreateUnitView在这个函数里面调用
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() { Unit = unit });


            numericComponent[NumericType.MaxHpBase] = 1000;
            // numericComponent[NumericType.HpBase] = numericComponent[NumericType.MaxHp];
            numericComponent[NumericType.HpBase] = 1000;
            numericComponent[NumericType.HpLoseSpeedBase] = 1;

            
            // unit.AddComponent<PlayerMoveComponent>();

            //unit.AddComponent<XunLuoPathComponent>();

            return unit;
        }

        #region 单机暂时 没用到 

        
        //玩家创建 
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unitComponent.Add(unit);

            Log.Debug("玩家创建2 客户端用户创建：ConfigId " + unitInfo.ConfigId + " UnitId " + unitInfo.UnitId);
            unit.Position = unitInfo.Position;
            //unit.Forward = unitInfo.Forward;
            unit.UnitType = UnitType.Player;
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
            //同步来自于数据库的数值到玩家的数值组件
            NumbericInit(unit, numericComponent, unitInfo);

            HandBookInit(unit, unit.GetComponent<C_HandBookComponent>());


            // 增加战意
            // unit.AddComponent<BattleWillComponent>();
            unit.AddComponent<ObjectWait>();

            //AfterUnitCreate_CreateUnitView在这个函数里面调用
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() { Unit = unit });




            // unit.AddComponent<PlayerMoveComponent>();

            //unit.AddComponent<XunLuoPathComponent>();

            return unit;
        }

        //怪物创建
        public static Unit CreateMonster(Scene currentScene, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Log.Debug("客户端怪物创建1：UnitConfigId " + unitInfo.ConfigId);

            //这里 暂时设置为客户端的 服务端那边没有联通
            Unit unit = new Unit();
            if (unitInfo.UnitId == 0)
            {
                unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), unitInfo.ConfigId);
            }
            else
            {
                unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            }

            //Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), unitInfo.ConfigId);
            unitComponent.Add(unit);

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unitInfo.ConfigId);

            unit.UnitType = (UnitType)unitConfig.Type;

            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            //numericComponent.SetNoEvent(NumericType.DamageValue,unitConfig.DamageValue);
            int maxHp = 0;
            // 确保怪物有合理的默认血量，如果配置表中血量为0或无效则设为100 
            if (unitInfo.ConfigId == 1082) //黄风大圣的话
            {
                Log.Debug("unitInfo.ConfigId: "+ unitInfo.ConfigId+" MaxHp: "+unitConfig.MaxHP);
                maxHp = 3000; //unitConfig.MaxHP > 0 ? unitConfig.MaxHP : 3000;
            }
            else
            {
                maxHp = unitConfig.MaxHP > 0 ? unitConfig.MaxHP : 3000;
            }
            
            numericComponent.SetNoEvent(NumericType.MaxHp, maxHp);
            numericComponent.SetNoEvent(NumericType.Hp, maxHp);

            unit.AddComponent<ObjectWait>();

            //AfterUnitCreate_CreateUnitView在这个函数里面调用
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() { Unit = unit });

            return unit;
        }

        //怪物创建
        public static Unit CreateMapUnit(Scene currentScene, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Log.Debug("怪物 客户端 地图植物 创建1：ConfigId " + unitInfo.ConfigId);

            //这里 暂时设置为客户端的 服务端那边没有联通
            Unit unit = new Unit();
            if (unitInfo.UnitId == 0)
            {
                unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), unitInfo.ConfigId);
            }
            else
            {
                unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            }

            //Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), unitInfo.ConfigId);
            unitComponent.Add(unit);

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unitInfo.ConfigId);

            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            //numericComponent.SetNoEvent(NumericType.DamageValue,unitConfig.DamageValue);
            
            // 确保地图单位有合理的默认血量，如果配置表中血量为0或无效则设为100
            int maxHp = unitConfig.MaxHP > 0 ? unitConfig.MaxHP : 100;
            numericComponent.SetNoEvent(NumericType.MaxHp, maxHp);
            numericComponent.SetNoEvent(NumericType.Hp, maxHp);

            unit.AddComponent<ObjectWait>();

            //AfterUnitCreate_CreateUnitView在这个函数里面调用
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() { Unit = unit });

            return unit;
        }

        public static Unit CreateEnergy(Scene currentScene, UnitInfo unitInfo)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            // Log.Debug("客户端怪物创建2：ConfigId " + unitInfo.ConfigId);

            //随机id写法
            //Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), unitInfo.ConfigId);
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);


            unitComponent.Add(unit);

            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

            UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unitInfo.ConfigId);

            numericComponent.SetNoEvent(NumericType.IsAlive, 1);
            // numericComponent.SetNoEvent(NumericType.DamageValue,unitConfig.DamageValue);
            // numericComponent.SetNoEvent(NumericType.MaxHp,unitConfig.MaxHP);
            // numericComponent.SetNoEvent(NumericType.Hp,unitConfig.MaxHP);
            //Log.Debug("客户端怪物创建2：ConfigId " + unitInfo.ConfigId);
            unit.AddComponent<ObjectWait>();

            //AfterUnitCreate_CreateUnitView在这个函数里面调用
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() { Unit = unit });


            return unit;
        }
        
        #endregion
        
        public static void NumbericInit(Unit unit, NumericComponent numericComponent, UnitInfo unitInfo)
        {
            //读取配表里的数据
            foreach (var kv in unitInfo.KV)
            {
                //Log.Debug("我是客户端数值初始化：key："+kv.Key+" value:"+kv.Value);
                numericComponent.Set(kv.Key, kv.Value);
            }

            unit.GetComponent<NumericComponent>().Set(NumericType.AdventureState, 1);
            //Log.Debug("来自数据库的神秘力量 Speed：" + numericComponent.GetAsFloat(NumericType.Speed));
            //Log.Debug("来自数据库的神秘力量 Speed：" + numericComponent.GetAsFloat(NumericType.Speed));

            //人为设置
            //numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
            //numericComponent.Set(NumericType.AOI, 15000); // 视野15米

            //numericComponent.Set(NumericType.MaxBagCapacity, 100); // 背包最大负重
            //读取配置表的设置 下面是一个例子
            //UnitConfig unitConfig = UnitConfigCategory.Instance.Get(unit.Id);
            //numericComponent.Set(NumericType.MaxBagCapacity, UnitConfig.Id); // 背包最大负重

            numericComponent.Set(NumericType.MaxHp, 1000);
            numericComponent.Set(NumericType.Hp, 1000);
            numericComponent.SetNoEvent(NumericType.MaxBagCapacity,20);
            
        }

        public static void NumbericInit2(Unit unit, NumericComponent numericComponent)
        {
            UnitConfig unitInfo = UnitConfigCategory.Instance.Get(unit.ConfigId);
            Log.Debug("数值组件 数据读取 unit.ConfigId"+unit.ConfigId+"HpMax"+unitInfo.MaxHP);
            
            numericComponent[NumericType.MaxHpBase] = 1000;
            numericComponent[NumericType.HpBase] = numericComponent[NumericType.MaxHp];
            numericComponent[NumericType.HpLoseSpeedBase] = 1;
            
            
            numericComponent[NumericType.MaxMpBase] = 1000;
            numericComponent[NumericType.MpBase] = numericComponent[NumericType.MaxMp];
            numericComponent[NumericType.MpLoseSpeedBase] = 1;

            numericComponent[NumericType.MaxBagCapacity] = 20;
        }

        public static void HandBookInit(Unit unit, C_HandBookComponent handBook)
        {
            //unit.AddComponent<C_HandBookComponent>();
        }
    }
}