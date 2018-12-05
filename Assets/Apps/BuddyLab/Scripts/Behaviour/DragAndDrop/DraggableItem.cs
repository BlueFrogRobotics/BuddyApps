using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Represents a draggable item
    /// </summary>
    public sealed class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private bool onlyDroppable = false;

        private GameObject mPlaceholder = null;
        private GameObject mItem = null;

        /// <summary>
        /// True if the item is being dragged
        /// </summary>
        public bool IsDragged { get; private set; }

        /// <summary>
        /// The initial item container
        /// </summary>
        public Transform ParentToReturnTo { get; set; }

        /// <summary>
        /// The target container. If the container is not DragOnly the item will be put there
        /// </summary>
        public Transform PlaceholderParent { get; set; }

        /// <summary>
        /// If set to true the dragged item will be cloned and put in the target container.
        /// Otherwise it will be removed from previous container
        /// </summary>
        public bool OnlyDroppable { get { return onlyDroppable; } set { onlyDroppable = value; } }
   

        public void OnBeginDrag(PointerEventData iEventData)
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

        public void OnDrag(PointerEventData iEventData)
        {
            mItem.transform.position = iEventData.position;

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

        public void OnEndDrag(PointerEventData iEventData)
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