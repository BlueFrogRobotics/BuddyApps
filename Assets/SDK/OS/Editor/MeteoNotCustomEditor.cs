using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MeteoNot))]
    public class MeteoNotCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}