using UnityEngine;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class ReplayState : StateMachineBehaviour
    {

        private GameObject mWindowQuestion;
        private float mTimer;
        private bool mIsSentenceDone;
        private TextToSpeech mTTS;
        private bool mGOLinkerSentence;
        private bool mGOLinkerDone;
        

        private bool mIsAnswerYes;
        public bool IsAnswerYes { get { return mIsAnswerYes; } set { mIsAnswerYes = value; } }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mTimer = 0.0f;
            mWindowQuestion = animator.GetComponent<GameObjectLinker>().WindowQuestion;
            mWindowQuestion.SetActive(true);
            mIsSentenceDone = false;
            mGOLinkerSentence = animator.GetComponent<GameObjectLinker>().mIsSentenceDone;
            mGOLinkerSentence = false;
            mGOLinkerDone = animator.GetComponent<GameObjectLinker>().mReplay;
            mGOLinkerDone = false;


        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if(mTimer < 12.0f && mIsAnswerYes && !mIsSentenceDone )
            {
                mTTS.Say("Ok je te laisse 10 secondes pour te replacer.");
                mIsSentenceDone = true;
            }

            if(mTimer > 22.0f && mTTS.HasFinishedTalking() && mIsAnswerYes)
            {
                animator.SetBool("IsReplayDone", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mWindowQuestion.SetActive(false);
            animator.SetBool("IsReplayDone", false);
            mIsSentenceDone = false;
            mIsAnswerYes = false;
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

