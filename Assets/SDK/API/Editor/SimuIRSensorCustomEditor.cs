using UnityEngine;
using System.Collections;

namespace BuddyAPI
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Simulation.SimuIRSensor))]
    public class SimuIRSensorCustomEditor : Editor
    {
        void InspectorGUI(Simulation.SimuIRSensor iTarget)
        {
            iTarget.MaxDistanceIR = EditorGUILayout.Slider("MaxDistance", iTarget.MaxDistanceIR, 0f, 1f);
            iTarget.MinDistanceIR = EditorGUILayout.Slider("MinDistance", iTarget.MinDistanceIR, 0f, 3f);
            iTarget.InfinityIsZeroIR = EditorGUILayout.Toggle("Infinity is Zero", iTarget.InfinityIsZeroIR);
        }

        public override void OnInspectorGUI()
        {
            Simulation.SimuIRSensor lTarget = (Simulation.SimuIRSensor)target;
            InspectorGUI(lTarget);
            if (GUI.changed)
                EditorUtility.SetDirty(lTarget);
        }
    }
#endif
}
