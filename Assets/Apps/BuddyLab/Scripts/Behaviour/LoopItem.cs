using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public class LoopItem : ABLItem
    {

        [SerializeField]
        private LoopType Type;

        private GameObject mBorderLoop=null;

        private bool mIsInitialised = false;



        public int NbItems { get { return mNbItems; }
        set {
                mNbItems = value;
                SetLoopSize(mNbItems);
            }

        }
        private int mNbItems = 1;

        private void Update()
        {
            if(mBorderLoop!=null && transform.GetSiblingIndex() - mBorderLoop.transform.GetSiblingIndex()-1!=NbItems)
            {
                if (GetComponent<DraggableItem>() != null && !GetComponent<DraggableItem>().IsDragged && mIsInitialised)
                {
                    NbItems = transform.GetSiblingIndex() - mBorderLoop.transform.GetSiblingIndex() - 1;
                    if(NbItems<=0)
                    {
                        Destroy(mBorderLoop);
                        Destroy(gameObject);
                    }
                }
            }
        }

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
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2( 350 + (iNb-1) * 105, 200);
            transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-66 - (iNb-1) * 52, 6.7F);
            if(GetComponent<DraggableItem>()!=null)
            {
                GetComponent<DraggableItem>().NbItemsAssociated = -iNb - 2;
            }
        }

        public void InitLoop(Transform iParent, int iNbItems=1)
        {
            mBorderLoop = new GameObject("endofloop");
            LayoutElement le = mBorderLoop.AddComponent<LayoutElement>();
            le.minWidth = 27;
            mBorderLoop.transform.SetParent(iParent);
            mBorderLoop.transform.SetSiblingIndex(transform.GetSiblingIndex() - NbItems);
            SetLoopSize(mNbItems);
            mIsInitialised = true;
            //NbItems = iNbItems;
        }

        public void DeleteBorder()
        {
            Destroy(mBorderLoop);
            mBorderLoop = null;
        }
    }
}