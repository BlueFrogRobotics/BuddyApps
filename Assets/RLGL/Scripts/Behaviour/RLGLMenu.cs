using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.RLGL
{
    public class RLGLMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject listener;

        [SerializeField]
        private Animator menu;
        [SerializeField]
        private Animator background;
        [SerializeField]
        private GameObject gameplay;

        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private Wheels mWheels;

        private bool mIsQuestionDone;
        private bool mIsMovementDone;
        private bool mIsUIDone;
        private float mTimer;

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        // Use this for initialization
        void Start()
        {
            mSTT = BYOS.Instance.SpeechToText;
            mTTS = BYOS.Instance.TextToSpeech;
            mWheels = BYOS.Instance.Motors.Wheels;
            mIsQuestionDone = false;
            mIsAnswerPlayYes = false;
            mIsMovementDone = false;
            mIsUIDone = false;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            
            
            if(!mIsQuestionDone && mTTS.HasFinishedTalking() )
            {
                Debug.Log("1");
                mIsQuestionDone = true;
                mTTS.Say("What do you want to do?");
                
            }

            
            if (!mIsAnswerPlayYes && mTimer > 5.0F && mSTT.HasFinished && mIsQuestionDone)
            {
                Debug.Log("2");
                mTimer = 0.0F;
                listener.GetComponent<RLGLListener>().STTRequest(5);
            }

            
            //if (mIsAnswerPlayYes && !mIsMovementDone)
            //{
            //    Debug.Log("3");
            //    background.SetTrigger("Close_BG");
            //    menu.SetTrigger("Close_WMenu3");
            //    mWheels.TurnAngle(360.0F, 300.0F, 0.02F);
            //    mTTS.Say("Oh yes I love this game!");
            //    mTimer = 0.0F;
            //    mIsMovementDone = true;
            //}

            
            //if (mIsMovementDone && !mIsUIDone && mTTS.HasFinishedTalking() && ((mWheels.Status == MovingState.REACHED_GOAL) || (mTimer > 2.0F)))
            //{
            //    Debug.Log("4");
            //    gameplay.SetActive(true);
            //}
        }

    }

}