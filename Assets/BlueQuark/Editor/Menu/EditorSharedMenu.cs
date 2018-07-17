using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
//using Buddy;
//using Buddy.Internal.System;
//using Buddy.Internal.AppManagement;
//using Buddy.Editor;
using BlueQuark.Editor;
using BlueQuark;

    public class EditorSharedMenu : AWindow {
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
        Debug.Log("LOADAPP");
        mAppPath = iApplicationPath;
        if(string.IsNullOrEmpty(mAppPath))
        {
            EditorUtils.LogE(LogContext.EDITOR, LogInfo.NOT_FOUND, "Path is empty");
        }
        else
        {
            EditorWindow.GetWindow(typeof(EditorSharedMenu));
        }
    }


    private void OnGUI()
    {
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
        string lPath = mAppPath + "/Resources/Raw/XMLShared/Menu";
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
            Debug.Log("BUILD XML");
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
                if (!mChangeIndex)
                {
                    for (int i = 0; i < lNodeList.Count; ++i)
                    {
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
                        }
                        
                    }
                    mChangeIndex = true;
                }

                DrawAfterChangeXML();
            }
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

