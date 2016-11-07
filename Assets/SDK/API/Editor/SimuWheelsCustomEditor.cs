using UnityEngine;
using System.Collections;

namespace BuddyAPI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BuddyAPI.Simulation.SimuWheels))]
    public class SimuWheelsCustomEditor : Editor
    {
        private Simulation.SimuWheels mTarget;
        public override void OnInspectorGUI()
        {
            mTarget = (Simulation.SimuWheels)target;

            mTarget.LinksBool = EditorGUILayout.Foldout(mTarget.LinksBool, "Links");
            if (mTarget.LinksBool)
                DrawDefaultInspector();

            if (GUI.changed)
                EditorUtility.SetDirty(mTarget);
        }
    }
#endif
}
