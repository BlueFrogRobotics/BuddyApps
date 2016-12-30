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
        private Wheels mWheels;

        private bool mIsQuestionDone;
        private bool mIsMovementDone;
        private float mTimer;

        private bool mIsFirstMovementDone;
        private bool mIsSecondMovementDone;
        private bool mIsThirdMovementDone;

        private bool mIsScriptDone;

        private bool mIsCanvasDisable;

        private bool mIsDone;

        private bool mIsFirstSentenceDone;

        private bool mNeedListen;
        public bool NeedListen { get { return mNeedListen; } set { mNeedListen = value; } }

        //private string mSTTNotif;
        //public string STTNotif { get { return mSTTNotif; } set { mSTTNotif = value; } }

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        // Use this for initialization
        void Start()
        {
            
            BYOS.Instance.VocalActivation.enabled = false;
            mNeedListen = true;
            mTTS = BYOS.Instance.TextToSpeech;
            mWheels = BYOS.Instance.Motors.Wheels;
            mIsQuestionDone = false;
            mIsAnswerPlayYes = false;
            mIsMovementDone = false;

            mIsFirstMovementDone = false;
            mIsSecondMovementDone = false;
            mIsThirdMovementDone = false;

            mIsScriptDone = false;
            mIsCanvasDisable = false;

            mIsFirstSentenceDone = false;

            mIsDone = false;
            //mSTTNotif = "";
            listener.GetComponent<RLGLListener>().ErrorCount = 0;
            
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            
            
            if(!mIsQuestionDone && mTTS.HasFinishedTalking && mTimer > 3.0F)
            {
                Debug.Log("1");
                mIsQuestionDone = true;
                mTTS.Say("What do you want to do?");
                mTimer = 0.0F;
            }

            
            if (!mIsAnswerPlayYes && mTimer > 3.0F && mTTS.HasFinishedTalking && mIsQuestionDone && mNeedListen)
            {
                Debug.Log("2");
                mTimer = 0.0F;
                listener.GetComponent<RLGLListener>().STTRequest(5);
                mNeedListen = false;
            }


            if (mIsAnswerPlayYes && !mIsMovementDone)
            {
                if(!mIsCanvasDisable )
                {
                    Debug.Log("3");
                    background.SetTrigger("Close_BG");
                    menu.SetTrigger("Close_WMenu3");
                    
                    mIsCanvasDisable = true;
                    mTimer = 0.0F;
                }
                if(mIsCanvasDisable && !mIsFirstSentenceDone && mTimer > 0.5F)
                {
                    mTTS.Say("Oh yes I love this game!");
                    mIsFirstSentenceDone = true;
                    mTimer = 0.0F;
                }
                if (!mIsFirstMovementDone && mTimer > 0.5F)
                {
                    
                    Debug.Log("3.1");
                    
                    if(!mIsDone)
                    {
                        mWheels.TurnAngle(90.0F, 400.0F, 0.02F);
                        mIsDone = true;
                        mTimer = 0.0F;
                    }
                    Debug.Log(mWheels.Status);
                    if((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.1F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F))
                    {
                        mIsFirstMovementDone = true;
                        mIsDone = false;
                    }
                }
                if (!mIsSecondMovementDone && mIsFirstMovementDone)
                {
                    
                    Debug.Log("3.2"  + mWheels.Status);
                    if(!mIsDone)
                    {
                        mTimer = 0.0F;
                        mWheels.TurnAngle(-180.0F, 400.0F, 0.02F);
                        mIsDone = true;
                    }
                    Debug.Log(mWheels.Status);
                    if ((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.1F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F))
                    {
                        mIsSecondMovementDone = true;
                        mIsDone = false;
                    }
                        
                    
                }
                if ( !mIsThirdMovementDone && mIsSecondMovementDone && !mIsMovementDone)
                {
                    
                    Debug.Log("3.3" + mWheels.Status);
                    if(!mIsDone)
                    {
                        mTimer = 0.0F;
                        mWheels.TurnAngle(90.0F, 400.0F, 0.02F);
                        mIsDone = true;
                    }
                    Debug.Log(mWheels.Status);
                    if ((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.1F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F))
                    {
                        mIsMovementDone = true;
                        mIsDone = false;
                    }
                }
            }


            if (!mIsScriptDone &&  mIsMovementDone && mTTS.HasFinishedTalking)
            {
                Debug.Log("4");
                gameplay.SetActive(true);
                mIsScriptDone = true;
            }
        }

        public void OnClickedButtonPlay()
        {
            mIsAnswerPlayYes = true;
        }

    }

}