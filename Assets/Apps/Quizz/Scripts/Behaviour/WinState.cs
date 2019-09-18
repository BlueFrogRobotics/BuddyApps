using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Quizz
{
    public class WinState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;
        private SoundsManager mSoundsManager;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
            mSoundsManager = GetComponent<SoundsManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Quizz: win state");
            StartCoroutine(Win());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator Win()
        {
            mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Score++;

            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.Interpreter.StopAndClear();

            mSoundsManager.PlaySound(SoundsManager.Sound.GOOD_ANSWER);
            Buddy.Behaviour.Interpreter.Run("Happy01");
            yield return new WaitForSeconds(2);
            //yield return new WaitUntil(() => (!Buddy.Behaviour.Interpreter.IsBusy && !Buddy.Behaviour.Interpreter.IsSomethingPending));
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            string text = Buddy.Resources.GetRandomString("win").Replace("[answer]", "" + mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer]);
            if (!string.IsNullOrEmpty(mQuizzBehaviour.ActualQuestion.AnswerComplement.Trim()))
                text += " [100] " + mQuizzBehaviour.ActualQuestion.AnswerComplement;

            Buddy.Vocal.Say(text, (iOutput) => {
                Trigger("CheckNumQuestion");
            });           
            
        }
    }
}