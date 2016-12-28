using UnityEngine;
using System.Collections;

//#if UNITY_EDITOR
//using UnityEditor;
//[CustomEditor(typeof(OccupancyGrid))]
//public class OccupancyGridDrawer : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        OccupancyGrid myScript = (OccupancyGrid)target;
//        if (GUILayout.Button("Build Object"))
//        {
//            myScript.BuildObject();
//        }
//    }
//    /*  const int rows = 3;
//      public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//      {
//          return base.GetPropertyHeight(property, label) * rows;
//      }
//      // Draw the property inside the given rect
//      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//      {
//          // Using BeginProperty / EndProperty on the parent property means that
//          // prefab override logic works on the entire property.
//          EditorGUI.BeginProperty(position, label, property);

//          // Draw label
//          // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//          position = EditorGUI.PrefixLabel(position, label);
//          // Don't make child fields be indented
//          var indent = EditorGUI.indentLevel;
//          EditorGUI.indentLevel = 0;
//          // Calculate rects
//           var nameresolutionRect = new Rect(position.x , position.y, 30, 16);
//           var resolutionRect = new Rect(position.x + 90, position.y, 30, 16);
//            var widthRect = new Rect(position.x + 90, position.y+16, 50, 16);
//            var heightRect = new Rect(position.x + 90, position.y+32, position.width - 90, 16);

//          // Draw fields - passs GUIContent.none to each so they are drawn without labels
//          EditorGUI.TextField(nameresolutionRect, "resolution");
//           EditorGUI.PropertyField(resolutionRect, property.FindPropertyRelative("mResolution"), GUIContent.none);
//           EditorGUI.PropertyField(widthRect, property.FindPropertyRelative("mWidth"), GUIContent.none);
//           EditorGUI.PropertyField(heightRect, property.FindPropertyRelative("mHeight"), GUIContent.none);
//          // Set indent back to what it was
//          EditorGUI.indentLevel = indent;
//          EditorGUI.EndProperty();
//      }*/
//}
//#endif


