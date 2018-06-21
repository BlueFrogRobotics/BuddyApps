using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLAskLevel : AStateMachineBehaviour
    {
        private TextToSpeech mTTS;
        private bool mListening;
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mSwitchState;
        private string mNameVoconGrammarFile;
        private string mSpeech;
        private float mTime;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
            mNameVoconGrammarFile = "rlgl";
            mSpeech = string.Empty;
            mTime = 0F;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS = Interaction.TextToSpeech;
            mRLGLBehaviour.Timer = 0;
            mSwitchState = false;
            mListening = false;
            //Interaction.Mood.Set(MoodType.THINKING);
            //Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);
            //Interaction.SpeechToText.OnBestRecognition.Add(OnRecognition);

            //Use vocon
            Interaction.VocalManager.UseVocon = true;

            Interaction.VocalManager.AddGrammar(mNameVoconGrammarFile, LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = OnRecognition;
            Interaction.VocalManager.OnVoconEvent = EventVocon;

            Interaction.VocalManager.EnableDefaultErrorHandling = false;

            StartCoroutine(AskLevel());
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTime += Time.deltaTime;
            if (mRLGLBehaviour.Timer > 10.0f && !mSwitchState && !mListening)
            {
                Interaction.Mood.Set(MoodType.NEUTRAL);
                //mListening = false;
                mRLGLBehaviour.Timer = 0.0f;
                mSwitchState = true;
                GetComponentInGameObject<LevelManager>(0).SetLevel(0);
                Trigger("LevelAsked");
            }

            if (mTime > 4F)
            {
                mListening = false;
                mTime = 0F;
            }

            if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
                return;
            if (!mListening && !mSwitchState)
            {
                Interaction.VocalManager.StartInstantReco();
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
            //Interaction.SpeechToText.OnBestRecognition.Remove(OnRecognition);

            // Vocon
            Interaction.VocalManager.StopRecognition();
            Interaction.VocalManager.RemoveGrammar(mNameVoconGrammarFile, LoadContext.APP);
            Interaction.VocalManager.UseVocon = false;
            Interaction.VocalManager.OnVoconBest = null;
            Interaction.VocalManager.OnVoconEvent = null;

            //Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
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

        private void OnRecognition(VoconResult iText)
        {
            Debug.Log("reconnu: " + iText.Utterance + "!");
            if (string.IsNullOrEmpty(iText.Utterance))
            {
                mListening = false;
                return;
            }

            int lLevel = 0;
            string lText = iText.Utterance.Replace(Dictionary.GetString("level"), "").Replace(Dictionary.GetString("the"), "");
            bool lHasParsed = int.TryParse(lText, out lLevel);
            if (lHasParsed)
            {
                if (lLevel >= 0 && lLevel < 11)
                {
                    mSwitchState = true;
                    GetComponentInGameObject<LevelManager>(0).SetLevel(lLevel - 1);
                    Trigger("LevelAsked");
                }
                else
                    mTTS.SayKey("nolevel");
            }
            else
                mTTS.SayKey("didntunderstand");
            mListening = false;

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

