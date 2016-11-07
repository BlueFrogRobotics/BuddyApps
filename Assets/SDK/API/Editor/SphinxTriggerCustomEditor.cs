using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BuddyAPI.SphinxTrigger))]
public class SphinxTriggerCustomEditor : Editor {
    public override void OnInspectorGUI()
    {
    }
}
#endif
