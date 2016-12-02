using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SimpleNot))]
    public class SimpleNotCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}