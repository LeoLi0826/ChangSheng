using System.Collections.Generic;

namespace ET.Client
{
    [EntitySystemOf(typeof(ForgeComponent))]
    [FriendOfAttribute(typeof(ET.Client.ForgeComponent))]
    public static partial class ForgeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.ForgeComponent self)
        {

        }
        [EntitySystem]
        private static void Destroy(this ET.Client.ForgeComponent self)
        {

        }

        public static void AddItemElement(this ET.Client.ForgeComponent self, Dictionary<ItemElementAttr,int> attrs)
        {
            foreach (var attr in attrs)
            {
                self.AddItemElement(attr.Key,attr.Value);
            }
        }

        public static void AddItemElement(this ET.Client.ForgeComponent self, ItemElementAttr attr, int value)
        {
            if (self.Element.TryGetValue(attr,out int old))
            {
                self.Element[attr] = old + value;
            }
            else
            {
                self.Element.Add(attr,value);
            }
        }
    }
}

