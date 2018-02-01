using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Every "drag and drop" item must contain this script
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        static public DragAndDropItem draggedItem;                                      // Item that is dragged now
        static public GameObject icon;                                                  // Icon of dragged item
        static public DragAndDropCell sourceCell;                                       // From this cell dragged item is
        public int id = 0;

        public delegate void DragEvent(DragAndDropItem item);
        static public event DragEvent OnItemDragStartEvent;                             // Drag start event
        static public event DragEvent OnItemDragEndEvent;                               // Drag end event

        [SerializeField]
        private Image[] arrayImages;

        public LoopItem LoopItem=null;
        public GameObject[] draggedObjects;
        private GameObject[] draggedIcons;

        //[SerializeField]
        //private Text text

        /// <summary>
        /// This item is dragged
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            sourceCell = GetComponentInParent<DragAndDropCell>();                       // Remember source cell
            draggedItem = this;                                                         // Set as dragged item
            //icon = new GameObject("Icon");                                              // Create object for item's icon
            //Image image = icon.AddComponent<Image>();
            //image.sprite = GetComponent<Image>().sprite;

            icon = Instantiate(gameObject);
            foreach (Image imageitem in icon.GetComponent<DragAndDropItem>().arrayImages)
            {
                imageitem.raycastTarget = false;
            }

            if(draggedObjects!=null && draggedObjects.Length>0)
            {
                draggedIcons = new GameObject[draggedObjects.Length];
                for(int i=0; i< draggedObjects.Length; i++)
                {
                    //draggedIcons[i] = Instantiate(draggedObjects[i], draggedObjects[i].transform.position- icon.transform.position, draggedObjects[i].transform.rotation);
                    draggedIcons[i] = Instantiate(draggedObjects[i], icon.transform, true);
                    draggedIcons[i].transform.position = new Vector3(icon.transform.position.x-105-(i) * 105, icon.transform.position.y);//draggedObjects[i].transform.position-new Vector3(50, 200);
                    foreach (Image imageitem in draggedIcons[i].GetComponent<DragAndDropItem>().arrayImages)
                    {
                        imageitem.raycastTarget = false;
                    }
                }

            }

            //draggedItem = this;                                                         // Set as dragged item
            //icon = new GameObject("Icon2");                                              // Create object for item's icon
            //Image image2 = icon.AddComponent<Image>();
            //image2.sprite = sourceCell.GetComponent<Image>().sprite;

            //icon2.transform.parent = icon.transform;

            //image.raycastTarget = false;                                                // Disable icon's raycast for correct drop handling
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            // Set icon's dimensions
            iconRect.sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                                                GetComponent<RectTransform>().sizeDelta.y);
            Canvas canvas = GetComponentInParent<Canvas>();                             // Get parent canvas
            if (canvas != null)
            {
                // Display on top of all GUI (in parent canvas)
                icon.transform.SetParent(canvas.transform, true);                       // Set canvas as parent
                icon.transform.SetAsLastSibling();                                      // Set as last child in canvas transform

                //if(draggedIcons!=null && draggedIcons.Length>0)
                //{
                //    foreach(GameObject objet in draggedIcons)
                //    {
                //        objet.transform.SetParent(canvas.transform, true);
                //        objet.transform.SetAsLastSibling();
                //    }
                //}
            }
            if (OnItemDragStartEvent != null)
            {
                OnItemDragStartEvent(this);                                             // Notify all about item drag start
            }

        }

        /// <summary>
        /// Every frame on this item drag
        /// </summary>
        /// <param name="data"></param>
        public void OnDrag(PointerEventData data)
        {
            if (icon != null)
            {
                Vector3 lDiffPosition= Input.mousePosition - icon.transform.position;
                icon.transform.position = Input.mousePosition;                          // Item's icon follows to cursor

                //if (draggedIcons != null && draggedIcons.Length > 0)
                //{
                //    foreach (GameObject objet in draggedIcons)
                //    {
                //        objet.transform.position += lDiffPosition;
                //    }
                //}
            }
        }

        /// <summary>
        /// This item is dropped
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (icon != null)
            {
                Destroy(icon);                                                          // Destroy icon on item drop

                if (draggedIcons != null && draggedIcons.Length > 0)
                {
                    foreach (GameObject objet in draggedIcons)
                    {
                        Destroy(objet);
                    }
                }
            }
            MakeVisible(true);                                                          // Make item visible in cell
            if (OnItemDragEndEvent != null)
            {
                OnItemDragEndEvent(this);                                               // Notify all cells about item drag end
            }
            draggedItem = null;
            icon = null;
            sourceCell = null;

            draggedIcons = null;
        }

        /// <summary>
        /// Enable item's raycast
        /// </summary>
        /// <param name="condition"> true - enable, false - disable </param>
        public void MakeRaycast(bool condition)
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = condition;
            }
        }

        /// <summary>
        /// Enable item's visibility
        /// </summary>
        /// <param name="condition"> true - enable, false - disable </param>
        public void MakeVisible(bool condition)
        {
            GetComponent<Image>().enabled = condition;
        }
    }
}