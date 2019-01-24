using BlueQuark;

using UnityEngine;

using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace BuddyApp.BuddyLab
{ 

    /// <summary>
    /// Example of control unit for drag and drop events handle
    /// </summary>
    public sealed class ItemControlUnit : MonoBehaviour
    {

        public delegate void Modification();
        public static event Modification OnModification;

        public BehaviourAlgorithm BehaviourAlgorithm { get; set; }
        public int Index { set { mIndex = value; } }

        [SerializeField]
        private ItemManager itemManager;

        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private ItemsContainer sequenceContainer;

        [SerializeField]
        private ItemsContainer trashZoneContainer;

        private List<GameObject> mArrayItems;

        private BlueQuark.Behaviour mBMLManager;

        private string mDirectoryPath = "";

        private string mConditionParam;

        private int mLoopCounter;
        private int mIndex;

        private int mNbModifs = 0;

        private LinkedList<BehaviourAlgorithm> mStackUndoBli;
        private Stack<BehaviourAlgorithm> mStackRedoBli;

        private bool mIsUndoing = false;



        void Start()
        {
            mLoopCounter = 1;
            mIndex = 0;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            mArrayItems = new List<GameObject>();
            //sequenceContainer.OnModification += SaveModification;
            //trashZoneContainer.OnModification += SaveModification;
            mStackUndoBli = new LinkedList<BehaviourAlgorithm>();
            mStackRedoBli = new Stack<BehaviourAlgorithm>();
            BehaviourAlgorithm = new BehaviourAlgorithm();
            OnModification += SaveModification;
        }

        private void OnDisable()
        {
            OnModification -= SaveModification;
        }

        public void ShowAlgo(string iFileName)
        {
            Debug.Log("show algo 1");
            FillBehaviourAlgorithm(iFileName);

            OpenProjectVisitor lVisitor = new OpenProjectVisitor(itemManager, panel.transform);
            lVisitor.Visit(BehaviourAlgorithm);
        }

        public void ShowAlgo(BehaviourAlgorithm iBehaviourAlgorithm)
        {
            Debug.Log("show algo 2");
            OpenProjectVisitor lVisitor = new OpenProjectVisitor(itemManager, panel.transform);
            lVisitor.Visit(iBehaviourAlgorithm);
            //mStackUndoBli.AddLast(iBehaviourAlgorithm);
        }

        public void FillBehaviourAlgorithm(string iFileName)
        {
            mDirectoryPath = Buddy.Resources.GetRawFullPath("Projects" + "/" + iFileName);
            Debug.Log("le full path: " + mDirectoryPath);
            BehaviourAlgorithm.Instructions.Clear();
            BehaviourAlgorithm lAlgo = null;
            try {
                lAlgo = Utils.UnserializeXML<BehaviourAlgorithm>(mDirectoryPath);
            }
            catch(Exception e) {
                Debug.Log("probleme: " + e.ToString());
            }
            if (lAlgo != null)
            {
                BehaviourAlgorithm = lAlgo;
            }
        }

        

        public void CleanSequence()
        {
            foreach(Transform child in panel.transform)
            {
                if (child != null && child.GetComponent<AGraphicElement>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void SaveSequence()
        {
            SaveAlgorithm(mDirectoryPath);
        }


        public void SaveAlgorithm(string iPath)
        {
            BehaviourAlgorithm.Instructions.Clear();
            foreach (Transform child in panel.transform)
            {
                if (child != null && child.GetComponent<AGraphicElement>() != null)
                    BehaviourAlgorithm.Instructions.Add(child.GetComponent<AGraphicElement>().GetInstruction(true));
            }
            Utils.SerializeXML<BehaviourAlgorithm>(BehaviourAlgorithm, iPath);
            BehaviourAlgorithm lBehaviour = new BehaviourAlgorithm();
            //lBehaviour.Instructions = new List<ABehaviourInstruction>(BehaviourAlgorithm.Instructions);
            lBehaviour = Utils.UnserializeXML<BehaviourAlgorithm>(iPath);
            mStackUndoBli.AddLast(lBehaviour);
                if (mStackUndoBli.Count > 10)
                    mStackUndoBli.RemoveFirst();
        }


        public void FillItemsArray()
        {
            mArrayItems.Clear();
            foreach (DraggableItem lItem in panel.GetComponentsInChildren<DraggableItem>())
            {
                mArrayItems.Add(lItem.gameObject);
            }
        }

        public void Undo()
        {
            Debug.Log("undo");
            foreach(BehaviourAlgorithm behaviour in mStackUndoBli) {
                //Debug.Log("count du behaviour: " + behaviour.Instructions.Count);
            }
            if (mStackUndoBli.Count > 1)
            {
                Debug.Log("undo count: "+ mStackUndoBli.Count);
                mIsUndoing = true;
                mStackRedoBli.Push(mStackUndoBli.Last.Value);
                //if(mStackUndoBli.Count>1)
                    mStackUndoBli.RemoveLast();
                mNbModifs--;
                CleanSequence();
                BehaviourAlgorithm lList = mStackUndoBli.Last.Value;
                Debug.Log("count list: " + lList.Instructions.Count);
                ShowAlgo(lList);
                //SaveSequence();
            }
            else if(mStackUndoBli.Count == 1) {
                Debug.Log("undo count: " + mStackUndoBli.Count);
                mIsUndoing = true;
                mStackRedoBli.Push(mStackUndoBli.Last.Value);
                mNbModifs--;
                CleanSequence();
                BehaviourAlgorithm lList = mStackUndoBli.Last.Value;
                ShowAlgo(lList);
                mStackUndoBli.RemoveLast();
            }
        }

        public void Redo()
        {
            if (mStackRedoBli.Count > 0)
            {
                CleanSequence();
                BehaviourAlgorithm lList = mStackRedoBli.Pop();
                ShowAlgo(lList);
                mStackUndoBli.AddLast(lList);
                mNbModifs++;
            }
        }

        public void ResetModif()
        {
            mStackUndoBli.Clear();
            mStackRedoBli.Clear();
        }

        public static void EndModif()
        {
            if(OnModification!=null)
                OnModification();
        }

        /// <summary>
        /// Called everytime a modification occurs
        /// </summary>
        private void SaveModification()
        {
            Debug.Log("save la modif");
            //if (!mIsUndoing) {
                //Debug.Log("dans if save modif");
                SaveSequence();
                mNbModifs++;
                mStackRedoBli.Clear();
            //}
            //else
            //    mIsUndoing = false;
        }
    }
}