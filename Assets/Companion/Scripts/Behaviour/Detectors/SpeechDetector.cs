using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Tells if a vocal interaction was requested with the "Hello Buddy" trigger
    /// </summary>
    public class SpeechDetector : MonoBehaviour
    {
        public bool SomeoneTalkingDetected { get { return mSomeoneTalking; } }

        private bool mSomeoneTalking;
        private VocalManager mVocalManager;

        void Start()
        {
            mSomeoneTalking = false;
            mVocalManager = BYOS.Instance.VocalManager;
        }

        void Update()
        {
            mSomeoneTalking = mVocalManager.RecognitionTriggered;
        }
    }
}