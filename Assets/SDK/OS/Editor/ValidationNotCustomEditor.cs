using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(ValidationNot))]
    public class ValidationNotCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}