using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;
using System.Net.Sockets;

#endif

namespace BuddyApp.Call
{

#if UNITY_EDITOR

    [CustomEditor(typeof(OTONetwork))]
    public class OTONetworkTransportEditor : Editor
    {
        GUILayoutOption[] _layoutOption1;
        GUILayoutOption[] _layoutOption2;
        GUILayoutOption[] _layoutOption3;

        OTONetworkTransportEditor()
        {
            _layoutOption1 = new GUILayoutOption[1];
            _layoutOption1[0] = GUILayout.MaxWidth(50);

            _layoutOption2 = new GUILayoutOption[1];
            _layoutOption2[0] = GUILayout.MaxWidth(150);

            _layoutOption3 = new GUILayoutOption[1];
            _layoutOption3[0] = GUILayout.MaxWidth(100);
        }

        public override void OnInspectorGUI()
        {
            OTONetwork script = (OTONetwork)target;

            EditorGUI.BeginDisabledGroup(script.Communicating);

            script.IsServer = EditorGUILayout.Toggle("Server ", script.IsServer);

            if (!script.IsServer)
            {
                script.IP = EditorGUILayout.TextField("IP adress", script.IP);
            }
            script.Port = EditorGUILayout.IntField("Port", script.Port);

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(5) });

            if (GUILayout.Button("Reset channels"))
            {
                script.DeleteAllChannels();
            }

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

            for (int i = 0; i < script.NChannels; i++)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID : " + script.Channel_ids[i] + ", name :", _layoutOption3);
                script.Channel_name[i] = EditorGUILayout.TextField(script.Channel_name[i]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                script.Channel_type[i] = (QosType)EditorGUILayout.EnumPopup(script.Channel_type[i]);
                if (GUILayout.Button("X", _layoutOption1))
                {
                    script.DeleteAChannel(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Receiver", _layoutOption1);
                script.Channel_receivers[i] = (OTONetReceiver)EditorGUILayout.ObjectField(script.Channel_receivers[i], typeof(OTONetReceiver), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sender", _layoutOption1);
                script.Channel_senders[i] = (OTONetSender)EditorGUILayout.ObjectField(script.Channel_senders[i], typeof(OTONetSender), true);
                EditorGUILayout.EndHorizontal();


                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add a channel"))
            {
                int at = (int)script.AddAChannel("", QosType.Reliable, null, null);
                script.Channel_name[at] = "Channel " + (int)script.Channel_ids[at];
            }

            EditorGUI.EndDisabledGroup();

            if (GUI.changed)
                EditorUtility.SetDirty(script);
        }
    }
#endif
}