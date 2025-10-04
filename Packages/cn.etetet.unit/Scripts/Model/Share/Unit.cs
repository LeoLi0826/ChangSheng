using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using Unity.Mathematics;

namespace ET
{
    public enum Direction
    {
        None = 0,
        Front = 1,    // 正面朝向
        Back = 2,     // 背面朝向
        Left = 3,     // 左侧朝向
        Right = 4,    // 右侧朝向
    }
    public enum UnitActionType
    {
        None,           // 无动作
        Idle,           // 待机
        Walk,           // 行走
        Run,            // 奔跑
        Attack,         // 普通攻击
        Weak,           // 虚弱
        HuangFengXuLi_1,
        HuangFengXuLi_2,
        HuangFengXuLi_3,
        HuangFengAttack1,     // 蓄力攻击1
        HuangFengAttack2,     // 蓄力攻击2
        HuangFengAttack3,     // 蓄力攻击3
        Zhaojia,
        Pick,           // 捡
        Skill,          // 技能
        Hurt,           // 受伤
        Die,            // 死亡
        Jump,           // 跳跃
        Fall,           // 下落
        Land,           // 落地
        Dash,           // 冲刺
        Block,          // 格挡
        Dodge,          // 闪避
        Victory,        // 胜利
        Defeat,         // 失败
        Stun,           // 眩晕
        Revive,         // 复活
        Interact,       // 交互
        Cast,           // 施法
        Channel,        // 引导
        Mount,          // 骑乘
        Dismount,       // 下坐骑
        Swim,           // 游泳
        Climb,          // 攀爬
        Fly,            // 飞行
        Sit,            // 坐下
        Dance,          // 跳舞
        Emote,          // 表情
        Taunt,          // 嘲讽
        Cheer,          // 欢呼
    }

    public enum MonsterActionType
    {
        None,
        Idle,
        Walk,
        Attack,
    }
    
    
    [ChildOf(typeof(UnitComponent))]
    [DebuggerDisplay("ViewName,nq")]
    public partial class Unit: Entity,IAwake, IAwake<int>
    {
        public int ConfigId { get; set; } //配置表id

        public UnitActionType unitActionType;

        public UnitType UnitType;
        public MonsterActionType MonsterActionType;
        public Direction Direction { get; set; } = Direction.Front; // 默认朝向为正面

        [BsonElement]
        private float3 position; //坐标

        [BsonIgnore]
        public float3 Position
        {
            get => this.position;
            set
            {
                float3 oldPos = this.position;
                this.position = value;
                EventSystem.Instance.Publish(this.Scene(), new ChangePosition() { Unit = this, OldPos = oldPos });
            }
        }
        
        [BsonElement]
        private float3 moveDir; //移动的方向
        
        [BsonIgnore]
        public float3 MoveDir
        {
            get => this.moveDir;

            set
            {
                this.moveDir = value;
            }
        }

        [BsonIgnore]
        public float3 Forward
        {
            get => math.mul(this.Rotation, math.forward());
            set => this.Rotation = quaternion.LookRotation(value, math.up());
        }
        
        [BsonElement]
        private quaternion rotation;
        
        [BsonIgnore]
        public quaternion Rotation
        {
            get => this.rotation;
            set
            {
                this.rotation = value;
                EventSystem.Instance.Publish(this.Scene(), new ChangeRotation() { Unit = this });
            }
        }

        protected override string ViewName
        {
            get
            {
                return $"{this.GetType().FullName} ({this.Id})";
            }
        }
    }
}