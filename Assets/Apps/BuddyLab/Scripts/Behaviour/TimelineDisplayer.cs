using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{
    public class TimelineDisplayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject dropLine;

        [SerializeField]
        private GameObject topLine;

        [SerializeField]
        private GameObject placeholderLine;

        private GameObject mSequence;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisplaySequence()
        {
            mSequence = Instantiate(dropLine, placeholderLine.transform);
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            mSequence.transform.parent = placeholderLine.transform;
            //mSequence.transform.localPosition = new Vector3(0, 0, 0);
            mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
            Canvas.ForceUpdateCanvases();
            HighlightElement(2);
        }

        public void HideSequence()
        {
            Destroy(mSequence);
            mSequence = null;
            placeholderLine.GetComponent<Animator>().SetTrigger("close");
        }

        public void HighlightElement(int iNum)
        {
            if(iNum>=0)
                mSequence.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.25F;
            else
                mSequence.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1F;
            DragAndDropCell[] items = mSequence.GetComponentsInChildren<DragAndDropCell>();
            for(int i=0; i<items.Length; i++)
            {
                if(i==iNum)
                {
                    mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-1* items[i].transform.localPosition.x, 80, 0);
                    items[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
                    items[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    items[i].gameObject.GetComponent<CanvasGroup>().alpha = 0.25F;
                    items[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}