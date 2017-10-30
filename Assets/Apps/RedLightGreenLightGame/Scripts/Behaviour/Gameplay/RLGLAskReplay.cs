using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLAskReplay : AStateMachineBehaviour
    {
        private LevelManager mLevelManager;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mSwitchState = false;
        private bool mListening;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mLevelManager = GetComponent<LevelManager>();
            mRLGLBehaviour.Timer = 0;
            mListening = false;
            mSwitchState = false;
            Interaction.SpeechToText.OnBestRecognition.Add(OnRecognition);
            StartCoroutine(AskReplay());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer > 6.0f)
            {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                mListening = false;
                mRLGLBehaviour.Timer = 0.0f;
            }

            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if (!mListening && !mSwitchState)
            {
                Interaction.Mood.Set(MoodType.LISTENING);
                Interaction.SpeechToText.Request();
                mListening = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Interaction.SpeechToText.OnBestRecognition.Remove(OnRecognition);
        }

        private IEnumerator AskReplay()
        {
            yield return SayKeyAndWait("askrestart");
            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("askrestart"), 
                () => StartCoroutine(Restart()),
                () => Trigger("Quit"));

        }

        private IEnumerator Restart()
        {
            mRLGLBehaviour.Life = 3;
            mLevelManager.Reset();
            yield return SayAndWait("ok");
            Trigger("Repositionning");
        }

        private void OnRecognition(string iText)
        {
            Debug.Log("reconnu: " + iText);
            if (Dictionary.ContainsPhonetic(iText, "yes"))
            {
                Toaster.Hide();
                StartCoroutine(Restart());
                mSwitchState = true;
            }
            else if (Dictionary.ContainsPhonetic(iText, "no"))
            {
                Toaster.Hide();
                Trigger("Quit");
                mSwitchState = true;
            }
            mListening = false;
        }

    }

}
