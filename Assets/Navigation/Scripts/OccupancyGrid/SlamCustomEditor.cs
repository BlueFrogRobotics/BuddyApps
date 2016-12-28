using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OccupancyGrid))]
public class SlamCustomEditor : Editor
{
    SerializedProperty slam;
    float value;
     void OnEnable()
     {
        slam = serializedObject.FindProperty("OccupancyGrid");
     }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(slam);
        serializedObject.ApplyModifiedProperties();
       // Slam ltarget = (Slam)target;
       // GUILayout.BeginVertical();
       // GUILayout.Label("Occupancy Grid settings");
       // GUILayout.Label("   Resolution : "+ ltarget.OccupancyGrid.Resolution);
      //  GUILayout.Label("   Width : " + ltarget.OccupancyGrid.Width);
      //  GUILayout.Label("   Height : " + ltarget.OccupancyGrid.Height);
      //  GUILayout.Label("   tester : " + ltarget.OccupancyGrid.tester);

      //  ltarget.OccupancyGrid.tester = EditorGUILayout.FloatField(ltarget.OccupancyGrid.tester);
        //ltarget.OccupancyGrid.tester = value;
       // mSlam.OccupancyGrid.Resolution = float.Parse( GUILayout.TextField("Resolution"));

      //  GUILayout.EndVertical();

       // GUILayout.Space(20);
       // base.OnInspectorGUI();
       //EditorUtility.SetDirty(ltarget);
    }
}
#endif