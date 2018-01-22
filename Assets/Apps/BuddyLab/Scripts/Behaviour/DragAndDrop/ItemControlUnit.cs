using UnityEngine;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Buddy;

namespace BuddyApp.BuddyLab
{ 

    /// <summary>
    /// Example of control unit for drag and drop events handle
    /// </summary>
    public class ItemControlUnit : MonoBehaviour
    {

        public delegate void NextAction(int iNum);
        public static NextAction OnNextAction;

        [SerializeField]
        private ItemManager itemManager;

        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private GameObject cell;

        private List<GameObject> mArrayItems;

        private BMLManager mBMLManager;

        private string mDirectoryPath = "";

        [SerializeField]
        private ConditionManager ConditionManager;

        [SerializeField]
        private LoopManager LoopManager;
        private int mLoopCounter;
        private int mIndex;
        public int Index { set { mIndex = value; } }

        private bool mIsRunning;
        public bool IsRunning { get { return mIsRunning; } set { mIsRunning = value; } }

        

        void Start()
        {
            mLoopCounter = 0;
            mIndex = 0;
            mIsRunning = false;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            mArrayItems = new List<GameObject>();
            mBMLManager = BYOS.Instance.Interaction.BMLManager;
            string lPath = BYOS.Instance.Resources.GetPathToRaw("os_laugh_01.wav");
            Debug.Log("path to sound: " + lPath);
        }

        void OnItemPlace(DragAndDropCell.DropDescriptor desc)
        {
            ItemControlUnit sourceSheet = desc.sourceCell.GetComponentInParent<ItemControlUnit>();
            ItemControlUnit destinationSheet = desc.destinationCell.GetComponentInParent<ItemControlUnit>();
            Debug.Log("truc ajouté");
            if (desc.sourceCell.cellType == DragAndDropCell.CellType.UnlimitedSource)
            {
                PlaceFromUnlimitedResources(desc);
            }
            if (desc.destinationCell.cellType == DragAndDropCell.CellType.Delete)
            {
                PlaceToDelete(desc);
            }
            if (desc.destinationCell.cellType == DragAndDropCell.CellType.Swap && desc.sourceCell.cellType == DragAndDropCell.CellType.Swap)
            {
                PlaceFromSwappedElements(desc);
            }

            if (desc.sourceCell.NumSwap == 1)
            {
                InitLoopItems();
            }
            // If item dropped between different sheets
            if (destinationSheet != sourceSheet)
            {
                Debug.Log(desc.item.name + " is dropped from " + sourceSheet.name + " to " + destinationSheet.name);
            }
        }


        public void ShowSequence(string iFileName)
        {
            mArrayItems = new List<GameObject>();
            GameObject child = Instantiate(cell);
            
            child.transform.parent = panel.transform;
            mArrayItems.Add(child);
            Canvas.ForceUpdateCanvases();
            float widthChild = child.GetComponent<RectTransform>().rect.width;
            float heightChild = child.GetComponent<RectTransform>().rect.height;
            mDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("Projects/"+iFileName);
            Debug.Log("directory path: " + mDirectoryPath);
            var fileInfo = new System.IO.FileInfo(mDirectoryPath);
            Debug.Log("size file: "+fileInfo.Length);
            if (fileInfo.Length > 0)
            {
                ListBLI lListBLI = Utils.UnserializeXML<ListBLI>(mDirectoryPath);
                Debug.Log("lol mdr");
                foreach (BLItemSerializable bli in lListBLI.List)
                {
                    
                    GameObject lItem = null;
                    if (bli.Category == Category.BML)
                    {
                        lItem = Instantiate(itemManager.GetBMLItem(bli.Index));
                        
                    }
                    else if (bli.Category == Category.CONDITION)
                        lItem = Instantiate(itemManager.GetConditionItem(bli.Index));
                    else if(bli.Category == Category.LOOP)
                    {
                        lItem = Instantiate(itemManager.GetLoopItem(bli.Index));
                        lItem.GetComponent<LoopItem>().NbItems = bli.NbItemsInLoop;
                    }

                    lItem.GetComponent<ABLItem>().Parameter = bli.Parameter;
                    lItem.transform.parent = mArrayItems[mArrayItems.Count - 1].transform;
                    //startPoint.SetActive(true);
                    Debug.Log("width du bonheur" + child.GetComponent<RectTransform>().rect.width);
                    //lItem.transform.SetParent(mArrayItems[mArrayItems.Count - 1].transform, true);
                    lItem.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);// new Vector3(widthChild / 2, heightChild/2, 0);
                    //startPoint.SetActive(false);
                    //lItem.GetComponent<RectTransform>().localPosition = new Vector3(52.5f, 80, 0);
                    //lItem.GetComponent<RectTransform>().localPosition = new Vector3(12, -35, 0);
                    GameObject lChild = Instantiate(cell);
                    lChild.transform.parent = panel.transform;
                    
                    mArrayItems.Add(lChild);
                    Canvas.ForceUpdateCanvases();
                    Debug.Log("category: " + bli.Category);
                    Debug.Log("index: " + bli.Index);
                }
                InitLoopItems();
               // mArrayItems[0].GetComponentInChildren<DragAndDropItem>().gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // new Vector3(widthChild / 2, heightChild / 2, 0);
            }
        }

