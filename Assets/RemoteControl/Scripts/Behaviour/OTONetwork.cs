using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    public class OTONetwork : MonoBehaviour
    {
        public int Port { get { return mPort; } set { mPort = value; } }
        public string IP { get { return mIP; } set { mIP = value; } }
        public List<int> Channel_ids { get { return mChannel_ids; } set { mChannel_ids = value; } }
        public List<string> Channel_name { get { return mChannel_name; } set { mChannel_name = value; } }
        public List<QosType> Channel_type { get { return mChannel_type; } set { mChannel_type = value; } }
        public List<OTONetSender> Channel_senders { get { return mChannel_senders; } set { mChannel_senders = value; } }
        public List<OTONetReceiver> Channel_receivers { get { return mChannel_receivers; } set { mChannel_receivers = value; } }

        private const int MAX_FRAME_PER_UPDATE = 10;

        private static ushort sBufferSize = 15000;

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
        private bool mRsconnect = false;
        private byte[] mRecBuffer = new byte[sBufferSize];
        private int mGenericHostId;
        private int mDistantHostID;
        private int mDistantConnectionID;
        private ConnectionConfig mConfig = new ConnectionConfig();
        private GlobalConfig mGConfig = new GlobalConfig();

        public bool IsServer;
        public bool Communicating { get; protected set; }
        public bool HasAPeer { get; protected set; }
        public int NChannels { get { return mChannel_ids.Count; } }

        void Start()
        {
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
            if (mRsconnect)
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

        private IEnumerator UpdateOto()
        {
            while (true) {
                if (sBufferSize < (ushort.MaxValue - 1000) && mRsconnect)
                    sBufferSize = ushort.MaxValue - 1;

                if (sBufferSize >= (ushort.MaxValue - 1000) && mRsconnect && !HasAPeer) {
                    mRecBuffer = new byte[sBufferSize];
                    HasAPeer = true;
                }

                ReceiveData();
                yield return null;
            }
        }

        private void Configure()
        {
            mConfig = new ConnectionConfig();

            // Verify the number of each lists
            if (mChannel_name.Count != NChannels
                || mChannel_type.Count != NChannels
                || mChannel_senders.Count != NChannels
                || mChannel_receivers.Count != NChannels)
                throw new IndexOutOfRangeException("Counter of channels lists are not equal");

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
            mGConfig.ReactorMaximumReceivedMessages = 1; //Initial value of 256
            mGConfig.ReactorMaximumSentMessages = 1; //Initial value of 256
        }

        private void Connect()
        {
            if (Communicating)
                Disconnect();

            if (IsServer) {
                NetworkTransport.Init(mGConfig);
                HostTopology lTopology = new HostTopology(mConfig, 1);

                mGenericHostId = NetworkTransport.AddHost(lTopology, mPort, null);
            } else {
                NetworkTransport.Init(mGConfig);

                HostTopology lTopology = new HostTopology(mConfig, 1);
                mGenericHostId = NetworkTransport.AddHost(lTopology, 0);
                byte lError;
                mDistantConnectionID = NetworkTransport.Connect(mGenericHostId, mIP, mPort, 0, out lError);
                if (lError != 0) {
                    throw new Exception("Error on connection : " + lError);
                }
            }
            Communicating = true;
        }

        private void Disconnect()
        {
            NetworkTransport.RemoveHost(mGenericHostId);
            NetworkTransport.Shutdown();

            foreach (OTONetSender lOtosender in mChannel_senders)
                if (lOtosender != null) lOtosender.Unsubscribe(ToSendEvent);

            Communicating = false;
            HasAPeer = false;
            mRsconnect = false;
            sBufferSize = 15000;
            mIsCorou = false;

            Debug.Log("OTONetwork Disconnected");
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
                                                    sBufferSize, out lDataSize, out lError);
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
                        mRsconnect = true;
                        break;

                    case NetworkEventType.DataEvent:
                        HasAPeer = true;
                        for (int i = 0; i < NChannels; i++) {
                            if (mChannel_ids[i] == lRecChannelID)
                                mChannel_receivers[i].ReceiveData(mRecBuffer, lDataSize);
                        }
                        break;

                    case NetworkEventType.DisconnectEvent:
                        new HomeCmd().Execute();
                        break;
                }
            }
        }

        private void ToSendEvent(OTONetSender iSender, byte[] iData, int iNdata)
        {
            if (!HasAPeer)
                throw new InvalidOperationException("Tried to send without being connected!");

            byte lError;
            for (int i = 0; i < NChannels; ++i) {
                if (mChannel_senders[i] == iSender) {
                    NetworkTransport.Send(mGenericHostId, mDistantConnectionID, mChannel_ids[i], iData, iData.Length, out lError);
                    if (lError != 0)
                        throw new Exception("Error on send : " + ((NetworkError)lError).ToString()
                            + " Chanel = " + mChannel_ids[i] + " Data size = "
                            + iData.Length + " _genericHostId " + mGenericHostId + " _distantConnectionID " + mDistantConnectionID);
                }
            }
        }
    }
}