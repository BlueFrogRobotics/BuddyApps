using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
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