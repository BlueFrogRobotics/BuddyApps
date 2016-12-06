using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public abstract class ADetector : MonoBehaviour
    {
        public delegate void Detection();
        public event Detection OnDetection;
        
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void OnSendDetection()
        {
            if (OnDetection != null)
                OnDetection();
        }
    }
}

