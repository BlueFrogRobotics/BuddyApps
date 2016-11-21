using UnityEngine;
using System.Collections;

namespace BuddyOS.UI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MenuPanel))]
    public class MenuPanelCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}