using UnityEngine;
using System.Collections;

namespace BuddyOS.System
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(AnomalySearcher))]
    public class AnomalySearcherCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}