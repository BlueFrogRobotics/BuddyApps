using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public sealed class SpecialItem : ABLItem
    {
        
        public override BLItemSerializable GetItem()
        {
            
            BLItemSerializable lItem = new BLItemSerializable();
            lItem.Category = Category.SPECIAL;
            lItem.Index = Index;
            lItem.ParameterKey = ParameterKey;
            lItem.Parameter = Parameter;
            return lItem;
        }
    }

}
