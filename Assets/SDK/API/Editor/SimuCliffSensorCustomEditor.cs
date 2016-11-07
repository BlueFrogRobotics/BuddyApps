using UnityEngine;
using System.Collections;

namespace BuddyAPI
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Simulation.SimuCliffSensor))]
    public class SimuCliffSensorCustomEditor : Editor
    {
        void InspectorGUI(Simulation.SimuCliffSensor iTarget)
        {
            iTarget.MaxDistanceCliff = EditorGUILayout.Slider("MaxDistance", iTarget.MaxDistanceCliff, 0f, 1f);
            iTarget.MinDistanceCliff = EditorGUILayout.Slider("MinDistance", iTarget.MinDistanceCliff, 0f, 3f);
            iTarget.IsTriggerCliff = EditorGUILayout.Toggle("Is Trigger", iTarget.IsTriggerCliff);
            iTarget.InfinityIsOneCliff = EditorGUILayout.Toggle("Infinty Is One", iTarget.InfinityIsOneCliff);
        }

        public override void OnInspectorGUI()
        {
            Simulation.SimuCliffSensor lTarget = (Simulation.SimuCliffSensor)target;
            InspectorGUI(lTarget);

            if (GUI.changed)
                EditorUtility.SetDirty(lTarget);
        }
    }
#endif
}

