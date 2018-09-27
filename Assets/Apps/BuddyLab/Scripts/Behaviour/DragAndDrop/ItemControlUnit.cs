using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BlueQuark;
using System;

namespace BuddyApp.BuddyLab
{ 

    /// <summary>
    /// Example of control unit for drag and drop events handle
    /// </summary>
    public sealed class ItemControlUnit : MonoBehaviour
    {

        public delegate void NextAction(int iNum);
        public static NextAction OnNextAction;

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

        [SerializeField]
        private ConditionManager ConditionManager;
        private string mConditionParam;

        [SerializeField]
        private LoopManager LoopManager;
        private int mLoopCounter;
        private int mIndex;
        public int Index { set { mIndex = value; } }

        private bool mIsRunning;
        public bool IsRunning { get { return mIsRunning; } set { mIsRunning = value; } }

        [SerializeField]
        private SpecialItemManager SpecialItemManager;

        private int mNbModifs = 0;

        private LinkedList<ListBLI> mStackUndoBli;
        private Stack<ListBLI> mStackRedoBli;

        public BehaviourAlgorithm BehaviourAlgorithm { get; set; }


        void Start()
        {
            mLoopCounter = 1;
            mIndex = 0;
            mIsRunning = false;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            mArrayItems = new List<GameObject>();
            //mBMLManager = BYOS.Instance.Interaction.BMLManager;
            sequenceContainer.OnModification += SaveModification;
            trashZoneContainer.OnModification += SaveModification;
            //Directory.Delete(BYOS.Instance.Resources.GetPathToRaw("Temp", LoadContext.APP), true);
            mStackUndoBli = new LinkedList<ListBLI>();
            mStackRedoBli = new Stack<ListBLI>();
            BehaviourAlgorithm = new BehaviourAlgorithm();
        }


        public void ShowSequence(string iFileName)
        {
            mStackUndoBli.Clear();
            mDirectoryPath = Buddy.Resources.GetRawFullPath("Projects" + "/" + iFileName);
            Debug.Log(mDirectoryPath);
            ShowSequence(iFileName, "Projects");
        }

        public void ShowSequence(string iFileName, string iDirectory)
        {
            mArrayItems = new List<GameObject>();

            string lDirectoryPath = Buddy.Resources.GetRawFullPath(iDirectory+"/" + iFileName);
            Debug.Log(lDirectoryPath);
            var fileInfo = new System.IO.FileInfo(lDirectoryPath);
            if (fileInfo.Length > 0)
            {
                ListBLI lListBLI = Utils.UnserializeXML<ListBLI>(lDirectoryPath);
                foreach (BLItemSerializable bli in lListBLI.List)
                {

                    GameObject lItem = null;
                    if (bli.Category == Category.BML)
                    {
                        lItem = Instantiate(itemManager.GetBMLItem(bli.Index));

                    }
                    else if (bli.Category == Category.CONDITION)
                    {
                        lItem = Instantiate(itemManager.GetConditionItem(bli.Index));
                    }
                    else if (bli.Category == Category.LOOP)
                    {
                        //Debug.Log("truc 0");
                        lItem = Instantiate(itemManager.GetLoopItem(bli.Index));
                        lItem.GetComponent<LoopItem>().NbItems = bli.NbItemsInLoop;
                        if(bli.LoopType==LoopType.SENSOR && lItem.GetComponent<LoopConditionManager>()!=null)
                        {
                            lItem.GetComponent<LoopConditionManager>().ChangeIcon(itemManager.GetConditionItemFromName(bli.Parameter).transform.GetChild(3).GetComponent<Image>().sprite);
                        }
                    }
                    else if (bli.Category == Category.SPECIAL)
                    {
                        lItem = Instantiate(itemManager.GetSpecialItem(bli.Index));
                    }

                    lItem.GetComponent<ABLItem>().Parameter = bli.Parameter;
                    lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
                    //lItem.transform.parent = panel.transform; //mArrayItems[mArrayItems.Count - 1].transform;
                    lItem.transform.SetParent(panel.transform, false);

                    if (lItem.GetComponent<LoopItem>() != null)
                    {
                        lItem.GetComponent<LoopItem>().InitLoop(panel.transform);
                    }

                    mArrayItems.Add(lItem);
                    //Canvas.ForceUpdateCanvases();
                    //Debug.Log("category: " + bli.Category);
                    //Debug.Log("index: " + bli.Index);
                }

            }

            //string lPath = BYOS.Instance.Resources.GetPathToRaw("Temp", LoadContext.APP) + "/0.xml";
            if (mStackUndoBli.Count==0)
            {
                SaveModification();
            }
        }

