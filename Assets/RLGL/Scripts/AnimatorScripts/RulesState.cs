using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class RulesState : StateMachineBehaviour
    {

        private TextToSpeech mTTS;
        private float mTimer;
        private bool mIsSentenceDone = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mTimer = 0.0f;
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            mTimer += Time.deltaTime;
            //Debug.Log("mtimer " + mTimer);

            if (mTimer >= 2.0f && !mIsSentenceDone)
            {
                //mTTS.Say("Bonjour mon ami, on va jouer ensemble! Recule de six grand pas et quand je dirais 1 2 3 soleil, " +
                //" il faudra que t'avance pour toucher mon visage mais attention si je te vois avancer quand j'ai les " +
                //" yeux ouvert alors il faudra recommencer. Maintenant que j'ai donné les consignes, nous allons pouvoir " +
                //" commencer à jouer dans 5 secondes. Cinq quatre, trois, deux, un, c'est parti");

                mTTS.Say("salut mon ami, nous allons jouer à un deux trois soleil ensemble");

                mIsSentenceDone = true;
            }

            if (mTimer > 10.0f && mIsSentenceDone && mTTS.HasFinishedTalking())
            {
                animator.SetBool("IsRulesDone", true);
            }

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("IsRulesDone", false);
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
