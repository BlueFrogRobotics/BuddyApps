using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorSharedBinaryQuestion : EditorWindow {



    [MenuItem ("Buddy/Shared/Binary Question")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorSharedBinaryQuestion));
    }

    private void OnGUI()
    {
        
    }
}
