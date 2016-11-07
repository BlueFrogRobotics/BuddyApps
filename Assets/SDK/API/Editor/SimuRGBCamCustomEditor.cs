using UnityEngine;
using System.Collections;

namespace BuddyAPI
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Simulation.SimuInputRGBCam))]
    public class SimuRGBCamCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Simulation.SimuInputRGBCam mTarget = (Simulation.SimuInputRGBCam)target;
            DrawDefaultInspector();
            if (GUI.changed)
                EditorUtility.SetDirty(mTarget);
        }
    }
#endif
}