        public void CleanSequence()
        {
            foreach (GameObject item in mArrayItems)
            {
                Destroy(item);
            }
            mArrayItems.Clear();
        }

        public void SaveSequence()
        {
            string text = "";
            ListBLI listBLI = new ListBLI();
            Debug.Log("le liste");
            FillItemsArray();
            foreach (GameObject cell in mArrayItems)
            {
                DragAndDropItem item = cell.GetComponentInChildren<DragAndDropItem>();
                if (item != null)
                    text += item.id + " ";
                if(item != null && item.gameObject.GetComponent<BMLItem>()!=null)
                    listBLI.List.Add(item.gameObject.GetComponent<BMLItem>().GetItem());
                else if(item != null && item.gameObject.GetComponent<ConditionItem>() != null)
                    listBLI.List.Add(item.gameObject.GetComponent<ConditionItem>().GetItem());
                else if (item != null && item.gameObject.GetComponent<LoopItem>() != null)
                    listBLI.List.Add(item.gameObject.GetComponent<LoopItem>().GetItem());
                Debug.Log("hum");
            }
            Debug.Log("sequence: " + text);
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
            Debug.Log("list count: " + listBLI.List.Count);
            Debug.Log("path: " + mDirectoryPath);
            Utils.SerializeXML<ListBLI>(listBLI, mDirectoryPath);
            //StartCoroutine(PlaySequence());
        }


        public  IEnumerator PlaySequence()
        {
            //Debug.Log("START PLAYSEQUENCE LOLOLOLOLOKRHGRETJREJREHGJHJEHJGEHJGE");
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
            ListBLI lListBLI = Utils.UnserializeXML<ListBLI>(mDirectoryPath);

            //foreach (BLItemSerializable bli in lListBLI.List)
            //mLoopCounter = 0;
            mIndex = 0;
            while(mIndex< lListBLI.List.Count)
            {
                BLItemSerializable bli = lListBLI.List[mIndex];

                if (OnNextAction!=null)
                {
                    OnNextAction(mIndex);
                }
               
                if (bli.Category == Category.BML)
                {
                    //Debug.Log("CATEGORY BML : ");
                    //Debug.Log("bli: " + bli.BML);
                    if (bli.ParameterKey != "")
                    {
                        //Debug.Log("key: " + bli.ParameterKey);
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add(bli.ParameterKey, bli.Parameter);
                        mBMLManager.LaunchByName(bli.BML, param);
                        Debug.Log("has launched : " + bli.BML + " with param: "+ bli.Parameter);
                    }
                    else
                    {
                        Debug.Log("has launched without param: " + mBMLManager.LaunchByName(bli.BML));
                    }
                    while (mIsRunning && mBMLManager.ActiveBML.Count>0 && mBMLManager.ActiveBML[0].IsRunning)
                    {
                        yield return null;
                    }
                }
                else if (bli.Category == Category.CONDITION)
                {
                    Debug.Log("CATEGORY CONDITION : ");
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
                        ConditionManager.ConditionType = "";
                }
                else if (bli.Category == Category.LOOP)
                {
                   //Debug.Log("INDEX LOOP CATEGORY : " + mIndex);
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
                        LoopManager.LoopCounter = mLoopCounter++;
                        mIndex -= (bli.NbItemsInLoop + 1);
                    }
                    Debug.Log("LOOPCOUNTER : " + mLoopCounter);
                    if (LoopManager.ChangeIndex)
                    {
                        mLoopCounter = 0;
                        LoopManager.LoopCounter = 0;
                        mIndex = LoopManager.IndexLoop;
                        LoopManager.IndexLoop = 0;
                        Debug.Log("CHANGE INDEX : " + (LoopManager.IndexLoop));
                        LoopManager.ChangeIndex = false;
                        LoopManager.LoopType = LoopType.NONE;
                    }
                   
                        
                    LoopManager.LoopType = bli.LoopType;
                    
                    if (bli.ParameterKey != "")
                    {
                        LoopManager.ParamLoop = bli.Parameter;
                    }
                    if (!mIsRunning)
                    {
                        LoopManager.LoopType = LoopType.NONE;
                        LoopManager.ChangeIndex = false;
                    }
                    LoopManager.ChangeIndex = false;
                }
                
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
            foreach (DragAndDropCell cell in panel.GetComponentsInChildren<DragAndDropCell>())
            {
                mArrayItems.Add(cell.gameObject);
            }
        }

