using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class LoopItem : ABLItem
    {

        [SerializeField]
        private LoopType Type;

        public int NbItems { get { return mNbItems; }
        set {
                mNbItems = value;
                SetLoopSize(mNbItems);
            }

        }
        private int mNbItems = 1;

        public override BLItemSerializable GetItem()
        {
            BLItemSerializable lItem = new BLItemSerializable();
            lItem.Category = Category.LOOP;
            lItem.Index = Index;
            lItem.Parameter = Parameter;
            lItem.LoopType = Type;
            lItem.NbItemsInLoop = mNbItems;
            return lItem;
        }

        public void AddItem()
        {
            mNbItems++;
            SetLoopSize(mNbItems);
        }

        public void RemoveItem()
        {
            if (mNbItems > 1)
            {
                mNbItems--;
                SetLoopSize(mNbItems);
            }
        }

        public void SetLoopSize(int iNb)
        {
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2( 345 + (iNb-1) * 105, 200);
            transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-56 - (iNb-1) * 52, 6.7F);
        }
    }
}