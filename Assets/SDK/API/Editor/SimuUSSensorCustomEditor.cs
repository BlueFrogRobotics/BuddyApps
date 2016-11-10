using UnityEngine;
using System.Collections;

namespace BuddyAPI
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(Simulation.SimuUSSensor))]
    public class SimuUSSensorCustomEditor : Editor
    {
        void InspectorGUI(Simulation.SimuUSSensor target)
        {
            target.MaxDistanceUS = EditorGUILayout.Slider("MaxDistance", target.MaxDistanceUS, 0F, 1F);
            target.MinDistanceUS = EditorGUILayout.Slider("MinDistance", target.MinDistanceUS, 0F, 3F);
            target.StepUS = EditorGUILayout.FloatField("Step", target.StepUS);
            target.ApertureSizeUS = EditorGUILayout.Vector3Field("Aperture", target.ApertureSizeUS);
            target.InfinityIsZeroUS = EditorGUILayout.Toggle("Infinity is Zero", target.InfinityIsZeroUS);
        }

        public override void OnInspectorGUI()
        {
            Simulation.SimuUSSensor lTarget = (Simulation.SimuUSSensor)target;
            InspectorGUI(lTarget);
            if (GUI.changed)
                EditorUtility.SetDirty(lTarget);
        }
    }
#endif
}

