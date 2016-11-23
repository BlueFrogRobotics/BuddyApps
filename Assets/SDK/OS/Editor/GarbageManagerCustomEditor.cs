using UnityEngine;
using System.Collections;

namespace BuddyOS.System
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(GarbageManager))]
    public class GarbageManagerCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}