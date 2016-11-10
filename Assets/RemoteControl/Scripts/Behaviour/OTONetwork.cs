using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;

#if UNITY_EDITOR

using UnityEditor;
using System.Net.Sockets;

#endif

namespace BuddyApp.Remote
{

    public class OTONetSender : MonoBehaviour
    {
        public delegate void ToSendEventHandler(OTONetSender sender, byte[] data, int ndata);
        private event ToSendEventHandler ToSendEvent;

        public void SendData(byte[] data, int ndata)
        {
            if (ToSendEvent == null) throw new System.InvalidOperationException("Send event has not been defined");
            else ToSendEvent(this, data, ndata);
        }

        public void Subscribe(ToSendEventHandler Event)
        {
            ToSendEvent += Event;
        }

        public void Unsubscribe(ToSendEventHandler Event)
        {
            ToSendEvent -= Event;
        }
    }

    public class OTONetReceiver : MonoBehaviour
    {
        public virtual void ReceiveData(byte[] data, int ndata)
        {
            throw new System.NotImplementedException("ReceiveData not implemented");
        }
    }

    public class OTONetwork : MonoBehaviour
    {

        public string IP;
        public int Port;

        public List<int> channel_ids;
        public List<string> channel_name;
        public List<QosType> channel_type;
        public List<OTONetSender> channel_senders;
        public List<OTONetReceiver> channel_receivers;

        public bool Communicating { get; protected set; }
        public bool HasAPeer { get; protected set; }
        public int NChannels { get { return channel_ids.Count; } }

        public bool IsServer;
        public bool IsClient { get { return !IsServer; } }

        private const int MaxFramePerUpdate = 10;

        private ConnectionConfig _config = new ConnectionConfig();
        private GlobalConfig _gConfig = new GlobalConfig();
        private int _genericHostId;
        private int _distantHostID;
        private int _distantConnectionID;
        private bool _isCorou = false;
        private bool _isconnect = false;
        static ushort bufferSize = 15000;
        byte[] recBuffer = new byte[bufferSize];

        IEnumerator UpdateOto()
        {
            while (true)
            {
                if (bufferSize < (ushort.MaxValue - 1000) && _isconnect)
                    bufferSize = ushort.MaxValue - 1;
                if (bufferSize >= (ushort.MaxValue - 1000) && _isconnect && !HasAPeer)
                {
                    recBuffer = new byte[bufferSize];
                    HasAPeer = true;
                }
                receiveData();
                yield return null;
            }
        }

        void Update()
        {
            if (!Communicating)
                return;
            else if (!_isCorou)
            {
                StartCoroutine("UpdateOto");
                _isCorou = true;
            }
        }

        void Start()
        {
            configure();
            connect();
        }


        void OnDisable()
        {
            if(_isconnect)
                disconnect();
        }


        private void configure()
        {
            _config = new ConnectionConfig();

            // Verify the number of each lists
            if (channel_name.Count != NChannels
                || channel_type.Count != NChannels
                || channel_senders.Count != NChannels
                || channel_receivers.Count != NChannels)
            {
                throw new System.IndexOutOfRangeException("Counter of channels lists are not equal");
            }

            // Fill config
            string str = "";
            for (int i = 0; i < NChannels; i++)
            {
                channel_ids[i] = _config.AddChannel(channel_type[i]);
                if (channel_senders[i] != null)
                {
                    channel_senders[i].Subscribe(ToSendEvent);
                }

                str += "Configure (" + i + ")";
                str += "\nid = " + channel_ids[i];
                str += "\nname = " + channel_name[i];
                str += "\ntype = " + channel_type[i];
                if (channel_receivers[i] != null) str += "\nreceiver = " + channel_receivers[i].gameObject.name;
                if (channel_senders[i] != null) str += "\nsender = " + channel_senders[i].gameObject.name;
                str += "\n\n";
            }
            Debug.Log(str);

            _config.PacketSize = ushort.MaxValue;
            _config.MaxSentMessageQueueSize = 1024;
            // Global config
            _gConfig = new GlobalConfig();
            _gConfig.MaxPacketSize = 65000;
            _gConfig.ReactorMaximumReceivedMessages = 1;//valeur initiale a 256
            _gConfig.ReactorMaximumSentMessages = 1;//valeur initiale a 256
        }

        private void connect()
        {
            if (Communicating) disconnect();


            if (IsServer)
            {
                NetworkTransport.Init(_gConfig);
                HostTopology topology = new HostTopology(_config, 1);

                _genericHostId = NetworkTransport.AddHost(topology, Port, null);

            }
            else
            {

                NetworkTransport.Init(_gConfig);

                HostTopology topology = new HostTopology(_config, 1);
                _genericHostId = NetworkTransport.AddHost(topology, 0);
                byte error;
                _distantConnectionID = NetworkTransport.Connect(_genericHostId, IP, Port, 0, out error);
                if (error != 0)
                {
                    throw new System.Exception("Error on connection : " + error);
                }
            }
            Communicating = true;

            //Debug.Log("Connected");
        }

        void disconnect()
        {
            NetworkTransport.RemoveHost(_genericHostId);
            NetworkTransport.Shutdown();
            foreach (OTONetSender otosender in channel_senders)
            {
                if (otosender != null) otosender.Unsubscribe(ToSendEvent);
            }
            Communicating = false;
            HasAPeer = false;
            _isconnect = false;
            bufferSize = 15000;
            _isCorou = false;            

            Debug.Log("OTONetwork Disconnected");
        }