        public void ShowSequence(ListBLI iListBLI)
        {
           
            foreach (BLItemSerializable bli in iListBLI.List)
            {

                GameObject lItem = null;
                if (bli.Category == Category.BML)
                {
                    lItem = Instantiate(itemManager.GetBMLItem(bli.Index));

                }
                else if (bli.Category == Category.CONDITION)
                    lItem = Instantiate(itemManager.GetConditionItem(bli.Index));
                else if (bli.Category == Category.LOOP)
                {
                    //Debug.Log("truc 0");
                    lItem = Instantiate(itemManager.GetLoopItem(bli.Index));
                    lItem.GetComponent<LoopItem>().NbItems = bli.NbItemsInLoop;
                }
                else if (bli.Category == Category.SPECIAL)
                {
                    lItem = Instantiate(itemManager.GetSpecialItem(bli.Index));
                }
                //Debug.Log("truc 1");
                lItem.GetComponent<ABLItem>().Parameter = bli.Parameter;
                //Debug.Log("truc 1.2");
                lItem.GetComponent<DraggableItem>().OnlyDroppable = false;
                //Debug.Log("truc 1.3");
                lItem.transform.parent = panel.transform; //mArrayItems[mArrayItems.Count - 1].transform;
                                                          //Debug.Log("truc 2");
                if (lItem.GetComponent<LoopItem>() != null)
                {
                    //Debug.Log("truc 3");
                    lItem.GetComponent<LoopItem>().InitLoop(panel.transform);
                }


                mArrayItems.Add(lItem);
                //Canvas.ForceUpdateCanvases();
                //Debug.Log("category: " + bli.Category);
                //Debug.Log("index: " + bli.Index);
            }
        }

        public void CleanSequence()
        {
            foreach (GameObject item in mArrayItems)
            {
                Destroy(item);
            }

            foreach(Transform child in panel.transform)
            {
                if(child.gameObject.name=="endofloop")
                {
                    Destroy(child.gameObject);
                }
            }
            mArrayItems.Clear();
        }

        public void SaveSequence()
        {
            SaveSequence(mDirectoryPath);
        }

        public void SaveSequence(string iPath)
        {
            ListBLI lListBLI = new ListBLI();
            FillItemsArray();
            foreach (GameObject item in mArrayItems)
            {

                if (item != null && item.GetComponent<BMLItem>() != null)
                    lListBLI.List.Add(item.GetComponent<BMLItem>().GetItem());
                else if (item != null && item.GetComponent<ConditionItem>() != null)
                    lListBLI.List.Add(item.GetComponent<ConditionItem>().GetItem());
                else if (item != null && item.GetComponent<LoopItem>() != null)
                    lListBLI.List.Add(item.GetComponent<LoopItem>().GetItem());
                else if (item != null && item.GetComponent<SpecialItem>() != null)
                    lListBLI.List.Add(item.GetComponent<SpecialItem>().GetItem());
            }

            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
            Utils.SerializeXML<ListBLI>(lListBLI, iPath);
            mStackUndoBli.AddLast(lListBLI);
            if (mStackUndoBli.Count > 10)
                mStackUndoBli.RemoveFirst();
        }


        public void SaveAlgorithm()
        {
            foreach (GameObject item in mArrayItems)
            {
                if (item != null && item.GetComponent<AGraphicElement>() != null)
                    BehaviourAlgorithm.Instructions.Add(item.GetComponent<AGraphicElement>().GetInstruction());
            }
            Utils.SerializeXML<BehaviourAlgorithm>(BehaviourAlgorithm, Buddy.Resources.GetRawFullPath("algo.xml"));
        }

