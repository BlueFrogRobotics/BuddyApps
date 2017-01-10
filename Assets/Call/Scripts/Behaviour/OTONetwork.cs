using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.Command;

namespace BuddyApp.Call
{
    public class OTONetwork : MonoBehaviour
    {
        public bool IsServer { get { return mIsServer; } set { mIsServer = value; } }
        public bool Communicating { get; protected set; }
        public bool HasAPeer { get; protected set; }
        public int NChannels { get { return mChannel_ids.Count; } }
        public int Port { get { return mPort; } set { mPort = value; } }
        public string IP { get { return mIP; } set { mIP = value; } }
        public List<int> Channel_ids { get { return mChannel_ids; } set { mChannel_ids = value; } }
        public List<string> Channel_name { get { return mChannel_name; } set { mChannel_name = value; } }
        public List<QosType> Channel_type { get { return mChannel_type; } set { mChannel_type = value; } }
        public List<OTONetSender> Channel_senders { get { return mChannel_senders; } set { mChannel_senders = value; } }
        public List<OTONetReceiver> Channel_receivers { get { return mChannel_receivers; } set { mChannel_receivers = value; } }

        private const int MAX_FRAME_PER_UPDATE = 20;
        private static ushort mBufferSize = 15000;

        [SerializeField, HideInInspector]
        private bool mIsServer;

        [SerializeField, HideInInspector]
        private int mPort;

        [SerializeField, HideInInspector]
        private string mIP;

        [SerializeField, HideInInspector]
        private List<int> mChannel_ids;

        [SerializeField, HideInInspector]
        private List<string> mChannel_name;

        [SerializeField, HideInInspector]
        private List<QosType> mChannel_type;

        [SerializeField, HideInInspector]
        private List<OTONetSender> mChannel_senders;

        [SerializeField, HideInInspector]
        private List<OTONetReceiver> mChannel_receivers;

        private bool mIsCorou = false;
        private bool mIsConnected = false;
        private byte[] mRecBuffer = new byte[mBufferSize];
        private int mGenericHostId;
        private int mDistantHostID;
        private int mDistantConnectionID;
        private ConnectionConfig mConfig = new ConnectionConfig();
        private GlobalConfig mGConfig = new GlobalConfig();

        private float mTime;

        void Start()
        {
            BuddyOS.BYOS.Instance.VocalManager.EnableTrigger = false;
            Configure();
            Connect();
        }

        void Update()
        {
            if (!Communicating)
                return;
            else if (!mIsCorou) {
                StartCoroutine(UpdateOto());
                mIsCorou = true;
            }
        }

        void OnDisable()
        {
            if(mIsConnected)
                Disconnect();
        }

        void OnDestroy()
        {
            if (mIsConnected || NetworkTransport.IsStarted)
                Disconnect();
        }

        public int AddAChannel(string iName, QosType iType, OTONetReceiver iOTONetReceiver, OTONetSender iOTONetSender)
        {
            int lID = mConfig.AddChannel(iType);
            mChannel_ids.Add(lID);
            mChannel_type.Add(iType);
            mChannel_name.Add(iName);
            mChannel_receivers.Add(iOTONetReceiver);
            mChannel_senders.Add(iOTONetSender);

            return mChannel_ids.Count - 1;
        }

        public void DeleteAChannel(int iAt)
        {
            mChannel_ids.RemoveAt(iAt);
            mChannel_type.RemoveAt(iAt);
            mChannel_name.RemoveAt(iAt);
            mChannel_receivers.RemoveAt(iAt);
            mChannel_senders.RemoveAt(iAt);
        }

        public void DeleteAllChannels()
        {
            mChannel_ids = new List<int>();
            mChannel_type = new List<QosType>();
            mChannel_name = new List<string>();
            mChannel_receivers = new List<OTONetReceiver>();
            mChannel_senders = new List<OTONetSender>();
        }

        public void ResetCom()
        {
            Disconnect();
            Configure();
            Connect();
        }

        public int GetSendRate(out byte iError)
        {
            return NetworkTransport.GetPacketSentRate(mGenericHostId, mDistantConnectionID, out iError);
        }

        public int GetReceivedRate(out byte iError)
        {
            return NetworkTransport.GetPacketReceivedRate(mGenericHostId, mDistantConnectionID, out iError);
        }

        IEnumerator UpdateOto()
        {
            while (true) {
                if (mBufferSize < (ushort.MaxValue - 1000) && mIsConnected)
                    mBufferSize = ushort.MaxValue - 1;

                if (mBufferSize >= (ushort.MaxValue - 1000) && mIsConnected && !HasAPeer) {
                    mRecBuffer = new byte[mBufferSize];
                    HasAPeer = true;
                }

                ReceiveData();
                yield return null;
            }
        }

