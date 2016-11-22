using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    public delegate void ToSendEventHandler(OTONetSender iSender, byte[] iData, int iNdata);
    public class OTONetSender : MonoBehaviour
    {
        private event ToSendEventHandler ToSendEvent;

        public void SendData(byte[] iData, int iNdata)
        {
            if (ToSendEvent == null) throw new InvalidOperationException("Send event has not been defined");
            else ToSendEvent(this, iData, iNdata);
        }

        public void Subscribe(ToSendEventHandler iEvent)
        {
            ToSendEvent += iEvent;
        }

        public void Unsubscribe(ToSendEventHandler iEvent)
        {
            ToSendEvent -= iEvent;
        }
    }

    public class OTONetReceiver : MonoBehaviour
    {
        public virtual void ReceiveData(byte[] iData, int iNdata)
        {
            throw new NotImplementedException("ReceiveData not implemented");
        }
    }

    public class OTONetwork : MonoBehaviour
    {
        public int Port;
        public string IP;

        public List<int> channel_ids;
        public List<string> channel_name;
        public List<QosType> channel_type;
        public List<OTONetSender> channel_senders;
        public List<OTONetReceiver> channel_receivers;

        public bool IsServer;
        public bool Communicating { get; protected set; }
        public bool HasAPeer { get; protected set; }
        public int NChannels { get { return channel_ids.Count; } }

        private bool mIsCorou = false;
        private bool mRsconnect = false;
        private byte[] mRecBuffer = new byte[sBufferSize];
        static ushort sBufferSize = 15000;
        private const int MAX_FRAME_PER_UPDATE = 10;
        private int mGenericHostId;
        private int mDistantHostID;
        private int mDistantConnectionID;
        private ConnectionConfig mConfig = new ConnectionConfig();
        private GlobalConfig mGConfig = new GlobalConfig();

        void Update()
        {
            if (!Communicating)
                return;

            else if (!mIsCorou)
            {
                StartCoroutine("UpdateOto");
                mIsCorou = true;
            }
        }

        void Start()
        {
            Configure();
            Connect();
        }


        void OnDisable()
        {
            if (mRsconnect)
                Disconnect();
        }

        private IEnumerator UpdateOto()
        {
            while (true)
            {
                if (sBufferSize < (ushort.MaxValue - 1000) && mRsconnect)
                    sBufferSize = ushort.MaxValue - 1;

                if (sBufferSize >= (ushort.MaxValue - 1000) && mRsconnect && !HasAPeer)
                {
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
            if (channel_name.Count != NChannels
                || channel_type.Count != NChannels
                || channel_senders.Count != NChannels
                || channel_receivers.Count != NChannels)
                throw new IndexOutOfRangeException("Counter of channels lists are not equal");

            // Fill config
            string lStr = "";
            for (int i = 0; i < NChannels; i++)
            {
                channel_ids[i] = mConfig.AddChannel(channel_type[i]);

                if (channel_senders[i] != null)
                    channel_senders[i].Subscribe(ToSendEvent);

                lStr += "Configure (" + i + ")";
                lStr += "\nid = " + channel_ids[i];
                lStr += "\nname = " + channel_name[i];
                lStr += "\ntype = " + channel_type[i];

                if (channel_receivers[i] != null)
                    lStr += "\nreceiver = " + channel_receivers[i].gameObject.name;

                if (channel_senders[i] != null)
                    lStr += "\nsender = " + channel_senders[i].gameObject.name;

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

            if (IsServer)
            {
                NetworkTransport.Init(mGConfig);
                HostTopology lTopology = new HostTopology(mConfig, 1);

                mGenericHostId = NetworkTransport.AddHost(lTopology, Port, null);
            }
            else
            {
                NetworkTransport.Init(mGConfig);

                HostTopology lTopology = new HostTopology(mConfig, 1);
                mGenericHostId = NetworkTransport.AddHost(lTopology, 0);
                byte lError;
                mDistantConnectionID = NetworkTransport.Connect(mGenericHostId, IP, Port, 0, out lError);
                if (lError != 0)
                {
                    throw new Exception("Error on connection : " + lError);
                }
            }
            Communicating = true;
        }

        void Disconnect()
        {
            NetworkTransport.RemoveHost(mGenericHostId);
            NetworkTransport.Shutdown();

            foreach (OTONetSender lOtosender in channel_senders)
                if (lOtosender != null) lOtosender.Unsubscribe(ToSendEvent);

            Communicating = false;
            HasAPeer = false;
            mRsconnect = false;
            sBufferSize = 15000;
            mIsCorou = false;

            Debug.Log("OTONetwork Disconnected");
        }

        void ReceiveData()
        {
            int lRecChannelID;
            byte lError;
            int lDataSize;
            int lCount = 0;
            while (lCount++ < MAX_FRAME_PER_UPDATE)
            {
                NetworkEventType lRecData = NetworkTransport.Receive(out mDistantHostID, out mDistantConnectionID, 
                                                    out lRecChannelID, mRecBuffer, 
                                                    sBufferSize, out lDataSize, out lError);
                if (lError != 0)
                    Debug.Log("Error = " + ((NetworkError)lError).ToString() +
                        " from distantHostID " + mDistantConnectionID + " on distantConnectionID " +
                        mDistantConnectionID + " on recChannelID " + lRecChannelID);

                if (lDataSize >= mRecBuffer.Length)
                    return;

                switch (lRecData)
                {
                    case NetworkEventType.Nothing:
                        return;

                    case NetworkEventType.ConnectEvent:
                        mRsconnect = true;
                        break;

                    case NetworkEventType.DataEvent:
                        HasAPeer = true;
                        for (int i = 0; i < NChannels; i++)
                        {
                            if (channel_ids[i] == lRecChannelID)
                                channel_receivers[i].ReceiveData(mRecBuffer, lDataSize);
                        }
                        break;

                    case NetworkEventType.DisconnectEvent:
                        HomeCmd.Create().Execute();
                        break;
                }
            }
        }

        public int AddAChannel(string iName, QosType iType, OTONetReceiver iOTONetReceiver, OTONetSender iOTONetSender)
        {
            int lID = mConfig.AddChannel(iType);
            channel_ids.Add(lID);
            channel_type.Add(iType);
            channel_name.Add(iName);
            channel_receivers.Add(iOTONetReceiver);
            channel_senders.Add(iOTONetSender);

            return channel_ids.Count - 1;
        }

        public void DeleteAChannel(int iAt)
        {
            channel_ids.RemoveAt(iAt);
            channel_type.RemoveAt(iAt);
            channel_name.RemoveAt(iAt);
            channel_receivers.RemoveAt(iAt);
            channel_senders.RemoveAt(iAt);
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
            Disconnect();
            Configure();
            Connect();
        }

        public int GetSendRate(out byte iError)
        {
            return NetworkTransport.GetPacketSentRate(mGenericHostId, mDistantConnectionID, out iError);
        }

        void ToSendEvent(OTONetSender iSender, byte[] iData, int iNdata)
        {
            if (!HasAPeer)
                throw new InvalidOperationException("Tried to send without being connected!");

            byte lError;

            for (int i = 0; i < NChannels; i++)
            {
                if (channel_senders[i] == iSender)
                {
                    NetworkTransport.Send(mGenericHostId, mDistantConnectionID, channel_ids[i], iData, iData.Length, out lError);
                    if (lError != 0)
                        throw new Exception("Error on send : " + ((NetworkError)lError).ToString()
                            + " Chanel = " + channel_ids[i] + " Data size = "
                            + iData.Length + " _genericHostId " + mGenericHostId + " _distantConnectionID " + mDistantConnectionID);
                }
            }
        }
    }
}