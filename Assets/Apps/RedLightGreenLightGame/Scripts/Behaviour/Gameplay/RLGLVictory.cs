using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLVictory : AStateMachineBehaviour
	{
        private LevelManager mLevelManager;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
            mLevelManager = GetComponent<LevelManager>();
            StartCoroutine(Congratulation());
			
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
            mRLGLBehaviour.IsPlaying = false;
        }

        private IEnumerator Congratulation()
        {
            Primitive.Motors.YesHinge.SetPosition(0F, 150.0F);
            yield return SayKeyAndWait("victory");
            Interaction.Mood.Set(MoodType.HAPPY);
            Interaction.Face.SetEvent(FaceEvent.SMILE);
            int lLevel = mLevelManager.LevelData.Level;
            Toaster.Display<VictoryToast>().With("leveldone" + string.Format("leveldone", (lLevel + 1)));
            yield return new WaitForSeconds(2);
            Toaster.Hide();
            yield return SayAndWait(Dictionary.GetRandomString("wonlevel") + (lLevel+1) );
            yield return new WaitForSeconds(1);
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mLevelManager.LevelUp();
            lLevel = mLevelManager.LevelData.Level;
            yield return SayAndWait(Dictionary.GetRandomString("seriousbegin") + (lLevel+1) +". Let's go");
            Trigger("Repositionning");
        }

    }

}
