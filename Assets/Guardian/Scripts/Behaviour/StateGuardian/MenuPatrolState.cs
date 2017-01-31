using UnityEngine;
using System;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class MenuPatrolState : AStateGuardian
    {
        public int Mode { get { return mMode; } set { mMode = value; } }

        private int mMode;
        private GameObject mMenu;
        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private Action<string> mActionSetMode;
        private float mTimer = 2.2f;
        private bool mHasAskedOnce = false;
        private Animator mMenuAnimator;
        private Mood mMood;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            mMenu = StateManager.Menu;
            mMenuAnimator = mMenu.GetComponent<Animator>();

            animator.SetBool("ChangeState", false);
            mTTS = BYOS.Instance.TextToSpeech;
            mSTT = BYOS.Instance.SpeechToText;
            mMood = BYOS.Instance.Mood;
            //mActionSetMode = new Action<string>(SetMode);
            mSTT.OnBestRecognition.Add(SetMode);
            mSTT.OnEnd.Add(OnEndReco);
            mTimer = 2.2f;
            animator.SetInteger("Mode", 0);
            mMode = 0;
            mHasAskedOnce = false;

            StateManager.BackgroundAnimator.SetTrigger("Open_BG");
            mMenuAnimator.SetTrigger("Open_WMenu3");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mMode != 0)
            {
                animator.SetBool("ChangeState", true);
                mSTT.OnBestRecognition.Remove(SetMode);
                mSTT.OnEnd.Remove(OnEndReco);
            }
            /*if (!mHasTalked)
            {
                mTTS.Say("Quel mode souhaite tu. Fixe ou mobile");
                mHasTalked = true;
            }*/

            mTimer -= Time.deltaTime;
            if (mTimer < 0.0f && (!mHasAskedOnce || mSTT.HasFinished) && mMode==0)
            {
                //SetWindowAppOverBuddyColor(1);
                Debug.Log("ask");
                mHasAskedOnce = true;
                //mMenu.SetActive(true);
                //StateManager.BackgroundPrefab.SetActive(true);
                mMood.Set(MoodType.LISTENING);
                mSTT.Request();
                mTimer = 4.2f;
            }


        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
            //StateManager.BackgroundAnimator.SetTrigger("Close_BG");
            mMenuAnimator.SetTrigger("Close_WMenu3");
            //StateManager.BackgroundPrefab.SetActive(false);
            mSTT.OnBestRecognition.Clear();
            mSTT.OnEnd.Clear();
            animator.SetInteger("Mode", mMode);
            //mMenu.SetActive(false);
            animator.SetBool("ChangeState", false);
        }

        public void SetMode(string iAnswer)
        {
            if (mMode == 0)
            {
                Debug.Log(iAnswer);

                if ( (iAnswer.ToLower().Contains("mobile") && !iAnswer.ToLower().Contains("immobile")) || (iAnswer.ToLower().Contains("move") && !iAnswer.ToLower().Contains("don't")) )//(iAnswer == "mobile")
                {
                    mMode = 2;
                    Debug.Log("FIXE");
                }
                else if (iAnswer.ToLower().Contains("immobile") || iAnswer.ToLower().Contains("fixe") || iAnswer.ToLower().Contains("fix") || iAnswer.ToLower().Contains("don't move"))//(iAnswer == "immobile" || iAnswer=="fixe" || iAnswer=="six")
                {
                    mMode = 1;
                    Debug.Log("MOBILE");
                }
                else
                {
                    mTTS.Say("I didn't understand");
                }
            }
        }

        private void OnEndReco()
        {
            mMood.Set(MoodType.NEUTRAL);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}