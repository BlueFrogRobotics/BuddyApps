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
            target.MaxDistanceUS = EditorGUILayout.Slider("MaxDistance", target.MaxDistanceUS, 0f, 1f);
            target.MinDistanceUS = EditorGUILayout.Slider("MinDistance", target.MinDistanceUS, 0f, 3f);
            target.StepUS = EditorGUILayout.FloatField("Step", target.StepUS);
            target.ApertureSizeUS = EditorGUILayout.Vector3Field("Aperture", target.ApertureSizeUS);
            target.InfinityIsZeroUS = EditorGUILayout.Toggle("Infinity is Zero", target.InfinityIsZeroUS);
        }

        public override void OnInspectorGUI()
        {
            Simulation.SimuUSSensor mTarget = (Simulation.SimuUSSensor)target;
            InspectorGUI(mTarget);
            if (GUI.changed)
                EditorUtility.SetDirty(mTarget);
        }
    }
#endif
}

