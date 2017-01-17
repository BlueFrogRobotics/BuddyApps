using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BuddyOS;
using BuddyTools;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class DictionaryEditor : EditorWindow
{
    private Dictionary<Language, LanguageThesaurus> mDictionaries;
    private Dictionary<int, Language> mIndexToDict;
    private List<List<DictionaryEntry>> mAllEntries;
    private List<string> mEditingFiles;
    private string mPathToDictionaries;
    private string mPathToCSV;
    private bool mHasLoad;
    private Vector2 mScrollPosition;

    [MenuItem("Buddy/Dictionary")]
    static void Init()
    {
        GetWindow(typeof(DictionaryEditor));
    }

    void OnEnable()
    {
        if (mDictionaries == null) {
            mDictionaries = new Dictionary<Language, LanguageThesaurus>();
            mEditingFiles = new List<string>();
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Your dictionaries to edit", EditorStyles.boldLabel);
        GUILayout.Space(5);
        if (GUILayout.Button("Select your dictionaries folder")) {
            mPathToDictionaries = EditorUtility.OpenFolderPanel("Select your dictionaries Lang folder", "Assets", "Lang");
            if (mPathToDictionaries != string.Empty && mPathToDictionaries != null) {
                mDictionaries.Clear();
                mEditingFiles.Clear();
                if (mAllEntries != null)
                    mAllEntries.Clear();
                foreach (string lFile in Directory.GetFiles(mPathToDictionaries)) {
                    string lRelativeFile = lFile.Substring(lFile.IndexOf("Assets/"));
                    string lFileLower = lFile.ToLower();
                    if (lFileLower.Contains(".asset") && !lFileLower.Contains(".meta")) {
                        mEditingFiles.Add(lRelativeFile);
                        if (lFileLower.Contains("french"))
                            mDictionaries.Add(Language.FRA, AssetDatabase.LoadAssetAtPath<LanguageThesaurus>(lRelativeFile));
                        else if (lFileLower.Contains("english")) {
                            mDictionaries.Add(Language.ENG, AssetDatabase.LoadAssetAtPath<LanguageThesaurus>(lRelativeFile));
                        }
                    }
                }
                mHasLoad = true;
                mPathToDictionaries = null;
            }
        }

        if (mHasLoad) {
            GUILayout.BeginVertical();

            int lNbEntry = mEditingFiles.Count;
            for (int i = 0; i < lNbEntry; ++i)
                GUILayout.Label("Editing : " + mEditingFiles[i]);

            GUILayout.Space(10);

            if (GUILayout.Button("Load TSV")) {
                mPathToCSV = EditorUtility.OpenFilePanel("Select your csv file", "", "tsv");
                if (mPathToCSV != null && mPathToCSV != string.Empty) {
                    List<string[]> lContent = SerializeCSV.Load(mPathToCSV, '\t');
                    Dictionary<int, Language> lColToLang = new Dictionary<int, Language>();
                    Dictionary<string, Language> lStringToLang = new Dictionary<string, Language>() {
                        { "FRA", Language.FRA },
                        { "ENG", Language.ENG }
                    };

                    foreach (string[] lLine in lContent) {
                        if (lLine.Length == 0)
                            continue;

                        if (lLine[0] == "TOKEN" || lLine[0] == "KEY") {
                            for (int lCol = 1; lCol < lLine.Length; ++lCol)
                                lColToLang.Add(lCol, lStringToLang[lLine[lCol]]);
                            continue;
                        }

                        if (lLine[0] != string.Empty) {
                            string lKey = lLine[0];
                            for (int lCol = 1; lCol < lLine.Length; ++lCol) {
                                Language lVal = lColToLang[lCol];
                                LanguageThesaurus lThes = mDictionaries[lVal];
                                if (!lThes.ContainsKey(lKey))
                                    lThes.Entries.Add(new DictionaryEntry() {
                                        Key = lKey,
                                        BaseValue = lLine[lCol],
                                        RandomValues = new List<string>() { "-random-" },
                                        ClosePhoneticValues = new List<string>() { "-phonetic-" }
                                    });
                            }
                        }
                    }
                    mPathToCSV = null;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("String entries", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (mAllEntries == null) {
                mAllEntries = new List<List<DictionaryEntry>>();
                mIndexToDict = new Dictionary<int, Language>();
                int lIndexDict = 0;
                foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries) {
                    mIndexToDict.Add(lIndexDict++, lThes.Key);
                    mAllEntries.Add(lThes.Value.Entries);
                }
            }

            int lNbDic = mAllEntries.Count;
            if (lNbDic > 0) {

                int lNbEntries = mAllEntries[0].Count;

                if (lNbEntries > 0) {

                    mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);

                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical();
                    GUILayout.Label("key");
                    for (int j = 0; j < lNbEntries; ++j) {
                        GUILayout.Space(19);
                        DictionaryEntry lEntry = mAllEntries[0][j];
                        string lNewKey = GUILayout.TextField(lEntry.Key);
                        if (lNewKey != lEntry.Key)
                            for (int i = 0; i < lNbDic; ++i)
                                mAllEntries[i][j].Key = lNewKey;
                        GUILayout.Space(26);
                    }
                    GUILayout.EndVertical();

                    for (int i = 0; i < lNbDic; ++i) {
                        GUILayout.BeginVertical();
                        GUILayout.Label(mIndexToDict[i].ToString());
                        List<DictionaryEntry> lEntries = mAllEntries[i];
                        for (int j = 0; j < lNbEntries; ++j) {

                            DictionaryEntry lEntry = lEntries[j];
                            string lSayEntries = Utils.CollectionToString(lEntry.RandomValues, "/");
                            string lListenEntries = Utils.CollectionToString(lEntry.ClosePhoneticValues, "/");

                            lEntry.BaseValue = GUILayout.TextField(lEntry.BaseValue);
                            lEntry.RandomValues = new List<string>(GUILayout.TextField(lSayEntries).Split('/'));
                            lEntry.ClosePhoneticValues = new List<string>(GUILayout.TextField(lListenEntries).Split('/'));

                            GUILayout.Space(9);
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndScrollView();
                }
            }

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add string"))
                foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                    lThes.Value.Entries.Add(new DictionaryEntry() {
                        Key = "-key-",
                        BaseValue = "-base-",
                        RandomValues = new List<string>() { "-random-" },
                        ClosePhoneticValues = new List<string>() { "-phonetic-" }
                    });

            if (GUILayout.Button("Remove last"))
                foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                    lThes.Value.Entries.RemoveAt(lThes.Value.Entries.Count - 1);

            GUILayout.EndHorizontal();

            foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                EditorUtility.SetDirty(lThes.Value);
        }
    }
}
#endif
