using UnityEngine;
using System;

namespace BuddyApp.Call
{
    public delegate void ToSendEventHandler(OTONetSender iSender, byte[] iData, int iNdata);

    public class OTONetSender : MonoBehaviour
    {
        private event ToSendEventHandler ToSendEvent;

        public void SendData(byte[] iData, int iNdata)
        {
            if (ToSendEvent == null)
                throw new InvalidOperationException("Send event has not been defined");
            else
                ToSendEvent(this, iData, iNdata);
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
}