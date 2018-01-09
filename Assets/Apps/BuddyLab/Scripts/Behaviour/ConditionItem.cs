using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class ConditionItem : ABLItem
    {
        [SerializeField]
        private string Name;

        public override BLItemSerializable GetItem()
        {
            BLItemSerializable lItem = new BLItemSerializable();
            lItem.Category = Category.CONDITION;
            lItem.Index = Index;
            lItem.Parameter = Parameter;
            lItem.ConditionName = Name;
            return lItem;
        }


    }
}