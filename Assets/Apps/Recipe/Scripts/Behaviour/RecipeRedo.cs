using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Recipe
{
    public class RecipeRedo : AStateMachineBehaviour
    {
        private const int NB_MAX_LISTENING = 6;

        private float mTimer;
        private int mNbListening;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer = 0F;
            mNbListening = 0;
            Buddy.Vocal.SayKeyAndListen("reciperedo", null, OnEndListening, null, SpeechRecognitionMode.FREESPEECH_ONLY);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            mNbListening++;
            Debug.Log("OnEndListenning " + iSpeechInput.Utterance);

            //if (iSpeechInput.IsInterrupted)
            //{
                RecipeUtils.DebugColor("1", "red");
            if (mNbListening < NB_MAX_LISTENING)
            {
                RecipeUtils.DebugColor("2", "red");
                if (!string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, "recipeyes"))
                {
                    RecipeUtils.DebugColor("3", "red");
                    Trigger("REDO_RECIPE");
                }
                else if (!(string.IsNullOrEmpty(iSpeechInput.Utterance) && Utils.ContainsOneOf(iSpeechInput.Utterance, "recipeno")))
                {
                    RecipeUtils.DebugColor("4", "red");
                    QuitApp();
                }
                else if ((string.IsNullOrEmpty(iSpeechInput.Utterance)))
                {
                    mTimer = 0F;
                    Buddy.Vocal.SayKeyAndListen("reciperedo", null, OnEndListening, null, SpeechRecognitionMode.FREESPEECH_ONLY);
                }
            }
            else
            {
                RecipeUtils.DebugColor("5", "red");
                QuitApp();
            }

        //}
    }
    }
}

