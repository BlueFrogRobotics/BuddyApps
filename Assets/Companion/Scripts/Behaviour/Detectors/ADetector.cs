using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Abstract detector class
    /// </summary>
    public abstract class ADetector : MonoBehaviour
    {
        public delegate void Detection();
        public event Detection OnDetection;

        //Detector detected somehting
        protected void OnSendDetection()
        {
            if (OnDetection != null)
                OnDetection();
        }
    }
}

