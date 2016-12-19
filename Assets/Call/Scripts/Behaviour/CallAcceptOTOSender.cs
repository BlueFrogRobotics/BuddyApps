using UnityEngine;
using System.Collections;

namespace BuddyApp.Call
{
    public class CallAcceptOTOSender : OTONetSender
    {
        public void AcceptCall()
        {
            SendData(new byte[] { 1 }, 1);
        }

        public void CloseCall()
        {
            SendData(new byte[] { 0 }, 1);
        }
    }
}