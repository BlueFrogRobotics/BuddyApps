using UnityEngine;
using System.Collections;

namespace BuddyOS
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BYOS))]
    public class BYOSCustomEditor : Editor
    {
        private BYOS mOS;

        public override void OnInspectorGUI()
        {
            mOS = (BYOS)target;

            mOS.EnableLogs = EditorGUILayout.Toggle("Enable logs", mOS.EnableLogs);
            EditorGUILayout.Space();
            mOS.EnableLoadApps = EditorGUILayout.BeginToggleGroup("Load apps", mOS.EnableLoadApps);
            mOS.EnableLoadDefaultApp = EditorGUILayout.BeginToggleGroup("   Start with app", mOS.EnableLoadDefaultApp);
            mOS.DefaultApp = EditorGUILayout.TextField("            Scene name", mOS.DefaultApp);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndToggleGroup();

            if (GUI.changed)
                EditorUtility.SetDirty(mOS);
        }
    }
#endif
}