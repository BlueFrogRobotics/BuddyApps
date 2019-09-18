using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Quizz
{
    public class StartGameState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>() ;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mQuizzBehaviour.Init();        

            string welcomeSentence = Buddy.Resources.GetRandomString("welcome").Replace("[n]", "" + QuizzData.Instance.NbQuestions);

            Buddy.Vocal.Say(welcomeSentence, (iOutput) => {
                // QuickStart launch a simple one player game without nickname
                string triggerName = QuizzData.Instance.OnePlayerGame ? "QuickStart" : "Start";
                Trigger(triggerName); 
            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
    }
}