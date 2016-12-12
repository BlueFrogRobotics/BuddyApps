using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class SpeechDetector : MonoBehaviour
    {
        public bool SomeoneTalkingDetected { get { return mSomeoneTalking; } }

        private bool mSomeoneTalking;
        private VocalActivation mVocalActivation;

        void Start()
        {
            mSomeoneTalking = false;
            mVocalActivation = BYOS.Instance.VocalActivation;
        }

        void Update()
        {
            mSomeoneTalking = mVocalActivation.RecognitionTriggered;
        }
    }
}