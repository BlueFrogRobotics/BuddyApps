using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TimerNot))]
    public class TimerNotCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}