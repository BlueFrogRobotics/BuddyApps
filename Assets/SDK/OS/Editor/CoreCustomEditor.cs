using UnityEngine;
using System.Collections;

namespace BuddyOS.Impl
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Core))]
    public class CoreCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}