using UnityEngine;
using System.Collections;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.RLGL
{
    public class CountState : StateMachineBehaviour
    {

        private Motors mMotors;
        private GameObject mCanvasUI;
        private Button mButtonToWin;
        private float mTimer;
        private Face mFace;
        private bool mIsCountDone;
        private TextToSpeech mTTS;
        private bool mFirstSentence;
        private bool mSecondSentence;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mFirstSentence = false;
            mSecondSentence = false;
            mIsCountDone = false;
            mTimer = 0.0f;
            mMotors = BYOS.Instance.Motors;
            mFace = BYOS.Instance.Face;
            mMotors.YesHinge.SetPosition(45.0f, 100.0f);
            mCanvasUI = animator.GetComponent<GameObjectLinker>().CanvasUI;
            mCanvasUI.SetActive(true);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            //Debug.Log(mTimer);
            if(!mIsCountDone && mTimer < 5.0f)
            {
                mTTS.SetSpeechRate(0.1f);
                if (mTTS.HasFinishedTalking() && !mFirstSentence && !mSecondSentence)
                {
                    mTTS.Say("Un deux trois,");
                    mFirstSentence = true;
                }
                else if(mTTS.HasFinishedTalking() && mFirstSentence && !mSecondSentence)
                {
                    mTTS.SetSpeechRate(1.0f);
                    mTTS.Say("soleil");
                    mIsCountDone = true;
                    mSecondSentence = true;
                }
            }

            if(mTTS.HasFinishedTalking() && mIsCountDone)
            {
                animator.SetBool("IsCountDone", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mFirstSentence = false;
            mSecondSentence = false;
            mTTS.SetSpeechRate(1.0f);
            animator.SetBool("IsCountDone", false);
            animator.SetBool("IsWon", false);
            mCanvasUI.SetActive(false);
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

