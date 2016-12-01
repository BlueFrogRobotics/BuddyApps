using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MessageNot))]
    public class MessageNotCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}