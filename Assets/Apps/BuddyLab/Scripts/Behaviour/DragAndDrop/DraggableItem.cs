using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    public sealed class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool IsDragged { get; private set; }
        public Transform ParentToReturnTo { get; set; }
        public Transform PlaceholderParent { get; set; }

        public bool OnlyDroppable { get { return onlyDroppable; } set { onlyDroppable = value; } }

        [SerializeField]
        private bool onlyDroppable = false;

        private GameObject mPlaceholder = null;

        private GameObject mItem = null;

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragged = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            mPlaceholder = new GameObject();
            LayoutElement le = mPlaceholder.AddComponent<LayoutElement>();
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

            ParentToReturnTo = this.transform.parent;
            PlaceholderParent = ParentToReturnTo;

            if (!ParentToReturnTo.GetComponent<ItemsContainer>().DragOnly)
            {
                mPlaceholder.transform.SetParent(this.transform.parent);
                mPlaceholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
            }

            if(OnlyDroppable)
            {
                mItem = Instantiate(gameObject);
                mItem.transform.SetParent(this.transform.root);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                this.transform.SetParent(this.transform.root);
                mItem = gameObject;
            }

        }

        public void OnDrag(PointerEventData eventData)
        {
            mItem.transform.position = eventData.position;

            if (PlaceholderParent != null && PlaceholderParent.GetComponent<ItemsContainer>() != null && !PlaceholderParent.GetComponent<ItemsContainer>().DragOnly)
            {
                if (mPlaceholder.transform.parent != PlaceholderParent)
                    mPlaceholder.transform.SetParent(PlaceholderParent);

                int newSiblingIndex = PlaceholderParent.childCount;

                for (int i = 0; i < PlaceholderParent.childCount; i++)
                {
                    if (mItem.transform.position.x < PlaceholderParent.GetChild(i).position.x)
                    {

                        newSiblingIndex = i;

                        if (mPlaceholder.transform.GetSiblingIndex() < newSiblingIndex)
                            newSiblingIndex--;

                        break;
                    }
                }

                if (newSiblingIndex < 1)
                    newSiblingIndex = 1;

                mPlaceholder.transform.SetSiblingIndex(newSiblingIndex);
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            int lIndex = mPlaceholder.transform.GetSiblingIndex();

            Destroy(mPlaceholder);
            if(OnlyDroppable)
            {
                Destroy(mItem);
                if(!PlaceholderParent.GetComponent<ItemsContainer>().DragOnly)
                {
                    PlaceholderParent.GetComponent<ItemsContainer>().AddItem(this, lIndex);
                }
            }
            else
            {
                mItem.transform.SetParent(PlaceholderParent);
                mItem.transform.SetSiblingIndex(mPlaceholder.transform.GetSiblingIndex());
                mItem.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            IsDragged = false;
            PlaceholderParent.GetComponent<ItemsContainer>().EndDrag();
        }

    }
}