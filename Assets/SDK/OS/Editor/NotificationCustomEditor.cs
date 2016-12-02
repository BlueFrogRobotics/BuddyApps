using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(NotificationManager))]
    public class NotificationCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}