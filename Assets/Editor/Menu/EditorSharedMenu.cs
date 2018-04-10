using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using Buddy;
using Buddy.Internal.System;
using Buddy.Internal.AppManagement;
using Buddy.Editor;

//public class EditorSharedMenu : EditorWindow {
    public class EditorSharedMenu : AWindow {

    //TODO : Do something to chose the app where you add the XML then we can chose the path in the build XML

    public class MenuItemObject
    {
        public string key;
        public string trigger;
        public bool quitApp;
    }

    private bool mChangeExistentXML;
    private bool mCreateXML;
    private bool mEnterClicked = false;
    private bool mClearItems = false;
    private static AppEditor sWindow;
    private AppInfo mAppInfo;
    //set an initial length or you might have some problems with the editor
    private List<MenuItemObject> items = new List<MenuItemObject>(0);

    private string nameOfXml;

    private int mListSize;
    private Vector2 mScrollView;
    private bool mChangeIndex;
    private string mStringKey;
    private string mStringTrigger;
    private bool mBoolQuitApp;
    private int mValue;
    private bool mResult;
    private bool mIsDraw = true;
    private bool mIsDrawCreateXML = false;
    private int mSaveListSize;

    private static string mAppPath;

    [MenuItem("Buddy/Shared/Menu Item")]
    public static void ShowWindow()
    {
        SelectAppPopup.Show(LoadApp);
        
    }

    private void OnEnable()
    {
        mChangeExistentXML = false;
        mCreateXML = false;
        mChangeIndex = false;
        //mEnterClicked = false;
    }

    private static void LoadApp(string iApplicationPath)
    {
        ////string lMetaPath = iApplicationPath + "/Config/meta.xml";
        //string lMetaPath = iApplicationPath + "/Resources/Raw/meta.xml";
        mAppPath = iApplicationPath;
        //if (!File.Exists(mAppPath))
        //{
        //    EditorUtils.LogE(LogContext.EDITOR, LogInfo.NOT_FOUND,
        //        "Cannot edit app settings. Cannot find " + lMetaPath);
        //    return;
        //} 
        if(string.IsNullOrEmpty(mAppPath))
        {
            EditorUtils.LogE(LogContext.EDITOR, LogInfo.NOT_FOUND, "Path is empty");
        }
        else
        {
            EditorWindow.GetWindow(typeof(EditorSharedMenu));
        }

        //AppInfo lAppInfo = Utils.UnserializeXML<AppInfo>(lMetaPath);
        //if (lAppInfo == null)
        //{
        //    EditorUtils.LogE(LogContext.EDITOR, LogInfo.READING,
        //        "Cannot edit app settings. Error while reading " + lMetaPath);
        //    return;
        //}

        //sWindow = (AppEditor)GetWindowWithRect(typeof(AppEditor), new Rect(100, 100, 350, 560), true,
        //    "Edit " + lAppInfo.ShortName + " settings");
        //sWindow.maxSize = new Vector2(350, 560);
        //sWindow.maxSize = new Vector2(350, 560);
        //sWindow.mApplicationPath = iApplicationPath + "/";
        //sWindow.mAppInfo = lAppInfo;
        //sWindow.LoadAppSettings();
    }


    private void OnGUI()
    {
        //Debug.Log("PATH RAW : " + mAppPath); 
        if (GUILayout.Button("Change existent XML"))
        {
            Refresh();
            EditorWindow.GetWindow(typeof(EditorSharedMenu));
            mChangeExistentXML = true;
            mCreateXML = false;
        }
        if(GUILayout.Button("Create a new XML"))
        {
            Refresh();
            EditorWindow.GetWindow(typeof(EditorSharedMenu));
            mCreateXML = true;
            mChangeExistentXML = false;
        }
        if(mCreateXML)
        {
            Debug.Log("CREATE XML");
            CreateXML();
        }
        else if (mChangeExistentXML)
        {
            Debug.Log("CHANGE XML");
            ChangeXML();
        }

    }

    void AddNewButton()
    {
        items.Add(new MenuItemObject());
    }

    void Remove(int iIndex)
    {
        items.RemoveAt(iIndex);
    }

    void BuildXML()
    {
        
        ///BYOS.Instance.Resources.GetPathToRaw(file)
        string lPath = mAppPath + "/Resources/Raw/XMLShared/Menu";
        //Debug.Log("BUILDING XML : " + lPath);
        Directory.CreateDirectory(lPath);

        if(File.Exists(lPath + "/" + nameOfXml + ".xml"))
        {
            File.Delete(lPath + "/" + nameOfXml + ".xml");
        }

        XmlWriterSettings lSettings = new XmlWriterSettings();
        lSettings.Indent = true;
        lSettings.OmitXmlDeclaration = true;
        lSettings.NewLineChars = "\r\n";
        lSettings.NewLineHandling = NewLineHandling.Replace;

        XmlWriter lWriter = XmlWriter.Create(lPath + "/" + nameOfXml + ".xml", lSettings);

        lWriter.WriteStartDocument();
        lWriter.WriteStartElement("List");
        lWriter.WriteElementString("ListSize", mListSize.ToString());
        for (int i = 0; i < mListSize; ++i)
        {
            lWriter.WriteStartElement("Button");
            lWriter.WriteElementString("Key", items[i].key);
            lWriter.WriteElementString("Trigger", items[i].trigger);
            lWriter.WriteElementString("QuitApp", items[i].quitApp.ToString());
            lWriter.WriteEndElement();
        }
        lWriter.WriteEndElement();
        lWriter.WriteEndDocument();
        lWriter.Close();

    }

    void CreateXML()
    {
        Debug.Log("ITEMS COUNT : " + items.Count);
        if (mClearItems)
        {
            mClearItems = false;
            items.Clear();
        }
        EditorGUILayout.LabelField("Define the list size with a number : ");
        mListSize = EditorGUILayout.DelayedIntField("List Size", mListSize);
        if (items.Count > mListSize)
        {
            for (int i = mListSize; i < items.Count - 1; ++i)
            {
                Remove(i);
            }
        }
        else
            mIsDrawCreateXML = false;
        EditorGUILayout.BeginVertical();
        mScrollView = EditorGUILayout.BeginScrollView(mScrollView);
        nameOfXml = EditorGUILayout.TextField("Name of the XML", nameOfXml);
        for (int i = 0; i < mListSize; ++i)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button number n : " + i);
            if(!mIsDrawCreateXML)
                AddNewButton();
            items[i].key = EditorGUILayout.TextField("my key : ", items[i].key);
            items[i].trigger = EditorGUILayout.TextField("Trigger to activate : ", items[i].trigger);
            items[i].quitApp = EditorGUILayout.Toggle("Does this button quit app : ", items[i].quitApp);
            if (GUILayout.Button("Delete this button"))
            {
                mListSize -= 1;
                Remove(i);
            }
            EditorGUILayout.Space();
        }
        mIsDrawCreateXML = true;
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Build XML"))
        {
            if (!string.IsNullOrEmpty(nameOfXml))
            {
                mCreateXML = false;
                BuildXML();
            }
                
            else
            {
                EditorUtils.LogE(LogContext.EDITOR, LogInfo.NOT_FOUND, "You forgot to put a name for your XML");
               
            }
                
        }
    }
    void DrawAfterChangeXML() 
    {
        EditorGUILayout.LabelField("Define the list size with a number : ");
        mListSize = EditorGUILayout.DelayedIntField("List Size", mListSize);
        if (items.Count > mListSize)
        {
            for (int i = mListSize ; i < items.Count - 1; ++i)
            {
                Remove(i);
            }
        }
        else
            mIsDraw = false;
        EditorGUILayout.BeginVertical();
        mScrollView = EditorGUILayout.BeginScrollView(mScrollView);
        
        for (int i = 0; i < mListSize; ++i)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button number n : " + i);
            if (!mIsDraw)
            {
                AddNewButton();
            }
            items[i].key = EditorGUILayout.TextField("my key : ", items[i].key);
            items[i].trigger = EditorGUILayout.TextField("Trigger to activate : ", items[i].trigger);
            items[i].quitApp = EditorGUILayout.Toggle("Does this button quit app : ", items[i].quitApp);
            if (GUILayout.Button("Delete this button"))
            {
                mListSize -= 1;
                Remove(i);
            }
            EditorGUILayout.Space();
        }
        mIsDraw = true;
        

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Build XML"))
        {
            if (!string.IsNullOrEmpty(nameOfXml))
            {
                //mCreateXML = false;

                BuildXML();

            }
        }
    }
    
    void ChangeXML()
    {
        mClearItems = true;
        string lPath = mAppPath + "/Resources/Raw/XMLShared/Menu";
        if (!mEnterClicked)
        {
            mIsDraw = false;
            EditorGUILayout.LabelField("Which XML do you want to change : ");
            nameOfXml = EditorGUILayout.TextField("Name of the XML", nameOfXml);
            if (GUILayout.Button("Change this XML"))
            {
                mEnterClicked = true;
            }
        }
        if (!string.IsNullOrEmpty(nameOfXml) && mEnterClicked)
        {
            if (File.Exists(lPath + "/" + nameOfXml + ".xml"))
            {
                XmlDocument lDoc = new XmlDocument();
                lDoc.Load(lPath + "/" + nameOfXml + ".xml");

                XmlElement lElmt = lDoc.DocumentElement;
                XmlNodeList lNodeList = lElmt.ChildNodes;

                //items = new List<MenuItemObject>(lNodeList.Count);
                if (!mChangeIndex)
                {
                    for (int i = 0; i < lNodeList.Count; ++i)
                    {
                        
                        //Debug.Log("LNODE LIST : " + lNodeList[i].Name);

                        if (lNodeList[i].Name == "ListSize")
                        {
                            EditorGUILayout.LabelField("Define the list size with a number : ");
                            mResult = int.TryParse(lNodeList[i].InnerText, out mValue);
                            if (mResult)
                                mListSize = mValue;

                        }
                        else if (lNodeList[i].Name == "Button")
                        {

                            AddNewButton();

                            items[i-1].key = lNodeList[i].SelectSingleNode("Key").InnerText;
                            items[i-1].trigger = lNodeList[i].SelectSingleNode("Trigger").InnerText;
                            items[i-1].quitApp = bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[i - 1].quitApp);
                            //PROBLEME ICI TOUJOURS TRUE
                            Debug.Log("LOL BOOL : " + bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[i - 1].quitApp));
                            //string lStringKey = lNodeList[i].SelectSingleNode("Key").InnerText;
                            //    string lStringTrigger = lNodeList[i].SelectSingleNode("Trigger").InnerText;
                            //    bool lBoolQuitApp = bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[i - 1].quitApp);
                            //    if (lNodeList[i].SelectSingleNode("Key") != null)
                            //        items[i - 1].key = EditorGUILayout.TextField("my key : ", lStringKey);
                            //    if (lNodeList[i].SelectSingleNode("Trigger") != null)
                            //        items[i - 1].trigger = EditorGUILayout.TextField("Trigger to activate : ",lStringKey);
                            //    if (bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[i - 1].quitApp))
                            //        items[i - 1].quitApp = EditorGUILayout.Toggle("Does this button quit app : ",lBoolQuitApp);

                        

                        //}

                        //items[i].key = EditorGUILayout.TextField("my key : ", lNodeList[i].SelectSingleNode("Key").InnerText);
                        //items[i].trigger = EditorGUILayout.TextField("Trigger to activate : ", lNodeList[i].SelectSingleNode("Trigger").InnerText);
                        //items[i].quitApp = EditorGUILayout.Toggle("Does this button quit app : ", bool.TryParse(lNodeList[i].SelectSingleNode("QuitApp").InnerText, out items[i].quitApp));
                        }

                        //XmlNode lXmlNode = lNodeList[i].SelectSingleNode("Key");
                        //if (lXmlNode != null)
                        //    Debug.Log(lXmlNode.InnerText);
                        //else if (lXmlNode == null)
                        //    Debug.Log("lolol");
                    }
                    mChangeIndex = true;
                }

                DrawAfterChangeXML();
            }
            //mChangeExistentXML = false;
        }
    }

    void Refresh()
    {
        mListSize = 0;
        nameOfXml = "";
    }

    protected override void RenderImpl()
    {

    }
}