        private void Configure()
        {
            mConfig = new ConnectionConfig();

            // Verify the number of each list
            if (mChannel_name.Count != NChannels
                || mChannel_type.Count != NChannels
                || mChannel_senders.Count != NChannels
                || mChannel_receivers.Count != NChannels)
                throw new IndexOutOfRangeException("Counters of channels are not equal");

            // Fill config
            string lStr = "";

            for (int i = 0; i < NChannels; i++) {
                mChannel_ids[i] = mConfig.AddChannel(mChannel_type[i]);

                if (mChannel_senders[i] != null)
                    mChannel_senders[i].Subscribe(ToSendEvent);

                lStr += "Configure (" + i + ")";
                lStr += "\nid = " + mChannel_ids[i];
                lStr += "\nname = " + mChannel_name[i];
                lStr += "\ntype = " + mChannel_type[i];

                if (mChannel_receivers[i] != null)
                    lStr += "\nreceiver = " + mChannel_receivers[i].gameObject.name;

                if (mChannel_senders[i] != null)
                    lStr += "\nsender = " + mChannel_senders[i].gameObject.name;

                lStr += "\n\n";
            }
            Debug.Log(lStr);

            mConfig.PacketSize = ushort.MaxValue;
            mConfig.MaxSentMessageQueueSize = 1024;
            // Global config
            mGConfig = new GlobalConfig();
            mGConfig.MaxPacketSize = 65000;
            mGConfig.ReactorMaximumReceivedMessages = 1;//valeur initiale a 256
            mGConfig.ReactorMaximumSentMessages = 1;//valeur initiale a 256
        }

        private void Connect()
        {
            if (Communicating)
                Disconnect();

            if (IsServer) {
                NetworkTransport.Init(mGConfig);
                HostTopology lTopology = new HostTopology(mConfig, 1);

                mGenericHostId = NetworkTransport.AddHost(lTopology, Port, null);
            }
            else {
                NetworkTransport.Init(mGConfig);

                HostTopology lTopology = new HostTopology(mConfig, 1);
                mGenericHostId = NetworkTransport.AddHost(lTopology, 0);
                byte lError;
                mDistantConnectionID = NetworkTransport.Connect(mGenericHostId, IP, Port, 0, out lError);

                if (lError != 0)
                    throw new Exception("Error on connection : " + lError);
            }
            Communicating = true;

            Debug.Log("OTO Connected");
            mTime = Time.time;
        }

        public void Disconnect()
        {
            NetworkTransport.RemoveHost(mGenericHostId);
            NetworkTransport.Shutdown();

            foreach (OTONetSender otosender in mChannel_senders)
                if (otosender != null) otosender.Unsubscribe(ToSendEvent);

            Communicating = false;
            HasAPeer = false;
            mIsConnected = false;
            mBufferSize = 15000;
            mIsCorou = false;

            Debug.Log("OTO Disconnected");
        }

        private void ReceiveData()
        {
            int lRecChannelID;
            byte lError;
            int lDataSize;
            int lCount = 0;

            while (lCount++ < MAX_FRAME_PER_UPDATE) {
                NetworkEventType lRecData = NetworkTransport.Receive(out mDistantHostID, out mDistantConnectionID,
                                                out lRecChannelID, mRecBuffer,
                                                mBufferSize, out lDataSize, out lError);
                if (lError != 0)
                    Debug.Log("Error = " + ((NetworkError)lError).ToString() +
                        " from distantHostID " + mDistantConnectionID + " on distantConnectionID " +
                        mDistantConnectionID + " on recChannelID " + lRecChannelID);

                if (lDataSize >= mRecBuffer.Length)
                    return;

                switch (lRecData) {
                    case NetworkEventType.Nothing:
                        return;

                    case NetworkEventType.ConnectEvent:
                        mIsConnected = true;
                        break;

                    case NetworkEventType.DataEvent:
                        HasAPeer = true;
                        for (int i = 0; i < NChannels; i++) {
                            if (mChannel_ids[i] == lRecChannelID)
                                mChannel_receivers[i].ReceiveData(mRecBuffer, lDataSize);
                        }
                        break;

                    case NetworkEventType.DisconnectEvent:
                        Debug.Log("Received DisconnectEvent");
                        Disconnect();
                        new HomeCmd().Execute();
                        break;
                }
            }
        }

        void ToSendEvent(OTONetSender iSender, byte[] iData, int iNData)
        {
            if (!HasAPeer)
                throw new InvalidOperationException("Tried to send without being connected!");

            byte lError;
            for (int i = 0; i < NChannels; i++) {
                if (mChannel_senders[i] == iSender) {
                    //Debug.Log("Sending data " + GetString(data) + " to hostID " + _genericHostId + " on connection " + _distantConnectionID + " on channel " + channel_ids[i]);
                    NetworkTransport.Send(mGenericHostId, mDistantConnectionID, mChannel_ids[i], iData, iData.Length, out lError);
                    
                    if (lError != 0)
                        throw new Exception("Error on send : " + ((NetworkError)lError).ToString() +
                            " Channel = " + mChannel_ids[i] + " Data size = " + iData.Length +
                            " GenericHostId " + mGenericHostId + " DistantConnectionID " + mDistantConnectionID +
                            " after " + (Time.time-mTime) + "s");
                }
            }
        }
    }
}