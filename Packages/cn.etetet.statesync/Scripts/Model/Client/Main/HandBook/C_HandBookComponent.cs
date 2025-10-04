using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf]
    [FriendOfAttribute(typeof(Item))]
    public class C_HandBookComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long, EntityRef<Item>> ItemDict = new Dictionary<long,  EntityRef<Item>>(); 

        public MultiMap<int, EntityRef<Item>> ItemMap = new MultiMap<int, EntityRef<Item>>();

        
    }
}