using BlueQuark;

using UnityEngine;

using System.Threading;
using System.Globalization;
using System.Collections.Generic;




namespace BuddyApp.BuddyLab
{ 

    /// <summary>
    /// Example of control unit for drag and drop events handle
    /// </summary>
    public sealed class ItemControlUnit : MonoBehaviour
    {

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



        void Start()
        {
            mLoopCounter = 1;
            mIndex = 0;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            mArrayItems = new List<GameObject>();
            sequenceContainer.OnModification += SaveModification;
            trashZoneContainer.OnModification += SaveModification;
            mStackUndoBli = new LinkedList<BehaviourAlgorithm>();
            mStackRedoBli = new Stack<BehaviourAlgorithm>();
            BehaviourAlgorithm = new BehaviourAlgorithm();
        }

        public void ShowAlgo(string iFileName)
        {
            FillBehaviourAlgorithm(iFileName);

            OpenProjectVisitor lVisitor = new OpenProjectVisitor(itemManager, panel.transform);
            lVisitor.Visit(BehaviourAlgorithm);
        }

        public void ShowAlgo(BehaviourAlgorithm iBehaviourAlgorithm)
        {
            Debug.Log("show algo");
            OpenProjectVisitor lVisitor = new OpenProjectVisitor(itemManager, panel.transform);
            lVisitor.Visit(iBehaviourAlgorithm);
        }

        public void FillBehaviourAlgorithm(string iFileName)
        {
            mDirectoryPath = Buddy.Resources.GetRawFullPath("Projects" + "/" + iFileName);
            Debug.Log("le full path: " + mDirectoryPath);
            BehaviourAlgorithm.Instructions.Clear();
            BehaviourAlgorithm lAlgo = Utils.UnserializeXML<BehaviourAlgorithm>(mDirectoryPath);
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
            mStackUndoBli.AddLast(BehaviourAlgorithm);
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
            if (mStackUndoBli.Count > 1)
            {
                mStackRedoBli.Push(mStackUndoBli.Last.Value);
                mStackUndoBli.RemoveLast();
                mNbModifs--;
                CleanSequence();
                BehaviourAlgorithm lList = mStackUndoBli.Last.Value;
                ShowAlgo(lList);
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

        /// <summary>
        /// Called everytime a modification occurs
        /// </summary>
        private void SaveModification()
        {
                SaveSequence();
                mNbModifs++;
                mStackRedoBli.Clear();
        }
    }
}