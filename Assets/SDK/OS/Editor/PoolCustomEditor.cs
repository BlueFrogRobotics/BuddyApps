using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Pool))]
    public class PoolCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}