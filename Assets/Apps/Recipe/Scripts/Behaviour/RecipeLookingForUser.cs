using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Recipe
{
    public class RecipeLookingForUser : AStateMachineBehaviour
    {
        RecipeBehaviour mBehaviour;
        private bool mIsUserClicked;
        private float mTimer;
        private bool mTrackingDone;
        //pas de detection il demande a lutilisateur de bien le placer 

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            RecipeUtils.DebugColor("looking for user state", "blue");
            mBehaviour = new RecipeBehaviour();
            mTimer = 0F;
            mTrackingDone = false;
            GetGameObject(0).SetActive(true);
            mIsUserClicked = false;
            Buddy.Navigation.Run<HumanTrackStrategy>().StaticTracking(tracking => true, OnTrackingDone, BehaviourMovementPattern.HEAD | BehaviourMovementPattern.BODY_ROTATION);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > 120F)
                QuitApp();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(0).SetActive(false);
        }

        public void OnTrackingDone(HumanEntity iHumanEntity)
        {
            mTimer = 0F;
            if (!mTrackingDone)
            {
                Buddy.Vocal.SayKeyAndListen("firstareyouready", null, OnEndListenning, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                mTrackingDone = true;
            }
        }


        private void OnEndListenning(SpeechInput iSpeechInput)
        {
            Debug.Log("OnEndListenning " + iSpeechInput.Utterance);

            if (!iSpeechInput.IsInterrupted)
            {
                if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("recipeyes")))
                {
                    Buddy.Navigation.Stop();
                    Trigger("HEAD_POSITION_DONE");
                }
                else if ((!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, Buddy.Resources.GetString("recipeno"))) && mTimer >= 12F)
                {
                    mTimer = 0F;
                    Buddy.Vocal.SayKeyAndListen("firstareyouready", null, OnEndListenning, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                }
            }



        }
    }
}

