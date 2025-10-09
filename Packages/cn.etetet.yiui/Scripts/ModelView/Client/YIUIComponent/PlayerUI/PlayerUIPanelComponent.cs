using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    public struct FishingFlag
    {
        public bool flag;
    }

    public struct GetResource
    {
        public long UnitId;
        public int TagType; // 1 为正常 2 为资源 3 为能量
    }
    
    public struct OutResource
    {
        public long UnitId;
    }
    
    /// <summary>
    /// Author  YIUI
    /// Date    2025.10.3
    /// Desc
    /// </summary>
    public partial class PlayerUIPanelComponent : IDynamicEvent<FishingFlag>, 
            IDynamicEvent<PlayerUIDateRefresh> ,IDynamicEvent<GetResource> ,IDynamicEvent<OutResource>,
            IDynamicEvent<EventThrowingCheckEnergy>
    {
        //移动半径
        public float raidus = 10.0f;
        //初始位置
        public Vector2 originPos = Vector2.zero;
        //摇杆方向
        public Vector2 moveDir = Vector2.zero;
        //最后移动的方向
        public Vector2 LastDir = Vector2.zero;
        //冷却时间
        public float coolTime = 0.0f;

        public bool isUpdate = false;

        public long joyMoveTimerId = 0;
        
        public EntityRef<Unit> unit;

        public bool FishingFlag;

        public long ResourceId;
    }
}