        private void InitLoopItems()
        {
            Debug.Log("init loop");
            foreach(GameObject obj in mArrayItems)
            {
                //Debug.Log("b");
                if (obj.GetComponentInChildren<DragAndDropItem>()!=null)
                {
                    obj.GetComponentInChildren<DragAndDropItem>().LoopItem = null;
                    obj.GetComponentInChildren<DragAndDropItem>().draggedObjects = null;
                    //Debug.Log("c");
                }
                if(obj.GetComponentInChildren<LoopItem>()!=null)
                {
                    //Debug.Log("d");
                    DragAndDropItem lLoop = obj.GetComponentInChildren<LoopItem>().GetComponent<DragAndDropItem>();
                    lLoop.draggedObjects = new GameObject[obj.GetComponentInChildren<LoopItem>().NbItems];
                    Debug.Log("nombre item dans loop: " + obj.GetComponentInChildren<LoopItem>().NbItems);
                    for (int i=0; i< obj.GetComponentInChildren<LoopItem>().NbItems; i++)
                    {
                        Debug.Log("lul "+ obj.transform.GetSiblingIndex());
                        if (obj.transform.GetSiblingIndex() - i - 2 < mArrayItems.Count - 1)
                        {
                            lLoop.draggedObjects[i] = mArrayItems[obj.transform.GetSiblingIndex() - i - 2].GetComponentInChildren<DragAndDropItem>().gameObject;
                            //Debug.Log("lul2");
                            mArrayItems[obj.transform.GetSiblingIndex() - i - 2].GetComponentInChildren<DragAndDropItem>().LoopItem = obj.GetComponentInChildren<LoopItem>();
                            //Debug.Log("lul3");
                        }
                    }
                }
            }
        }

