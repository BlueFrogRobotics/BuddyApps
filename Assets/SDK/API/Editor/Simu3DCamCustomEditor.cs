using System.Collections;
using UnityEngine;

namespace BuddyAPI
{

#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Simulation.SimuInputDepthCam))]
    public class Simu3DCamCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Simulation.SimuInputDepthCam mTarget = (Simulation.SimuInputDepthCam)target;
            DrawDefaultInspector();
            if (GUI.changed)
                EditorUtility.SetDirty(mTarget);
        }
    }
#endif
}