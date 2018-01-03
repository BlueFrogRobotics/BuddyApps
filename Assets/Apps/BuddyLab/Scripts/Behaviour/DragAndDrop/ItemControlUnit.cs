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

        [SerializeField]
        private ItemManager itemManager;

        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private GameObject cell;

        [SerializeField]
        private GameObject startPoint;

        private List<GameObject> mArrayItems;

        private BMLManager mBMLManager;

        private string mDirectoryPath = "";

        [SerializeField]
        private ConditionManager ConditionManager;

        private bool mIsRunning;
        public bool IsRunning { get { return mIsRunning; } set { mIsRunning = value; } }
        

        void Start()
        {
            IsRunning = false;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            mArrayItems = new List<GameObject>();
            mBMLManager = BYOS.Instance.Interaction.BMLManager;
            //mBMLManager.LoadAppBML();
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
                //if (desc.destinationCell.gameObject.GetComponentInChildren<DragAndDropItem>() == null)
                //{
                GameObject child = Instantiate(cell);
                child.transform.parent = panel.transform;
                if (desc.destinationCell.gameObject.GetComponentsInChildren<DragAndDropItem>().Length > 1)
                {
                    child.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex());
                    desc.destinationCell.ItemToRemove.gameObject.transform.parent = child.transform;
                    Canvas.ForceUpdateCanvases();
                    desc.destinationCell.ItemToRemove.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    desc.destinationCell.ItemToRemove = null;
                }
                //child.transform.SetSiblingIndex(child.transform.GetSiblingIndex() - 1);
                //child.transform.SetSiblingIndex(1);
                FillItemsArray();
                //mArrayItems.Insert(child.transform.GetSiblingIndex(), child);
                //}
            }
            if (desc.destinationCell.cellType == DragAndDropCell.CellType.Delete)
            {
                Debug.Log("supprimé");
                mArrayItems.Remove(desc.sourceCell.gameObject);
                Destroy(desc.sourceCell.gameObject);
            }
            if (desc.destinationCell.cellType == DragAndDropCell.CellType.Swap && desc.sourceCell.cellType == DragAndDropCell.CellType.Swap)
            {
                desc.sourceCell.transform.SetSiblingIndex(desc.destinationCell.transform.GetSiblingIndex());
                FillItemsArray();
                //mArrayItems.Remove(desc.sourceCell.gameObject);
                //mArrayItems.Insert(desc.sourceCell.transform.GetSiblingIndex(), desc.sourceCell.gameObject);
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
            foreach (GameObject cell in mArrayItems)
            {
                DragAndDropItem item = cell.GetComponentInChildren<DragAndDropItem>();
                if (item != null)
                    text += item.id + " ";
                if(item != null && item.gameObject.GetComponent<BMLItem>()!=null)
                    listBLI.List.Add(item.gameObject.GetComponent<BMLItem>().GetItem());
                else if(item != null && item.gameObject.GetComponent<ConditionItem>() != null)
                    listBLI.List.Add(item.gameObject.GetComponent<ConditionItem>().GetItem());
                Debug.Log("hum");
            }
            Debug.Log("sequence: " + text);
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
            Debug.Log("list count: " + listBLI.List.Count);
            Debug.Log("path: " + mDirectoryPath);
            Utils.SerializeXML<ListBLI>(listBLI, mDirectoryPath);
            //StartCoroutine(PlaySequence());
        }

        public IEnumerator PlaySequence()
        {
            if (mIsRunning)
            {
                //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("project.xml");
                ListBLI lListBLI = Utils.UnserializeXML<ListBLI>(mDirectoryPath);

                foreach (BLItemSerializable bli in lListBLI.List)
                {

                    if (bli.Category == Category.BML)
                    {
                        Debug.Log("bli: " + bli.BML);
                        if (bli.ParameterKey != "")
                        {
                            //Debug.Log("key: " + bli.ParameterKey);
                            Dictionary<string, string> param = new Dictionary<string, string>();
                            param.Add(bli.ParameterKey, bli.Parameter);
                            //Changer launchbyID par launchbyName
                            //Debug.Log("has launched with param: " + mBMLManager.LaunchByID(bli.BML, param));
                            Debug.Log("has launched without param: " + mBMLManager.LaunchByName(bli.BML, param));
                        }
                        else
                        {
                            //Changer launchbyID par launchbyName
                            //Debug.Log("has launched without param: " + mBMLManager.LaunchByID(bli.BML));
                            Debug.Log("has launched without param: " + mBMLManager.LaunchByName(bli.BML));
                        }
                        Debug.Log("nimporte quoi");
                        Debug.Log("kikoo" + mBMLManager.ActiveBML.Count);
                        Debug.Log("nimporte quoi 2");
                        while (mBMLManager.ActiveBML.Count > 0 && mBMLManager.ActiveBML[0].IsRunning)
                        {
                            //Debug.Log("ITEM CONTROL UNIT : BML FINI ");
                            //Debug.Log("BML NAME: " + mBMLManager.ActiveBML[0].Name);
                            yield return null;
                        }
                    }
                    else if (bli.Category == Category.CONDITION)
                    {
                        ConditionManager.ConditionType = bli.ConditionName;
                        if (bli.ParameterKey != "")
                        {
                            ConditionManager.ParamCondition = bli.Parameter;
                        }
                        //Debug.Log("ITEMCONTROLUNIT : CONDITION : " + bli.ConditionName);
                        while (!ConditionManager.IsEventDone)
                        {
                            Debug.Log("CONDITION COROUTINE");
                            yield return null;
                        }

                    }
                    else if (bli.Category == Category.LOOP)
                    {
                        Debug.Log("ITEMCONTROLUNIT : LOOP ");

                    }
                    ConditionManager.IsEventDone = false;
                }
            }
            else
                yield return null;
           
        }

        public void FillItemsArray()
        {
            mArrayItems.Clear();
            foreach (DragAndDropCell cell in panel.GetComponentsInChildren<DragAndDropCell>())
            {
                mArrayItems.Add(cell.gameObject);
            }
        }
    }
}