        private void PlaceFromUnlimitedResources(DragAndDropCell.DropDescriptor desc)
        {
            //if (desc.destinationCell.gameObject.GetComponentInChildren<DragAndDropItem>() == null)
            //{
            GameObject child = Instantiate(cell);
            child.transform.parent = panel.transform;
            if (desc.destinationCell.gameObject.GetComponentsInChildren<DragAndDropItem>().Length > 1)
            {
                //mArrayItems[desc.destinationCell.transform.GetSiblingIndex()].transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex() + 1);
                //Debug.Log("sibling index: " + desc.destinationCell.transform.GetSiblingIndex());
                //Debug.Log("position du child: x: " + desc.destinationCell.ItemPositionX);
                //Debug.Log("position du item: x: " + (mArrayItems[desc.destinationCell.transform.GetSiblingIndex()+1].GetComponentInChildren<DragAndDropItem>().transform.position.x - (desc.destinationCell.GetComponent<RectTransform>().rect.width*2) )  );
                foreach(DragAndDropItem dragitem in desc.destinationCell.gameObject.GetComponentsInChildren<DragAndDropItem>())
                {
                    if(dragitem.LoopItem!=null)
                    {
                        dragitem.LoopItem.AddItem();
                    }
                }
                if (desc.destinationCell.transform.GetSiblingIndex() >= mArrayItems.Count - 1 || mArrayItems[desc.destinationCell.transform.GetSiblingIndex() + 1].GetComponentInChildren<DragAndDropItem>() == null)
                {
                    //Debug.Log("a");
                    child.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex() + 1);
                }
                else if (desc.destinationCell.transform.GetSiblingIndex() < mArrayItems.Count - 1 && desc.destinationCell.ItemPositionX > (mArrayItems[desc.destinationCell.transform.GetSiblingIndex() + 1].GetComponentInChildren<DragAndDropItem>().transform.position.x - (desc.destinationCell.GetComponent<RectTransform>().rect.width * 2)))
                {
                    //Debug.Log("b");
                    child.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex());
                }
                else
                {
                    //Debug.Log("c");
                    child.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex() + 1);
                }
                desc.destinationCell.ItemToRemove.gameObject.transform.parent = child.transform;
                Canvas.ForceUpdateCanvases();
                desc.destinationCell.ItemToRemove.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                //FillItemsArray();

                if (desc.destinationCell.GetItem().gameObject.GetComponent<ABLItem>().GetItem().Category == Category.LOOP)
                {
                    Debug.Log("loop inclu");
                    desc.destinationCell.GetItem().gameObject.transform.GetChild(0).localPosition = new Vector3(-56, 6.7F, 0);
                    desc.destinationCell.GetItem().draggedObjects = new GameObject[1];
                    desc.destinationCell.GetItem().draggedObjects[0] = mArrayItems[desc.destinationCell.transform.GetSiblingIndex() - 2].GetComponentInChildren<DragAndDropItem>().gameObject;
                    mArrayItems[desc.destinationCell.transform.GetSiblingIndex() - 2].GetComponentInChildren<DragAndDropItem>().LoopItem = desc.destinationCell.GetItem().gameObject.GetComponent<LoopItem>();
                    //desc.destinationCell.GetItem().draggedObjects[1] = mArrayItems[desc.destinationCell.transform.GetSiblingIndex()-3].GetComponentInChildren<DragAndDropItem>().gameObject;

                }

                desc.destinationCell.ItemToRemove = null;
            }
            //child.transform.SetSiblingIndex(child.transform.GetSiblingIndex() - 1);
            //child.transform.SetSiblingIndex(1);
            FillItemsArray();
            InitLoopItems();
            //mArrayItems.Insert(child.transform.GetSiblingIndex(), child);
            //}
        }

        private void PlaceFromSwappedElements(DragAndDropCell.DropDescriptor desc)
        {
            Debug.Log("source index: " + desc.sourceCell.transform.GetSiblingIndex());
            Debug.Log("destinationCell index: " + desc.destinationCell.transform.GetSiblingIndex());
            Debug.Log("num swap: " + desc.sourceCell.NumSwap);
            bool lSwapToLeft = desc.destinationCell.transform.GetSiblingIndex() < desc.sourceCell.transform.GetSiblingIndex();
            //if( (desc.sourceCell.NumSwap == 1 || desc.destinationCell.GetComponentInChildren<LoopItem>()==null) )
                desc.sourceCell.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex());
            FillItemsArray();
            if (desc.sourceCell.NumSwap == 1)
            {
                //desc.sourceCell.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex());
                //FillItemsArray();
                if (desc.destinationCell.GetItem().gameObject.GetComponent<ABLItem>().GetItem().Category == Category.LOOP && desc.sourceCell.GetItem().LoopItem == null)
                {
                    desc.destinationCell.GetItem().gameObject.GetComponent<LoopItem>().AddItem();
                    desc.sourceCell.GetItem().LoopItem = desc.destinationCell.GetItem().gameObject.GetComponent<LoopItem>();
                    Debug.Log("le 1");

                    desc.destinationCell.GetItem().draggedObjects = new GameObject[desc.sourceCell.GetItem().LoopItem.NbItems];
                    for (int i = 0; i < desc.sourceCell.GetItem().LoopItem.NbItems; i++)
                    {
                        desc.destinationCell.GetItem().draggedObjects[i] = mArrayItems[desc.destinationCell.transform.GetSiblingIndex() - i].GetComponentInChildren<DragAndDropItem>().gameObject;
                    }
                }

                if (desc.destinationCell.GetItem().LoopItem != null && desc.sourceCell.GetItem().LoopItem == null && desc.sourceCell.GetComponentInChildren<LoopItem>() == null)
                {
                    desc.destinationCell.GetItem().LoopItem.AddItem();
                    desc.sourceCell.GetItem().LoopItem = desc.destinationCell.GetItem().LoopItem;
                    Debug.Log("le 2");

                    desc.sourceCell.GetItem().LoopItem.gameObject.GetComponent<DragAndDropItem>().draggedObjects = new GameObject[desc.sourceCell.GetItem().LoopItem.NbItems];
                    for (int i = 0; i < desc.sourceCell.GetItem().LoopItem.NbItems; i++)
                    {
                        desc.sourceCell.GetItem().LoopItem.gameObject.GetComponent<DragAndDropItem>().draggedObjects[i] = mArrayItems[desc.destinationCell.transform.GetSiblingIndex() - i].GetComponentInChildren<DragAndDropItem>().gameObject;
                    }
                }

                else if (desc.sourceCell.GetItem().LoopItem != null && (desc.destinationCell.GetItem().LoopItem == null && desc.destinationCell.GetItem().gameObject.GetComponent<LoopItem>() == null))
                {
                    Debug.Log("le 3");
                    desc.sourceCell.GetItem().LoopItem.RemoveItem();
                    desc.sourceCell.GetItem().LoopItem.gameObject.GetComponent<DragAndDropItem>().draggedObjects = new GameObject[desc.sourceCell.GetItem().LoopItem.NbItems];
                    for (int i = 0; i < desc.sourceCell.GetItem().LoopItem.NbItems; i++)
                    {
                        desc.sourceCell.GetItem().LoopItem.gameObject.GetComponent<DragAndDropItem>().draggedObjects[i] = mArrayItems[desc.destinationCell.transform.GetSiblingIndex() - i].GetComponentInChildren<DragAndDropItem>().gameObject;
                    }

                    desc.sourceCell.GetItem().LoopItem = null;
                }

            }


            if (desc.sourceCell.GetItem().gameObject.GetComponent<ABLItem>().GetItem().Category == Category.LOOP )
            {
                if (desc.sourceCell.GetItem() == null)
                    Debug.Log("getitem null");
                if(desc.sourceCell.GetItem().draggedObjects==null)
                    Debug.Log("draggedobjects null");
                
                if (lSwapToLeft && desc.sourceCell.NumSwap == 1)
                {
                    Debug.Log("to left");
                    for (int i = desc.sourceCell.GetItem().draggedObjects.Length - 1; i >= 0; i--)
                    {
                        Debug.Log("woosh");
                        if(desc.sourceCell.GetItem().draggedObjects[i]!=null && desc.sourceCell.GetItem().draggedObjects[i].GetComponentInParent<DragAndDropCell>()!=null)
                            desc.sourceCell.GetItem().draggedObjects[i].GetComponentInParent<DragAndDropCell>().transform.SetSiblingIndex(desc.sourceCell.transform.GetSiblingIndex());
                    }
                }
                else if(!lSwapToLeft)
                {
                    Debug.Log("to right");
                    for (int i = desc.sourceCell.GetItem().draggedObjects.Length - 1; i >= 0; i--)
                    {
                        Debug.Log("woosh");
                        if (desc.sourceCell.GetItem().draggedObjects[i] != null && desc.sourceCell.GetItem().draggedObjects[i].GetComponentInParent<DragAndDropCell>() != null)
                            desc.sourceCell.GetItem().draggedObjects[i].GetComponentInParent<DragAndDropCell>().transform.SetSiblingIndex(desc.sourceCell.transform.GetSiblingIndex() - 1);
                    }
                }

                //desc.destinationCell.GetItem().draggedObjects[1] = mArrayItems[2].GetComponentInChildren<DragAndDropItem>().gameObject;
            }

            FillItemsArray();
            //InitLoopItems();
            //Debug.Log("DU FRIC!!!");
            //mArrayItems.Remove(desc.sourceCell.gameObject);
            //mArrayItems.Insert(desc.sourceCell.transform.GetSiblingIndex(), desc.sourceCell.gameObject);
        }

        private void PlaceToDelete(DragAndDropCell.DropDescriptor desc)
        {
            Debug.Log("supprimé");
            if (desc.sourceCell.GetComponentInChildren<DragAndDropItem>() != null && desc.sourceCell.GetComponentInChildren<DragAndDropItem>().LoopItem != null)
            {
                Debug.Log("dans la suppression");
                desc.sourceCell.GetComponentInChildren<DragAndDropItem>().LoopItem.RemoveItem();
            }
            mArrayItems.Remove(desc.sourceCell.gameObject);
            Destroy(desc.sourceCell.gameObject);
            FillItemsArray();
            InitLoopItems();
        }
    }
}