        public  IEnumerator PlaySequence()
        {
            
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
            ListBLI lListBLI = Utils.UnserializeXML<ListBLI>(mDirectoryPath);

            //foreach (BLItemSerializable bli in lListBLI.List)
            //mLoopCounter = 0;
            mIndex = 0;
            Debug.Log("ENTER PLAY SEQUENCE : " + ConditionManager.IsEventDone);
            ConditionManager.IsEventDone = false;
            Debug.Log("ENTER PLAY SEQUENCE 2 : " + ConditionManager.IsEventDone);
            while (mIndex< lListBLI.List.Count)
            {
                BLItemSerializable bli = lListBLI.List[mIndex];

                if (OnNextAction!=null)
                {
                    OnNextAction(mIndex);
                } 
                
               
                if (bli.Category == Category.BML)
                {
                    ConditionManager.IsInCondition = false;
                    Debug.Log("PLAY SEQ : BML");
                    if (bli.ParameterKey != "")
                    {
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add(bli.ParameterKey, bli.Parameter);
                        Debug.Log("try to launch : " + bli.BML + " with param: " + bli.Parameter);
                        //mBMLManager.Interpreter.Run(bli.BML, param);
                        
                        Debug.Log("has launched : " + bli.BML + " with param: "+ bli.Parameter);
                    }
                    else
                    {
                        Debug.Log("try to launch without param: " + bli.BML);
                        Debug.Log("has launched without param: " + mBMLManager.Interpreter.Run(bli.BML));
                    }
                    while (mIsRunning && mBMLManager.IsBusy /*mBMLManager.ActiveBML.Count>0 && mBMLManager.ActiveBML[0].IsRunning*/)
                    {
                        yield return null;
                    }
                }
                else if (bli.Category == Category.CONDITION)
                {
                    Debug.Log("PLAY SEQ : CONDITION");
                    ConditionManager.IsInCondition = true;
                    ConditionManager.ConditionType = bli.ConditionName;
                    if (bli.ParameterKey != "")
                    {
                        ConditionManager.ParamCondition = bli.Parameter;
                    }
                    while (!ConditionManager.IsEventDone && mIsRunning)
                    {
                        yield return null;
                    }
                    if (!mIsRunning)
                    {
                        ConditionManager.ConditionType = "";
                        ConditionManager.IsInCondition = false;
                    }
                        
                }
                else if (bli.Category == Category.LOOP)
                {
                    ConditionManager.IsInCondition = false;
                    Debug.Log("PLAY SEQ : LOOP");
                    if (bli.Parameter != "")
                    {
                        LoopManager.ParamLoop = bli.Parameter;
                    }
                    LoopManager.LoopType = bli.LoopType;
                    LoopManager.IndexLoop = mIndex;
                    if(bli.LoopType == LoopType.INFINITE)
                    {
                        mIndex -= (bli.NbItemsInLoop + 1);
                        if (!mIsRunning)
                        {
                            LoopManager.ResetParam();
                            LoopManager.NeedChangeIndex();
                        }   
                    }
                    if (bli.LoopType == LoopType.LOOP_X && !LoopManager.ChangeIndex)
                    {
                        if(Int32.Parse(bli.Parameter) != 1 )
                        {
                            LoopManager.LoopCounter = mLoopCounter++;
                            mIndex -= (bli.NbItemsInLoop + 1);
                        }

                    }
                    if(bli.LoopType == LoopType.SENSOR && !LoopManager.ChangeIndex)
                    {
                        mIndex -= (bli.NbItemsInLoop + 1);
                        mConditionParam = bli.Parameter;
                        if (LoopManager.IsSensorLoopWithParam)
                        {
                            if (ConditionManager.ConditionType == "")
                                ConditionManager.ConditionType = mConditionParam;
                            if (!mIsRunning)
                                ConditionManager.ConditionType = "";
                        }
                        
                        
                    }
                    if (LoopManager.ChangeIndex)
                    {
                        mConditionParam = "";
                        ConditionManager.ConditionType = "";
                        mLoopCounter = 1;
                        LoopManager.LoopCounter = 1;
                        mIndex = LoopManager.IndexLoop;
                        LoopManager.IndexLoop = 0;
                        LoopManager.ChangeIndex = false;
                        LoopManager.LoopType = LoopType.NONE;
                        LoopManager.IsSensorLoopWithParam = false;
                    }
                    
                    if (!mIsRunning)
                    {
                        LoopManager.LoopType = LoopType.NONE;
                        LoopManager.ChangeIndex = false;
                    }
                    LoopManager.ChangeIndex = false;
                }
                else if (bli.Category == Category.SPECIAL)
                {
                    Debug.Log("CATEGORY SPECIAL ITEM 1 " + bli.Index + " " + bli.Parameter + " " + bli.ParameterKey);
                    if (bli.ParameterKey != "")
                    {
                        //SpecialItemManager.NameSItem = bli.ParameterKey;
                        //Debug.Log("CATEGORY SPECIAL ITEM 2: " + bli.ParameterKey);
                        //if (bli.ParameterKey == "GOTO")
                        //{
                        //    mIndex = Int32.Parse(bli.Parameter);
                        //    Debug.Log("CATEGORY SPECIAL ITEM 3: " + bli.Parameter);
                        //}
                    }
                    yield return new WaitForSeconds(1.0F);

                    mIndex = Int32.Parse(bli.Parameter) - 1;
                }
                ConditionManager.IsInCondition = false;
                ConditionManager.IsEventDone = false;
                if (!mIsRunning)
                {
                    break;
                }
                mIndex++;
            }
            

           
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
            Debug.Log("nb modif: " + mNbModifs);
            if(mStackUndoBli.Count>1)//mNbModifs>1)
            {
                mStackRedoBli.Push(mStackUndoBli.Last.Value);//mQueueUndoBli.ToArray()[mNbModifs]);
                mStackUndoBli.RemoveLast();
                mNbModifs--;
                CleanSequence();
                ListBLI lList = mStackUndoBli.Last.Value;
                ShowSequence(lList);
                //ShowSequence(mQueueUndoBli.ToArray()[mNbModifs-1]);
                //ShowSequence((mNbModifs-1) + ".xml", "Temp");
            }
            //else if(mStackUndoBli.Count==1)
            //{
            //    ListBLI lList = mStackUndoBli.Last.Value;
            //    mStackRedoBli.Push(lList);
            //}
        }

