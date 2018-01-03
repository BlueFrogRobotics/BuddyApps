using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class LoopItem : ABLItem
    {

        [SerializeField]
        private LoopType Type;

        public override BLItemSerializable GetItem()
        {
            BLItemSerializable lItem = new BLItemSerializable();
            lItem.Category = Category.LOOP;
            lItem.Index = Index;
            lItem.Parameter = Parameter;
            lItem.LoopType = Type;
            return lItem;
        }
    }
}