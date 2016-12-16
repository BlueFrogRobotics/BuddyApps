using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class RLGLTutorial : MonoBehaviour
    {

        [SerializeField]
        private GameObject Listener;

        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private int mCountStep;
        private bool mIsQuestionDone;

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        private float mTimer;

        // Use this for initialization
        void Start()
        {
            mSTT = BYOS.Instance.SpeechToText;
            mTTS = BYOS.Instance.TextToSpeech;
            mIsQuestionDone = false;
            mCountStep = 0;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mCountStep == 4 && mTTS.HasFinishedTalking() && !mIsQuestionDone)
            {
                mTTS.Say("Do you want to play now?");
                mIsQuestionDone = true;
            }


            if (mCountStep == 4 && mTimer > 5.0F && mSTT.HasFinished)
            {
                mTimer = 0.0F;
                Listener.GetComponent<RLGLListener>().STTRequest(5);
            }
        }
    }

}
