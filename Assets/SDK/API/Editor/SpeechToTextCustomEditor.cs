using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BuddyAPI.SpeechToText))]
public class SpeechToTextCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
    }
}

#endif