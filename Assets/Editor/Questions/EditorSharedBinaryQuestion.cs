using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Buddy.Internal.AppManagement;
using Buddy.Editor;
using Buddy;
using System.Xml;
using System.IO;

public class EditorSharedBinaryQuestion : AWindow
{
    public class QuestionItemObject
    {
        public string key;
        public string trigger;
    }

    private List<QuestionItemObject> items = new List<QuestionItemObject>(0);

    private static string mAppPath;
    private bool mChangeExistentXML;
    private bool mCreateXML;
    private string nameOfXml;
    private int mListSize;
    private bool mIsDrawCreateXML = false;
    private Vector2 mScrollView;
    private bool mEnterClicked;
    private bool mResult;
    private bool mChangeIndex;
    private int mValue;
    private bool mIsDraw = true;
    private bool mClearItems;

    [MenuItem ("Buddy/Shared/Question Manager")]
    public static void ShowWindow()
    {
        SelectAppPopup.Show(LoadApp);
    }

    private void OnEnable()
    {
        mChangeExistentXML = false;
        mCreateXML = false;
        mChangeIndex = false;
    }

    private static void LoadApp(string iApplicationPath)
    {
        mAppPath = iApplicationPath;

        if (string.IsNullOrEmpty(mAppPath))
        {
            EditorUtils.LogE(LogContext.EDITOR, LogInfo.NOT_FOUND, "Path is empty");
        }
        else
        {
            EditorWindow.GetWindow(typeof(EditorSharedBinaryQuestion));
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Change existent XML"))
        {
            Refresh();
            EditorWindow.GetWindow(typeof(EditorSharedBinaryQuestion));
            mChangeExistentXML = true;
            mCreateXML = false;
        }
        if (GUILayout.Button("Create a new XML"))
        {
            Refresh();
            EditorWindow.GetWindow(typeof(EditorSharedBinaryQuestion));
            mCreateXML = true;
            mChangeExistentXML = false;
        }
        if (mCreateXML)
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

    void BuildXML()
    {
        string lPath = mAppPath + "/Resources/Raw/XMLShared/Question";
        Directory.CreateDirectory(lPath);

        if (File.Exists(lPath + "/" + nameOfXml + ".xml"))
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
            lWriter.WriteEndElement();
        }
        lWriter.WriteEndElement();
        lWriter.WriteEndDocument();
        lWriter.Close();

    }

    private void CreateXML()
    {
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
        if(mListSize > 5)
        {
            mListSize = 5;
        }
        EditorGUILayout.BeginVertical();
        mScrollView = EditorGUILayout.BeginScrollView(mScrollView);
        nameOfXml = EditorGUILayout.TextField("Name of the XML", nameOfXml);
        for (int i = 0; i < mListSize; ++i)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button number n : " + i);
            if (!mIsDrawCreateXML)
                AddNewButton();
            items[i].key = EditorGUILayout.TextField("my key : ", items[i].key);
            items[i].trigger = EditorGUILayout.TextField("Trigger to activate : ", items[i].trigger);
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
    private void DrawAfterChangeXML()
    {
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

    private void ChangeXML()
    {
        mClearItems = true;
        string lPath = mAppPath + "/Resources/Raw/XMLShared/Question";
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
                            items[i - 1].key = lNodeList[i].SelectSingleNode("Key").InnerText;
                            items[i - 1].trigger = lNodeList[i].SelectSingleNode("Trigger").InnerText;
                        }
                    }
                    mChangeIndex = true;
                }
                DrawAfterChangeXML();
            }
        }
    }

    protected override void RenderImpl()
    {

    }

    void AddNewButton()
    {
        items.Add(new QuestionItemObject());
    }

    void Remove(int iIndex)
    {
        items.RemoveAt(iIndex);
    }

    private void Refresh()
    {
        mListSize = 0;
        nameOfXml = "";
    }
}
