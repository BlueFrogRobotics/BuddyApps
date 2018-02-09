using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuddyApp.BuddyLab
{
    public class ItemsContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool DropOnly=false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            eventData.useDragThreshold = false;

            if (eventData.pointerDrag == null)
                return;

            DraggableItem d = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (d != null && !DropOnly)
            {
                d.placeholderParent = this.transform;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("OnPointerExit");
            if (eventData.pointerDrag == null)
                return;

            DraggableItem d = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (d != null && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            //DraggableItem d = eventData.pointerDrag.GetComponent<DraggableItem>();
            //if (d != null)
            //{
            //    d.parentToReturnTo = this.transform;
            //    if(d.OnlyDroppable)
            //    {
            //        Debug.Log("clone item droppe");
            //        GameObject lItem = Instantiate(d.gameObject);
            //        lItem.transform.SetParent(this.transform);
            //    }
            //    //if (!DropOnly)
            //        //d.OnlyDroppable = false;
            //}

        }

        public void AddItem(DraggableItem iItem, int iIndex)
        {
            if (iItem != null)
            {
                iItem.parentToReturnTo = this.transform;
                if (iItem.OnlyDroppable)
                {
                    Debug.Log("clone item droppe");
                    GameObject lItem = Instantiate(iItem.gameObject);
                    lItem.transform.SetParent(this.transform);
                    lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
                    lItem.transform.SetSiblingIndex(iIndex);
                }
                //if (!DropOnly)
                //d.OnlyDroppable = false;
            }
        }

    }
}