using UnityEngine;
using System;

namespace BuddyApp.Remote
{
    public class OTONetReceiver : MonoBehaviour
    {
        public virtual void ReceiveData(byte[] iData, int iNdata)
        {
            throw new NotImplementedException("ReceiveData not implemented");
        }
    }
}