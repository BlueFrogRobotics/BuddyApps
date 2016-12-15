using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BuddyOS;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class DictionaryEditor : EditorWindow
{
    private Dictionary<Language, LanguageThesaurus> mDictionaries;
    private List<List<DictionaryEntry>> mAllEntries;
    private List<string> mEditingFiles;
    private string mPathToDictionaries;
    private bool mHasLoad;

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
        if (GUILayout.Button("Select your dictionaries folder"))
            mPathToDictionaries = EditorUtility.OpenFolderPanel("Select your dictionaries folder", "Assets", "");

        if (GUILayout.Button("Load dictionaries") && mPathToDictionaries != string.Empty) {

            foreach (string lFile in Directory.GetFiles(mPathToDictionaries)) {
                string lRelativeFile = lFile.Substring(lFile.IndexOf("Assets/"));
                string lFileLower = lFile.ToLower();
                if (lFileLower.Contains(".asset") && !lFileLower.Contains(".meta")) {
                    mEditingFiles.Add(lRelativeFile);
                    if (lFileLower.Contains("french"))
                        mDictionaries.Add(Language.FRA, AssetDatabase.LoadAssetAtPath<LanguageThesaurus>(lRelativeFile));
                    else if (lFileLower.Contains("anglais") || lFileLower.Contains("english")) {
                        mDictionaries.Add(Language.ENG, AssetDatabase.LoadAssetAtPath<LanguageThesaurus>(lRelativeFile));
                    }
                }
            }
            mHasLoad = true;
        }

        if (mHasLoad) {
            GUILayout.BeginVertical();

            int lNbEntry = mEditingFiles.Count;
            for (int i = 0; i < lNbEntry; ++i)
                GUILayout.Label("Editing : " + mEditingFiles[i]);

            GUILayout.Space(10);
            GUILayout.Label("String entries", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("key");
            if (mAllEntries == null) {
                mAllEntries = new List<List<DictionaryEntry>>();
                foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                    mAllEntries.Add(lThes.Value.Entries);
            }

            foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                GUILayout.Label(lThes.Key.ToString());

            GUILayout.EndHorizontal();

            int lNbDic = mAllEntries.Count;
            if (lNbDic > 0) {
                int lNbEntries = mAllEntries[0].Count;
                if (lNbEntries > 0) {
                    for (int i = 0; i < lNbEntries; ++i) {
                        GUILayout.BeginHorizontal();
                        string lKey = GUILayout.TextField(mAllEntries[0][i].Key);
                        for (int j = 0; j < lNbDic; ++j) {
                            List<DictionaryEntry> lEntries = mAllEntries[j];
                            DictionaryEntry lEntry = lEntries[i];
                            lEntry.Key = lKey;
                            lEntry.Value = GUILayout.TextField((string)lEntry.Value);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();

            if (GUILayout.Button("Add string")) {
                foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries) {
                    lThes.Value.Entries.Add(new DictionaryEntry() { Key = "key", Value = "txt" });
                }
            }

            foreach (KeyValuePair<Language, LanguageThesaurus> lThes in mDictionaries)
                EditorUtility.SetDirty(lThes.Value);
        }
    }
}
#endif
