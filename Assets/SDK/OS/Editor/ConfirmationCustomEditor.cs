using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(ConfirmationNot))]
    public class ConfirmationCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}