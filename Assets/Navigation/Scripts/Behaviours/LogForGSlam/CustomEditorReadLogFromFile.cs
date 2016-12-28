using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ReadLogFromFile))]
public class CustomEditorReadLogFromFile : Editor
{

    public override void OnInspectorGUI()
    {
        ReadLogFromFile mObject = (ReadLogFromFile)target;
        base.OnInspectorGUI();
        if (mObject.FileIsLoading)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
            {
                mObject.ActualPLayerMode = ReadLogFromFile.PlayerMode.PLAY;
            }
            if (GUILayout.Button("Pause"))
            {
                mObject.ActualPLayerMode = ReadLogFromFile.PlayerMode.PAUSE;
            }
            if (GUILayout.Button("Stop"))
            {
                mObject.ActualPLayerMode = ReadLogFromFile.PlayerMode.STOP;
                mObject.RowToGet = 0;
                mObject.mPurcentOfData = (int)(((float)mObject.RowToGet / (float)mObject.numberLines) * 100.0f);
            }
            GUILayout.EndHorizontal();
            mObject.RowToGet = (int)GUILayout.HorizontalSlider(mObject.RowToGet, 0, mObject.numberLines - 1);
            if (GUILayout.Button("Save"))
            {
                mObject.saveAsJPEG();
            }
        }
        else {
            if (GUILayout.Button("Load from file"))
            {
                mObject.readAllLines();
            }
        }

    }

}
#endif