        public void Redo()
        {
            //string lPath = BYOS.Instance.Resources.GetPathToRaw("Temp", LoadContext.APP) + "/" + (mNbModifs) + ".xml";
            if (mStackRedoBli.Count>0)//(File.Exists(lPath))
            {
                CleanSequence();
                //ShowSequence(mQueueUndoBli.ToArray()[mNbModifs]);
                //mStackRedoBli.Pop();
                ListBLI lList = mStackRedoBli.Pop();
                ShowSequence(lList);
                mStackUndoBli.AddLast(lList);
                //ShowSequence((mNbModifs) + ".xml", "Temp");
                mNbModifs++;
            }
        }

        /// <summary>
        /// Called everytime a modification occurs
        /// </summary>
        private void SaveModification()
        {
            //string lPath = "";
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("Temp", LoadContext.APP);
            //if (mNbModifs < 10)
            //{
                //if(!Directory.Exists(lDirectoryPath))
                //    Directory.CreateDirectory(lDirectoryPath);

                //using (var file = File.Create(BYOS.Instance.Resources.GetPathToRaw("Temp", LoadContext.APP) + "/" + mNbModifs + ".xml"))
                //{
                //    lPath = file.Name;
                //    Debug.Log("avant save sequence");
                //}
                SaveSequence();
                mNbModifs++;
                mStackRedoBli.Clear();
           // }
            Debug.Log("modif saved");
        }
    }
}