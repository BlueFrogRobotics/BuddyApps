﻿using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{

    [RequireComponent(typeof(RedLightGreenLightStateMachineManager))]
    public class RLGLBehaviour : MonoBehaviour
    {
        private TextToSpeech mTTS;
        private Animator mAnimator;
        private Face mFace;
        [HideInInspector]
        public bool mReplay;

        [SerializeField]
        private GameObject Background;


        private int mIndex;
        public int Index { get { return mIndex; } set { mIndex = value; } }

        private bool mIsClicked;
        public bool IsClicked { get { return mIsClicked; } set { mIsClicked = value; } }

        //[SerializeField]
        //private GameObject mCanvasRules;

        // Use this for initialization
        void Start()
        {
            mFace = BYOS.Instance.Interaction.Face;
            mAnimator = GetComponent<Animator>();

        }

        public void OnClickedButtonYes()
        {
            mIsClicked = true;
            if(mIndex == 0)
            {
                BYOS.Instance.Interaction.Face.SetExpression(MoodType.NEUTRAL);
                mAnimator.GetBehaviour<StartState>().IsAnswerYes = true;
                Debug.Log("ON CLICK BUTTON YES START");
            }
            else if (mIndex == 1)
            {
                BYOS.Instance.Interaction.Face.SetExpression(MoodType.NEUTRAL);
                mAnimator.GetBehaviour<RulesState>().IsAnswerRuleYes = true;
                Debug.Log("ON CLICK BUTTON YES RULES");
            }
            else if (mIndex == 2)
            {
                BYOS.Instance.Interaction.Face.SetExpression(MoodType.NEUTRAL);
                mAnimator.GetBehaviour<ReplayState>().IsAnswerReplayYes = true;
                Debug.Log("ON CLICK BUTTON YES REPLAY");
            }
        }

        public void OnClickedButtonNo()
        {
            mIsClicked = true;
            if (mIndex == 0)
            {
                mAnimator.GetBehaviour<StartState>().IsAnswerNo = true;

                Debug.Log("ON CLICK BUTTON NO START");
            }
            else if (mIndex == 1)
            {
                mAnimator.GetBehaviour<RulesState>().IsAnswerRuleNo = true;
                Debug.Log("ON CLICK BUTTON NO RULES");
            }
            else if (mIndex == 2)
            {
                RedLightGreenLightActivity.QuitApp();
                //mAnimator.GetBehaviour<ReplayState>().IsAnswerReplayNo = true;
            }
        }

        public void OnCLickTuto()
        {
            mIsClicked = true;
        }

        public void OnCLickMenu()
        {
            mIsClicked = true;
        }

        public void OnClickedButtonTowin()
        {
            mAnimator.SetBool("IsWon", true);
        }

        public void OnClickedButtonReplay()
        {
            mReplay = true;
        }
    }
}