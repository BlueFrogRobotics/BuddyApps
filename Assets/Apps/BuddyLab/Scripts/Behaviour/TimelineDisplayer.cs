﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

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
        private ListBLI mListBLI;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisplaySequence(string iFileName)
        {
            mSequence = Instantiate(dropLine, placeholderLine.transform);
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            mSequence.transform.parent = placeholderLine.transform;
            //mSequence.transform.localPosition = new Vector3(0, 0, 0);
            mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
            Canvas.ForceUpdateCanvases();
            string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("Projects/" + iFileName);
            mListBLI = Utils.UnserializeXML<ListBLI>(lDirectoryPath);
            Debug.Log("elements dans la liste: "+mListBLI.List.Count);
            //HighlightElement(2);
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
                    //if(items[i].gameObject.GetComponentInChildren<DragAndDropItem>().LoopItem!=null)
                    //{
                    //    Debug.Log("DES CHIIIIIIPS!");
                    //    items[i].gameObject.GetComponentInChildren<DragAndDropItem>().LoopItem.gameObject.GetComponentInParent<DragAndDropCell>().GetComponent<CanvasGroup>().alpha = 1F;
                    //}
                }
                else if (i< mListBLI.List.Count && mListBLI.List[i].Category==Category.LOOP && i - mListBLI.List[i].NbItemsInLoop <= iNum && iNum < i)
                {
                    Debug.Log("le nombre d items: " + mListBLI.List[i].NbItemsInLoop);
                    items[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
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