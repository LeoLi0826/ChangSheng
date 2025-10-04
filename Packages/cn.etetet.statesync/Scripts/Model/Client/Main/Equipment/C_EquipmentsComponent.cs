using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [ComponentOf]
    
    public class C_EquipmentsComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<int, EntityRef<Item>> EquipItems = new Dictionary<int, EntityRef<Item>>();
        
    }
}