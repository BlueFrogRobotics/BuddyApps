using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool IsDragged { get; private set; }
        public Transform parentToReturnTo = null;
        public Transform placeholderParent = null;
        public bool OnlyDroppable = false;
        public int NbItemsAssociated = 0;

        GameObject placeholder = null;

        GameObject mItem = null;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            IsDragged = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            placeholder = new GameObject();
            LayoutElement le = placeholder.AddComponent<LayoutElement>();
            if (this.GetComponent<LayoutElement>() != null)
            {
                le.minWidth = this.GetComponent<LayoutElement>().minWidth;
                le.minHeight = this.GetComponent<LayoutElement>().minHeight;
            }
            else
            {
                le.minWidth = this.GetComponent<RectTransform>().rect.width;
                le.minHeight = this.GetComponent<RectTransform>().rect.height;
            }
            //le.flexibleWidth = 0;
            //le.flexibleHeight = 0;

            parentToReturnTo = this.transform.parent;
            placeholderParent = parentToReturnTo;

            if (!parentToReturnTo.GetComponent<ItemsContainer>().DropOnly)
            {
                placeholder.transform.SetParent(this.transform.parent);
                placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
            }
            //else
            //{
            //    placeholder.transform.SetParent(this.transform.root);
            //}

            if(OnlyDroppable)
            {
                mItem = Instantiate(gameObject);
                mItem.transform.SetParent(this.transform.root);//.parent.parent);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                AssociateItems();
                this.transform.SetParent(this.transform.root);//.parent.parent);
                mItem = gameObject;
            }

        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log ("OnDrag");

            mItem.transform.position = eventData.position;

            if (placeholderParent != null && placeholderParent.GetComponent<ItemsContainer>() != null && !placeholderParent.GetComponent<ItemsContainer>().DropOnly)
            {
                if (placeholder.transform.parent != placeholderParent)
                    placeholder.transform.SetParent(placeholderParent);

                int newSiblingIndex = placeholderParent.childCount;

                for (int i = 0; i < placeholderParent.childCount; i++)
                {
                    if (mItem.transform.position.x < placeholderParent.GetChild(i).position.x)
                    {

                        newSiblingIndex = i;

                        if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                            newSiblingIndex--;

                        break;
                    }
                }

                if (newSiblingIndex < 1)
                    newSiblingIndex = 1;

                if(GetComponent<LoopItem>()!=null)
                    newSiblingIndex = MoveIndexOutsideLoop(placeholderParent, newSiblingIndex);

                placeholder.transform.SetSiblingIndex(newSiblingIndex);
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            //mItem.transform.SetParent(parentToReturnTo);
            mItem.transform.SetParent(placeholderParent);
            int lIndex = placeholder.transform.GetSiblingIndex();
            mItem.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            mItem.GetComponent<CanvasGroup>().blocksRaycasts = true;

            Destroy(placeholder);
            if(OnlyDroppable)
            {
                Destroy(mItem);
                if(!placeholderParent.GetComponent<ItemsContainer>().DropOnly)
                {
                    placeholderParent.GetComponent<ItemsContainer>().AddItem(this, lIndex);
                }
            }
            else
            {
                DissociateItems();
            }
            IsDragged = false;
        }

        public void AssociateItems()
        {
            if(GetComponent<LoopItem>()!=null)
            {
                GetComponent<LoopItem>().DeleteBorder();
            }
            if (NbItemsAssociated > 0)
            {
                for (int i = 0; i < NbItemsAssociated; i++)
                {
                    transform.parent.GetChild(transform.GetSiblingIndex() + 1).SetParent(this.transform);
                }
            }
            else if(NbItemsAssociated<0)
            {
                for (int i = 0; i < NbItemsAssociated*-1; i++)
                {
                    transform.parent.GetChild(transform.GetSiblingIndex() - 1).SetParent(this.transform);
                }
            }
        }

        public void DissociateItems()
        {
            if (mItem.GetComponentsInChildren<DraggableItem>() != null && mItem.GetComponentsInChildren<DraggableItem>().Length > 0)
            {
                int lIndex = mItem.transform.GetSiblingIndex();
                int lNbChildren = mItem.GetComponentsInChildren<DraggableItem>().Length;
                Debug.Log("nb children: " + lNbChildren);
                foreach (DraggableItem lItem in mItem.GetComponentsInChildren<DraggableItem>())
                {
                    if (NbItemsAssociated > 0)
                    {
                        lItem.transform.SetParent(mItem.transform.parent);
                        lItem.transform.SetSiblingIndex(lIndex + 1);
                        lIndex++;
                    }
                    else
                    {
                        lItem.transform.SetParent(mItem.transform.parent);
                        lItem.transform.SetSiblingIndex(lIndex);
                    }
                }
                if (GetComponent<LoopItem>() != null)
                {
                    GetComponent<LoopItem>().InitLoop(mItem.transform.parent, lNbChildren);
                }
            }
        }

        private int MoveIndexOutsideLoop(Transform iParent, int iIndex)
        {
            if (iParent.GetComponentsInChildren<DraggableItem>() != null && iParent.GetComponentsInChildren<DraggableItem>().Length > 0)
            {
                int lIndex = iIndex;
                for(int i=iIndex; i<iParent.childCount; i++)
                {
                    if(iParent.GetChild(i).GetComponent<LoopItem>()!=null && iIndex> i-iParent.GetChild(i).GetComponent<LoopItem>().NbItems-1 && iIndex<=i)
                    {
                        if(OnlyDroppable)
                            lIndex = i + 2;
                        else
                            lIndex = i + 1;
                    }
                }
                return lIndex;
            }
            else
                return iIndex;
        }

    }
}