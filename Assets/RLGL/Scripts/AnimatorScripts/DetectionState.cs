using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class DetectionState : StateMachineBehaviour
    {

        private GameObject mMotionDetector;
        private Motors mMotors;
        private bool mIsHeadUp = false;
        private TextToSpeech mTTS;
        private Face mFace;
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsDetected;
        private int mCount;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCount = 0;
            mIsSentenceDone = false;
            mMotors = BYOS.Instance.Motors;
            mTTS = BYOS.Instance.TextToSpeech;
            mFace = BYOS.Instance.Face;
            mTimer = 0.0f;

            if (!mIsHeadUp)
            {
                mMotors.YesHinge.SetPosition(-5.0f, 100.0f);
                mIsHeadUp = true;
            }

            if (mMotionDetector == null)
            {
                mMotionDetector = animator.gameObject.GetComponent<GameObjectLinker>().MotionDetector;
            }
            if (mIsHeadUp)
            {
                mMotionDetector.SetActive(true);
            }
            mIsDetected = false;
           
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsDetected = mMotionDetector.GetComponent<MotionGame>().IsMoving();
            mTimer += Time.deltaTime;
            Debug.Log(mTimer);
            //Player detected by buddy
            if(mTimer < 2.0f)
            {
                mIsDetected = false;
            }
            if (mIsDetected && !mIsSentenceDone && mTimer < 8.0f && mTimer > 2.0f)
            {
                mFace.SetMood(FaceMood.HAPPY);
                mFace.SetEvent(FaceEvent.SMILE);

                mTTS.Say("ahah je t'ai vu avancer, retourne au point de daipart. Je te laisse 10 secondes pour retourner au point de daipart");

                mIsSentenceDone = true;
            }
            if(mIsSentenceDone  && mTTS.HasFinishedTalking() && mTimer > 20.0f)
            {
                animator.SetBool("IsDetectedTrue", true);
            }

            if (!mIsSentenceDone && mTimer < 8.0f && mCount == 0 && mTimer < 2.0f)
            {
                mCount++;
                mTTS.Say("hum tu es fort, je ne te vois pas bouger");
                mFace.SetMood(FaceMood.FOCUS);
            }

            //player not detected by buddy
            if (!mIsSentenceDone && mTimer > 8.0f && mTTS.HasFinishedTalking())
            {
                animator.SetBool("IsDetectedFalse", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsSentenceDone = false;
            animator.SetBool("IsDetectedTrue", false);
            animator.SetBool("IsDetectedFalse", false);
            mFace.SetMood(FaceMood.NEUTRAL);
            if (mMotionDetector != null)
                mMotionDetector.SetActive(false);

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

