using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLRepositionning : AStateMachineBehaviour
    {
        //TODO take real level
        private int mLevel = 0;
        private bool mEndTimer = false;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            StartCoroutine(Repositionning());
            mEndTimer = false;
            //Interaction.TextToSpeech.Silence(1000);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator Repositionning()
        {
            yield return SayKeyAndWait("repositionning");
            //Toaster.Display<CountdownToast>().With(5, EndCountDown);
            if (Interaction.TextToSpeech.HasFinishedTalking)
                EndCountDown();
            while (!mEndTimer)
                yield return null;
            //yield return new WaitForSeconds(5);
            mLevel++;
            //mRLGLBehaviour.StartPositionning();
            Trigger("Repositionning");
        }

        private void EndCountDown()
        {
            mEndTimer = true;
        }

    }

}
