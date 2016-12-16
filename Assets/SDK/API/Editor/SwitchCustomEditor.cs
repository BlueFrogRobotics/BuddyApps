using UnityEngine;

namespace BuddyAPI.Switch
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Switch))]
    public class SwitchCustomEditor : Editor
    {
        private Switch mSwitch;

        public override void OnInspectorGUI()
        {
            mSwitch = (Switch)target;

            CreateButton(InstanceType.REAL);
            CreateButton(InstanceType.SIMU);
            CreateButton(InstanceType.NULL);

            DrawDefaultInspector();

            if (GUI.changed)
                EditorUtility.SetDirty(mSwitch);
        }

        private void CreateButton(InstanceType iType)
        {
            if (GUILayout.Button(iType.ToString())) {
                if (mSwitch.Simulation != null)
                    mSwitch.Simulation.SetActive(iType == InstanceType.SIMU);
                foreach (AComponent lComponent in mSwitch.Components) {
                    lComponent.InstanceType = iType;
                    EditorUtility.SetDirty(lComponent);
                }
                if (iType == InstanceType.SIMU) {
                    mSwitch.Render.gameObject.SetActive(true);
                    mSwitch.Regular.renderMode = RenderMode.ScreenSpaceCamera;
                } else {
                    mSwitch.Render.gameObject.SetActive(false);
                    mSwitch.Regular.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }
        }
    }
#endif
}