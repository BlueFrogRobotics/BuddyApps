using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.RLGL
{
    public class RLGLMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject Listener;

        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private bool mIsQuestionDone;
        private float mTimer;

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        // Use this for initialization
        void Start()
        {
            mSTT = BYOS.Instance.SpeechToText;
            mTTS = BYOS.Instance.TextToSpeech;
            mIsAnswerPlayYes = true;
            Debug.Log("");
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (!mIsQuestionDone && mTTS.HasFinishedTalking) {
                mTTS.Say("What do you want to do?");
                mIsQuestionDone = true;
            }


            if (!mIsAnswerPlayYes && mTimer > 5.0F && mSTT.HasFinished) {
                mTimer = 0.0F;
                Listener.GetComponent<RLGLListener>().STTRequest(5);
            }
        }
    }
}