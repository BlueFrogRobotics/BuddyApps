using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;


namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerRTPSentence : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(StartGameplay());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        IEnumerator StartGameplay()
        {
            //yield return SayKeyAndWait("smallrules");
            while (BYOS.Instance.Interaction.TextToSpeech.IsSpeaking)
                yield return null;
            mRLGLBehaviour.StartGameplay();
            //yield return null;
        }
    }
}

