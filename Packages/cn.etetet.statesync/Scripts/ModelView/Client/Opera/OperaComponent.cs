using System;

using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(Scene))]
	public class OperaComponent: Entity, IAwake, IUpdate,ILateUpdate
    {
        public Vector3 ClickPoint;

	    public int mapMask;
	    
	    public EntityRef<Unit> MyUnit;
    }
}
