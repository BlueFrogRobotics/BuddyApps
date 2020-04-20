using UnityEngine;
using System.Net.NetworkInformation;
using System.Text;

namespace BuddyApp.CoursTelepresence
{
    public class ConnectAndPing : MonoBehaviour
    {

        private System.Net.NetworkInformation.Ping mPingSender;
        private PingOptions mPingOptions;
        private string mDataToSend;
        private byte[] mBuffer;
        private int mTimeOut;
        private string mHost;

        // Use this for initialization
        void Start()
        {
            Debug.Log("START TEST PING");
            mPingOptions = new PingOptions();
            mPingSender = new System.Net.NetworkInformation.Ping();
            mDataToSend = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            mTimeOut = 120;
            mHost = "8.8.8.8";

            //Don't fragment the packet in multiple packet
            mPingOptions.DontFragment = true;
            mBuffer = Encoding.ASCII.GetBytes(mDataToSend);
            PingReply mPingReply = mPingSender.Send(mHost, mTimeOut, mBuffer, mPingOptions);

            if (mPingReply.Status == IPStatus.Success)
                Debug.Log("SUCESS : " + mPingReply.RoundtripTime + " ms");
            else if (mPingReply.Status == IPStatus.TimedOut)
                Debug.Log("TIMEOUT");
        }

    }

}
