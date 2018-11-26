using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class TimelineDisplayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject dropLine;

        [SerializeField]
        private GameObject topLine;

        [SerializeField]
        private GameObject placeholderLine;

        [SerializeField]
        private GameObject displayDropLine;

        [SerializeField]
        private ItemControlUnit itemControlUnit;

        [SerializeField]
        private ItemManager itemManager;

        private GameObject mSequence;
        private ListBLI mListBLI;

        private Dictionary<ABehaviourInstruction, AGraphicElement> mItemsKeys;

        // Use this for initialization
        void Start()
        {
            mItemsKeys = new Dictionary<ABehaviourInstruction, AGraphicElement>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void DisplaySequence(string iFileName)
        {
            Debug.Log("display sequence");
            mItemsKeys.Clear();
            mSequence = Instantiate(dropLine, placeholderLine.transform);
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            mSequence.transform.parent = placeholderLine.transform;
            //mSequence.transform.localPosition = new Vector3(0, 0, 0);
            mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
            if (mSequence == null)
            {
                Debug.Log("sequence nul avant update!");
            }
            Canvas.ForceUpdateCanvases();
            if (mSequence == null)
            {
                Debug.Log("sequence nul apres update!");
            }
            Debug.Log("avant path");
            string lDirectoryPath = Buddy.Resources.GetRawFullPath("Projects/" + iFileName);
            Debug.Log("apres path: "+ lDirectoryPath);
            mListBLI = Utils.UnserializeXML<ListBLI>(lDirectoryPath);
            Debug.Log("elements dans la liste: "+mListBLI.List.Count);
            HighlightElement(0);
            Debug.Log("highlight element 2");
        }

        public void HideSequence()
        {
            Debug.Log("hide sequence!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //Destroy(mSequence);
            //mSequence = null;

            foreach (Transform child in displayDropLine.transform)
            {
                if (child != null && child.GetComponent<AGraphicElement>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            placeholderLine.GetComponent<Animator>().SetTrigger("close");
        }

        public void DisplayAlgo()
        {
            //mSequence = Instantiate(dropLine, placeholderLine.transform);
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            displayDropLine.transform.parent = placeholderLine.transform;
            displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
            //Debug.Log("position display: " + displayDropLine.GetComponent<RectTransform>().position.x);
            OpenProjectVisitor lVisitor = new OpenProjectVisitor(itemManager, displayDropLine.transform);
            lVisitor.Visit(itemControlUnit.BehaviourAlgorithm);
            foreach(AGraphicElement element in displayDropLine.GetComponentsInChildren<AGraphicElement>())
            {
                mItemsKeys.Add(element.GetInstruction(true), element);
            }

            mItemsKeys.Values.ToList();
        }

        public void OnExecuteInstruction(ABehaviourInstruction iInstruction)
        {
            Debug.Log("INSTRUCTION: "+iInstruction.GetType().BaseType.ToString());
            float lPosition = -1*mItemsKeys[iInstruction].transform.localPosition.x;
            foreach (AGraphicElement element in displayDropLine.GetComponentsInChildren<AGraphicElement>())
            {
                element.gameObject.GetComponent<CanvasGroup>().alpha = 0.25F;
            }
            mItemsKeys[iInstruction].gameObject.GetComponent<CanvasGroup>().alpha = 1;
            //Debug.Log("machin chose1");
            GameObject lItem = mItemsKeys[iInstruction].gameObject;
            //Debug.Log("position display: " + lItem.GetComponent<RectTransform>().position.x);
            //Debug.Log("machin chose2");
            while (lItem.GetComponent<AGraphicElement>().GetInstruction(false).Parent!=null)
            {
                //Debug.Log("machin chose3");
                lItem = mItemsKeys[lItem.GetComponent<AGraphicElement>().GetInstruction(false).Parent].gameObject;
                lItem.GetComponent<CanvasGroup>().alpha = 1;
                lPosition -= lItem.GetComponent<RectTransform>().localPosition.x;
                //displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-1 * displayDropLine.GetComponent<RectTransform>().localPosition.x + lItem.GetComponent<RectTransform>().localPosition.x, 80, 0);
            }
            //displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-1 * mItemsKeys[iInstruction].transform.localPosition.x, 80, 0);
            Vector3 itemRelative = displayDropLine.transform.InverseTransformPoint(mItemsKeys[iInstruction].transform.position);
            displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-1*itemRelative.x, 80, 0);
            //displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(lPosition, 80, 0);
            //mItemsKeys[iInstruction].gameObject.SetActive(false);
        }

        public void HighlightElement(int iNum)
        {

            //Debug.Log("miaou 1");
            if (mSequence == null)
            {
                Debug.Log("sequence nul!");
            }
            else
            {
                if (iNum >= 0)
                    mSequence.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.25F;
                else
                    mSequence.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1F;
                //Debug.Log("miaou 2");
                DraggableItem[] items = mSequence.GetComponentsInChildren<DraggableItem>();
                //Debug.Log("miaou 3");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i == iNum)
                    {
                        mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-1 * items[i].transform.localPosition.x, 80, 0);
                        items[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
                        items[i].transform.GetChild(1).gameObject.SetActive(true);
                        //if(items[i].gameObject.GetComponentInChildren<DragAndDropItem>().LoopItem!=null)
                        //{
                        //    Debug.Log("DES CHIIIIIIPS!");
                        //    items[i].gameObject.GetComponentInChildren<DragAndDropItem>().LoopItem.gameObject.GetComponentInParent<DragAndDropCell>().GetComponent<CanvasGroup>().alpha = 1F;
                        //}
                    }
                    else if (i < mListBLI.List.Count && mListBLI.List[i].Category == Category.LOOP && i - mListBLI.List[i].NbItemsInLoop <= iNum && iNum < i)
                    {
                        Debug.Log("le nombre d items: " + mListBLI.List[i].NbItemsInLoop);
                        items[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
                    }
                    else
                    {
                        items[i].gameObject.GetComponent<CanvasGroup>().alpha = 0.25F;
                        items[i].transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }

        public void EnableTimeline(bool iParam)
        {
            if(iParam)
                placeholderLine.GetComponent<Animator>().SetTrigger("open");
            else
                placeholderLine.GetComponent<Animator>().SetTrigger("close");
        }
    }
}