        void receiveData()
        {
            int recChannelID;
            byte error;
            int dataSize;
            int count = 0;
            while (count++ < MaxFramePerUpdate)
            {
                NetworkEventType recData = NetworkTransport.Receive(out _distantHostID, out _distantConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);
                if (error != 0)
                    Debug.Log("Error = " + ((NetworkError)error).ToString() + " from distantHostID " + _distantConnectionID + " on distantConnectionID " + _distantConnectionID + " on recChannelID " + recChannelID);
                if (dataSize >= recBuffer.Length)
                {
                    //Debug.Log("Over Recbuffer : \nRecBuffer = " + recBuffer.Length + " Chanel = " + recChannelID + " Data size = " + dataSize);
                    return;
                }
                switch (recData)
                {

                    case NetworkEventType.Nothing:
                        return;

                    case NetworkEventType.ConnectEvent:
                        //Debug.Log(String.Format("Connect from host {0} connection {1}", _distantHostID, _distantConnectionID));
                        _isconnect = true;
                        break;

                    case NetworkEventType.DataEvent:
                        HasAPeer = true;
                        for (int i = 0; i < NChannels; i++)
                        {
                            if (channel_ids[i] == recChannelID)
                            {
                                //  Debug.Log("recBufferSize = " + recBuffer.Length + "dataSize" + dataSize + " chanel" + recChannelID);
                                channel_receivers[i].ReceiveData(recBuffer, dataSize);
                            }
                        }
                        break;

                    case NetworkEventType.DisconnectEvent:
                        //HasAPeer = false;
                        //_isconnect = false;
                        //bufferSize = 15000;
                        //_isCorou = false;
                        /*if (!IsServer)
                            Debug.Log(String.Format("DisConnect from host {0} connection {1}", _distantHostID, _distantConnectionID));*/
                        //BuddyOS.LoadAppBySceneCmd.Create("CompanionApp").Execute();
                        HomeCmd.Create().Execute();

                        break;
                }
            }
        }


        public int AddAChannel(string name, QosType type, OTONetReceiver otontr, OTONetSender otonts)
        {
            int id = _config.AddChannel(type);
            channel_ids.Add(id);
            channel_type.Add(type);
            channel_name.Add(name);
            channel_receivers.Add(otontr);
            channel_senders.Add(otonts);

            return channel_ids.Count - 1;
        }

        public void DeleteAChannel(int at)
        {
            channel_ids.RemoveAt(at);
            channel_type.RemoveAt(at);
            channel_name.RemoveAt(at);
            channel_receivers.RemoveAt(at);
            channel_senders.RemoveAt(at);
        }

        public void DeleteAllChannels()
        {
            channel_ids = new List<int>();
            channel_type = new List<QosType>();
            channel_name = new List<string>();
            channel_receivers = new List<OTONetReceiver>();
            channel_senders = new List<OTONetSender>();
        }

        public void ResetCom()
        {
            disconnect();
            configure();
            connect();
        }

        public int GetSendRate(out byte iError)
        {
            return NetworkTransport.GetPacketSentRate(_genericHostId, _distantConnectionID, out iError);
        }

        void ToSendEvent(OTONetSender sender, byte[] data, int ndata)
        {

            if (!HasAPeer) throw new System.InvalidOperationException("Tried to send without being connected!");

            byte error;

            for (int i = 0; i < NChannels; i++)
            {
                if (channel_senders[i] == sender)
                {
                    NetworkTransport.Send(_genericHostId, _distantConnectionID, channel_ids[i], data, data.Length, out error);
                    //     Debug.Log("Error on send : " + ((NetworkError)error).ToString() + " Chanel = " + channel_ids[i] + " Data size = " + data.Length + " _genericHostId " + _genericHostId + " _distantConnectionID " + _distantConnectionID);

                    // Debug.Log("data lenght" + data.Length + "chanel" + i);
                    if (error != 0)
                    {
                        throw new System.Exception("Error on send : " + ((NetworkError)error).ToString() + " Chanel = " + channel_ids[i] + " Data size = " + data.Length + " _genericHostId " + _genericHostId + " _distantConnectionID " + _distantConnectionID);
                    }
                }
            }
        }
    }


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
            //base.OnInspectorGUI();

            OTONetwork script = (OTONetwork)target;

            EditorGUI.BeginDisabledGroup(script.Communicating);

            /*		script.discovery = (NetworkDiscovery) EditorGUILayout.ObjectField("network discovery", script.discovery, typeof(NetworkDiscovery), true);
                    if (!script.discovery)
                        Debug.Log ("discovery null");*/
            script.IsServer = EditorGUILayout.Toggle("Server ", script.IsServer);


            if (script.IsClient)
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
                EditorGUILayout.LabelField("ID : " + script.channel_ids[i] + ", name :", _layoutOption3);
                script.channel_name[i] = EditorGUILayout.TextField(script.channel_name[i]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                script.channel_type[i] = (QosType)EditorGUILayout.EnumPopup(script.channel_type[i]);
                if (GUILayout.Button("X", _layoutOption1))
                {
                    script.DeleteAChannel(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Receiver", _layoutOption1);
                script.channel_receivers[i] = (OTONetReceiver)EditorGUILayout.ObjectField(script.channel_receivers[i], typeof(OTONetReceiver), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sender", _layoutOption1);
                script.channel_senders[i] = (OTONetSender)EditorGUILayout.ObjectField(script.channel_senders[i], typeof(OTONetSender), true);
                EditorGUILayout.EndHorizontal();


                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add a channel"))
            {
                int at = (int)script.AddAChannel("", QosType.Reliable, null, null);
                script.channel_name[at] = "Channel " + (int)script.channel_ids[at];
            }


            EditorGUI.EndDisabledGroup();

            if (GUI.changed)
                EditorUtility.SetDirty(script);

        }
    }


#endif

}