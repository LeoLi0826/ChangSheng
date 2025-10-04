using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET.Client
{
    [Invoke(TimerInvokeType.BattleRound)]
    public class AdventureBattleRoundTimer : ATimer<AdventureComponent>
    {
        protected override void Run(AdventureComponent t)
        {
            //战斗过程
            // t?.PlayOneBattleRound().Coroutine();
        }
    }
    //
    //
    // public class AdventureComponentDestroySystem : DestroySystem<AdventureComponent>
    // {
    //     public override void Destroy(AdventureComponent self)
    //     {
    //         TimerComponent.Instance?.Remove(ref self.BattleTimer);
    //         self.BattleTimer = 0;
    //         self.Round = 0;
    //         self.EnemyIdList.Clear();
    //         self.AliveEnemyIdList.Clear();
    //         self.Random = null;
    //     }
    // }

    [FriendOf(typeof(AdventureComponent))]
    public static class AdventureComponentSystem
    {
        // public static void SetBattleRandomSeed(this AdventureComponent self)
        // {
        //     uint seed = (uint) UnitHelper.GetMyUnitFromCurrentScene(self.ZoneScene().CurrentScene()).GetComponent<NumericComponent>().GetAsInt(NumericType.BattleRandomSeed);
        //     if (self.Random == null)
        //     {
        //         self.Random = new SRandom(seed);
        //     }
        //     else
        //     {
        //         self.Random.SetRandomSeed(seed);
        //     }
        // }

        public static void ResetAdventure(this AdventureComponent self)
        {
            for (int i = 0; i < self.EnemyIdList.Count; i++)
            {
                //清空unit组件下的一些东西
                self.Root().CurrentScene().GetComponent<UnitComponent>().Remove(self.EnemyIdList[i]);
            }

            //TimerComponent.Instance?.Remove(ref self.BattleTimer);
            self.BattleTimer = 0;
            self.Round = 0;
            self.EnemyIdList.Clear();
            self.AliveEnemyIdList.Clear();

            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
            int maxHp = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.MaxHp);
            Log.Debug("玩家血量 " + maxHp);
           
            // unit.GetComponent<NumericComponent>().Set(NumericType.Hp, maxHp);
            unit.GetComponent<NumericComponent>().Set(NumericType.Hp, 99);

            unit.GetComponent<NumericComponent>().Set(NumericType.IsAlive, 1);

            //发送玩家播放动画事件 到显示层
            //Game.EventSystem.PublishAsync(new EventType.AdventureRoundReset() {ZoneScene = self.ZoneScene()}).Coroutine();
        }

        public static async ETTask StartAdventure(this AdventureComponent self,int type)
        {
            //初始化
            self.ResetAdventure();

            switch (type)
            {
                //创建资源
                case 1:
                    //await self.CreateAdventureEnemy();
                    break;
                //创建能量
                case 2:
                    //await self.CreateAdventureEnergy();
                    break;

                    
                
            }
            await ETTask.CompletedTask;
            //创建资源
            //await self.CreateAdventureEnemy();
            //self.ShowAdventureHpBarInfo(true);
            //播放预设的动画 或者战斗
            //self.BattleTimer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 500, TimerType.BattleRound, self);
        }


        // public static  void  ShowAdventureHpBarInfo(this AdventureComponent self,bool isShow)
        // {
        //     Unit myUnit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
        //     ShowAdventureHpBar.Instance.Unit = myUnit;
        //     ShowAdventureHpBar.Instance.isShow = isShow;
        //      Game.EventSystem.PublishClass(ShowAdventureHpBar.Instance);
        //     for ( int i = 0; i < self.EnemyIdList.Count; i++ )
        //     {
        //         Unit monsterUnit =  self.ZoneScene().CurrentScene().GetComponent<UnitComponent>().Get(self.EnemyIdList[i]);
        //         ShowAdventureHpBar.Instance.Unit = monsterUnit;
        //         Game.EventSystem.PublishClass(ShowAdventureHpBar.Instance);
        //     }
        // }
        //
        //
        // public static async ETTask CreateAdventureEnemy(this AdventureComponent self)
        // {
        //     Log.Debug("我开始进入到创建资源1");
        //     //根据关卡ID创建出怪物
        //     Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
        //
        //     //玩家的关卡 我在游戏启动的时候 强行设置了关卡为1
        //     int levelId = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.AdventureState);
        //     levelId = 2;
        //     Log.Debug("levelId:" + levelId);
        //
        // BattleLevelConfig battleLevelConfig = BattleLevelConfigCategory.Instance.Get(levelId);
        // for (int i = 0; i < battleLevelConfig.MonsterIds.Length; i++)
        // {
        //     Log.Debug("创建敌人 ID: ");
        //     
        //     Unit monsterUnit = UnitFactory.CreateMonster(self.Root().CurrentScene(), battleLevelConfig.MonsterIds[i]);
        //     //monsterUnit.Position = new float3(1.5f, -2 + i, 0);
        //     self.EnemyIdList.Add(monsterUnit.Id);
        // }
        //
        //     await ETTask.CompletedTask;
        // }
        // public static async ETTask CreateAdventureEnergy(this AdventureComponent self)
        // {
        //     Log.Debug("我开始进入到创建资源2");
        //     //根据关卡ID创建出怪物
        //     Unit unit   = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
        //     
        //     //玩家的关卡 我在游戏启动的时候 强行设置了关卡为1
        //     int levelId = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.AdventureState);
        //     levelId = 2;
        //     Log.Debug("levelId:"+levelId);
        //     
        //     
        //     Unit UnitPrefab = UnitFactory.CreateEnergy(self.Root().CurrentScene());
        //     Log.Debug("我开始进入到创建资源22: ");
        //     //UnitPrefab.Position = new float3(3f, -4f , -3);
        //     self.EnemyIdList.Add(UnitPrefab.Id);
        //     
        //     // BattleLevelConfig battleLevelConfig = BattleLevelConfigCategory.Instance.Get(levelId);
        //     // for ( int i = 0; i < battleLevelConfig.MonsterIds.Length; i++ )
        //     // {
        //     //     Unit monsterUnit     = await UnitFactory.CreateMonster(self.Root().CurrentScene(), battleLevelConfig.MonsterIds[i]);
        //     //     monsterUnit.Position = new float3(1.5f, -2+i, 0);
        //     //     self.EnemyIdList.Add(monsterUnit.Id);
        //     // }
        //
        //     await ETTask.CompletedTask;
        // }

    }
}