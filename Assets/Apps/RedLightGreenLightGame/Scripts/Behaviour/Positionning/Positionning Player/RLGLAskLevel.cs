using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLAskLevel : AStateMachineBehaviour
    {
        private TextToSpeech mTTS;
        private bool mListening;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mSwitchState;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS = Interaction.TextToSpeech;
            mRLGLBehaviour.Timer = 0;
            mSwitchState = false;
            //Interaction.Mood.Set(MoodType.THINKING);
            Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);
            Interaction.SpeechToText.OnBestRecognition.Add(OnRecognition);
            StartCoroutine(AskLevel());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer > 10.0f && !mSwitchState && !mListening)
            {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                //mListening = false;
                mRLGLBehaviour.Timer = 0.0f;
                mSwitchState = true;
                GetComponentInGameObject<LevelManager>(0).SetLevel(0);
                Trigger("LevelAsked");
            }

            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;

            if (!mListening && !mSwitchState)
            {
                Interaction.Mood.Set(MoodType.LISTENING);
                Interaction.SpeechToText.Request();
                mListening = true;
            }
            //if (!mIsSentenceDone)
            //{
            //    mTTS.SayKey("failedtopositionning");
            //    mIsSentenceDone = true;
            //}

            //if (mTTS.HasFinishedTalking && mIsSentenceDone)
            //    animator.SetTrigger("Start");

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Interaction.SpeechToText.OnBestRecognition.Remove(OnRecognition);
            Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
        }

        private IEnumerator AskLevel()
        {
            string lText = Dictionary.GetRandomString("asklevel");
            lText = lText.Replace("[maxlevel]", "" + 10);
            mTTS.Say(lText);
            mRLGLBehaviour.Timer = 0;
            yield return null;
            //yield return SayKeyAndWait("asklevel");

        }

        void ErrorSTT(STTError iSpeech)
        {
            mListening = false;
        }

        private void OnRecognition(string iText)
        {
            Debug.Log("reconnu: " + iText);
            int lLevel = 0;
            string lText = iText.Replace(Dictionary.GetString("level"), "").Replace(Dictionary.GetString("the"), "");
            bool lHasParsed = int.TryParse(lText, out lLevel);
            mListening = false;
            if (lHasParsed)
            {
                mSwitchState = true;
                GetComponentInGameObject<LevelManager>(0).SetLevel(lLevel-1);
                Trigger("LevelAsked");
            }
            else
                mTTS.SayKey("didntunderstand");
            //if (Dictionary.ContainsPhonetic(iText, "yes"))
            //{
            //    Toaster.Hide();
            //    StartCoroutine(Restart());
            //    mSwitchState = true;
            //}
            //else if (Dictionary.ContainsPhonetic(iText, "no"))
            //{
            //    Toaster.Hide();
            //    Trigger("Quit");
            //    mSwitchState = true;
            //}
        }
    }
}

