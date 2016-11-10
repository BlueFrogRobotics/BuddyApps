﻿using UnityEngine;
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

            mOS.EnableCommandLogs = EditorGUILayout.Toggle("Command logs", mOS.EnableCommandLogs);
            mOS.EnableNetworkLogs = EditorGUILayout.Toggle("Network logs", mOS.EnableNetworkLogs);
            EditorGUILayout.Space();
            mOS.EnableLoadDefaultApp = EditorGUILayout.BeginToggleGroup("Start with app", mOS.EnableLoadDefaultApp);
            mOS.DefaultApp = EditorGUILayout.TextField("    Scene name", mOS.DefaultApp);
            EditorGUILayout.EndToggleGroup();

            if (GUI.changed)
                EditorUtility.SetDirty(mOS);
        }
    }
#endif
}