using UnityEngine;
using System.Collections;
using Buddy.UI;
using Buddy;
using System;

namespace BuddyApp.RedLightGreenLight
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
        private Face mFace;

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

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        private Dictionary mDico;

        // Use this for initialization
        void Start()
        {
            mFace = BYOS.Instance.Interaction.Face;
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            mNeedListen = true;
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
            mWheels = BYOS.Instance.Primitive.Motors.Wheels;
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
            listener.GetComponent<RLGLListener>().ErrorCount = 0;

            mDico = BYOS.Instance.Dictionary;

        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;


            if (!mIsQuestionDone && mTTS.HasFinishedTalking && mTimer > 3.0F) {
                mIsQuestionDone = true;
                BYOS.Instance.Interaction.TextToSpeech.Say(mDico.GetRandomString("menu1"));
                mTimer = 0.0F;
            }
            if (!mIsAnswerPlayYes && mTimer > 3.0F && mTTS.HasFinishedTalking && mIsQuestionDone && mNeedListen) {
                Debug.Log("kikoo : " + mTTS.HasFinishedTalking );
                mTimer = 0.0F;
                listener.GetComponent<RLGLListener>().STTRequest(5);
                mNeedListen = false;
            }
            if (mIsAnswerPlayYes && !mIsMovementDone) {
                if (!mIsCanvasDisable) {
                    BYOS.Instance.Toaster.Display<ChoiceToast>().With("Menu");
                    background.SetTrigger("Close");
                    menu.SetTrigger("Close_WMenu3");
                    mIsCanvasDisable = true;
                    mTimer = 0.0F;
                }
                if (mIsCanvasDisable && !mIsFirstSentenceDone && mTimer > 0.5F) {
                    mFace.SetExpression(MoodType.NEUTRAL);
                    mTTS.Say(mDico.GetRandomString("menu2"));
                    mIsFirstSentenceDone = true;
                    mTimer = 0.0F;
                }
                if (!mIsFirstMovementDone && mTimer > 0.5F) {
                    if (!mIsDone) {
                        mWheels.TurnAngle(90.0F, 400.0F, 0.02F);
                        mIsDone = true;
                        mTimer = 0.0F;
                    }
                    Debug.Log(mWheels.Status);
                    if ((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.5F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F)) {
                        mIsFirstMovementDone = true;
                        mIsDone = false;
                    }
                }
                if (!mIsSecondMovementDone && mIsFirstMovementDone) {
                    if (!mIsDone) {
                        mTimer = 0.0F;
                        mWheels.TurnAngle(-180.0F, 400.0F, 0.02F);
                        mIsDone = true;
                    }
                    Debug.Log(mWheels.Status);
                    if ((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.5F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F)) {
                        mIsSecondMovementDone = true;
                        mIsDone = false;
                    }
                }
                if (!mIsThirdMovementDone && mIsSecondMovementDone && !mIsMovementDone) {
                    if (!mIsDone) {
                        mTimer = 0.0F;
                        mWheels.TurnAngle(90.0F, 400.0F, 0.02F);
                        mIsDone = true;
                    }
                    Debug.Log(mWheels.Status);
                    if ((mWheels.Status == MovingState.REACHED_GOAL && mTimer > 0.5F) || (mWheels.Status == MovingState.MOTIONLESS && mTimer > 0.5F)) {
                        mIsMovementDone = true;
                        mIsDone = false;
                    }
                }
            }
            if (!mIsScriptDone && mIsMovementDone && mTTS.HasFinishedTalking) {
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