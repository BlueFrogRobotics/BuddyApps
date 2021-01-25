using UnityEngine;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Class that manages the DraggableItems that it contains
    /// </summary>
    public sealed class ItemsContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// If set to true items can't be put in the container
        /// </summary>
        public bool DragOnly=false;

        //public delegate void Modification();
        //public event Modification OnModification;

        public void OnPointerEnter(PointerEventData iEventData)
        {
            iEventData.useDragThreshold = false; 

            if (iEventData.pointerDrag == null)
                return;

            DraggableItem lItem = iEventData.pointerDrag.GetComponent<DraggableItem>();
            if (lItem != null && !DragOnly)
            {
                lItem.PlaceholderParent = this.transform;
            }
        }

        public void OnPointerExit(PointerEventData iEventData)
        {
            if (iEventData.pointerDrag == null)
                return;

            DraggableItem lItem = iEventData.pointerDrag.GetComponent<DraggableItem>();
            if (lItem != null && lItem.PlaceholderParent == this.transform)
            {
                lItem.PlaceholderParent = lItem.ParentToReturnTo;
            }
        }

        public void OnDrop(PointerEventData iEventData)
        {
            Debug.Log(iEventData.pointerDrag.name + " was dropped on " + gameObject.name);

        }

        public void AddItem(DraggableItem iItem, int iIndex)
        {
            if (iItem != null) {
                iItem.ParentToReturnTo = this.transform;
                if (iItem.OnlyDroppable) {
                    GameObject lItem = Instantiate(iItem.gameObject);
                    lItem.transform.SetParent(this.transform);
                    lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
                    lItem.transform.SetSiblingIndex(iIndex);
                    ItemsContainer lItemContainer = lItem.GetComponentInChildren<ItemsContainer>();
                    if(lItemContainer!=null) {
                        lItemContainer.DragOnly = false;
                    }
                }

            }
        }


        public void EndDrag()
        {
            ItemControlUnit.EndModif();
            Debug.Log("end drag");
            //if (OnModification != null)
            //    OnModification();
            //else if(GetComponentInParent<ItemsContainer>()!=null) {
            //    GetComponentInParent<ItemsContainer>().EndDrag();
            //}
        }

    }
}