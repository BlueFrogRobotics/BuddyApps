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
            mItemsKeys.Clear();
            mSequence = Instantiate(dropLine, placeholderLine.transform);
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            mSequence.transform.parent = placeholderLine.transform;
            mSequence.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
            Canvas.ForceUpdateCanvases();

            string lDirectoryPath = Buddy.Platform.Application.PersistentDataPath + "Projects" + iFileName;

        }

        public void HideSequence()
        {
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
            Debug.Log("display algo");
            placeholderLine.GetComponent<Animator>().SetTrigger("open");
            displayDropLine.transform.parent = placeholderLine.transform;
            displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-400, 80, 0);
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
            GameObject lItem = mItemsKeys[iInstruction].gameObject;
            while (lItem.GetComponent<AGraphicElement>().GetInstruction(false).Parent!=null)
            {
                lItem = mItemsKeys[lItem.GetComponent<AGraphicElement>().GetInstruction(false).Parent].gameObject;
                lItem.GetComponent<CanvasGroup>().alpha = 1;
                lPosition -= lItem.GetComponent<RectTransform>().localPosition.x;
            }
            Vector3 itemRelative = displayDropLine.transform.InverseTransformPoint(mItemsKeys[iInstruction].transform.position);
            displayDropLine.GetComponent<RectTransform>().localPosition = new Vector3(-1*itemRelative.x, 80, 0);
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