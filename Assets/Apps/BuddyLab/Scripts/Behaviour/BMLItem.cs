using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    

    public class BMLItem : ABLItem
    {
        [SerializeField]
        private string BML;

        public override BLItemSerializable GetItem()
        {
            BLItemSerializable lItem = new BLItemSerializable();
            lItem.Category = Category.BML;
            lItem.Index = Index;
            lItem.ParameterKey = ParameterKey;
            lItem.Parameter = Parameter;
            lItem.BML = BML;
            return lItem;
        }

    }
}