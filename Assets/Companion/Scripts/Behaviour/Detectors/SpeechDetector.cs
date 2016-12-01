using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class SpeechDetector : MonoBehaviour
    {
        public bool SomeoneTalkingDetected { get { return mSomeoneTalking; } }

        private bool mSomeoneTalking;
        private SphinxTrigger mSphinx;

        void Start()
        {
            mSomeoneTalking = false;
            mSphinx = BYOS.Instance.SphinxTrigger;
            //mSphinx.LaunchRecognition();
        }

        void Update()
        {
            mSomeoneTalking = mSphinx.HasTriggered();
        }
    }
}