using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    public sealed class ItemsContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool DropOnly=false;
        public delegate void Modification();
        public event Modification OnModification;

        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;

            if (eventData.pointerDrag == null)
                return;

            DraggableItem d = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (d != null && !DropOnly)
            {
                d.PlaceholderParent = this.transform;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            DraggableItem d = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (d != null && d.PlaceholderParent == this.transform)
            {
                d.PlaceholderParent = d.ParentToReturnTo;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

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

                }

            }
        }


        public void EndDrag()
        {
            if (OnModification != null)
                OnModification();
        }

    }
}