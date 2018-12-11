using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
    public class ECInitByeState : StateMachineBehaviour
    {

        private AnimatorManager mAnimatorManager;
        private IdleBehaviour mIdleBehaviour;
        private QuestionsBehaviour mQuestionsBehaviour;
        //private TextToSpeech mTTS;

        private bool mAddReco;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("[EXCENTER] on state ECInitByeState");
            mAnimatorManager = GameObject.Find("AIBehaviour").GetComponent<AnimatorManager>();
            mIdleBehaviour = GameObject.Find("AIBehaviour").GetComponent<IdleBehaviour>();
            mQuestionsBehaviour = GameObject.Find("AIBehaviour").GetComponent<QuestionsBehaviour>();
            //mTTS = BYOS.Instance.Interaction.TextToSpeech;
            //BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);
            //BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
            //BYOS.Instance.Interaction.VocalManager.UseVocon = true;
            mAddReco = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mAddReco && mIdleBehaviour.behaviourEnd && mQuestionsBehaviour.behaviourEnd)
            {
                //if (ExperienceCenterData.Instance.VoiceTrigger)
                //    BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
                //Buddy.Vocal.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
                Buddy.Vocal.EnableTrigger = true;
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
                //BYOS.Instance.Interaction.VocalManager.AddGrammar("experiencecenter", LoadContext.APP);
                Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
                Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);
                Buddy.Vocal.OnListeningStatus.Add(SpeechToTextError);
                //BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
                mAddReco = true;
                Debug.Log("[EXCENTER] update bye state");
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Debug.Log("[EXCENTER] exit bye state");
            Buddy.Vocal.StopListening();
            Buddy.Vocal.OnEndListening.Remove(SpeechToTextCallback);
            Buddy.Vocal.OnListeningStatus.Remove(SpeechToTextError);
            //BYOS.Instance.Interaction.VocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);  
            //BYOS.Instance.Interaction.VocalManager.UseVocon = false;
        }

        public void SpeechToTextCallback(SpeechInput iSpeech)
        {
            Debug.Log("[EXCENTER] stt bye state");
            if (!string.IsNullOrEmpty(iSpeech.Utterance))
            {
                Debug.LogFormat("[EXCENTER] ByeBye - SpeechToText : {0}", iSpeech);
                bool lClauseFound = false;
                string[] lPhonetics = Buddy.Resources.GetPhoneticStrings("byecome");
                foreach (string lClause in lPhonetics)
                {
                    if (!string.IsNullOrEmpty(iSpeech.Utterance) && iSpeech.Utterance.Contains(lClause))
                    {
                        lClauseFound = true;
                        break;
                    }
                }

                if (!lClauseFound)
                    Debug.Log("[EXCENTER] ByeBye - SpeechToText : Not Found");
                else
                {
                    mAnimatorManager.ActivateCmd((byte)(Command.MoveForward));
                }
            }
        }

        public void SpeechToTextError(SpeechInputStatus iStatus)
        {
            if (iStatus.IsError)
            {
                Debug.LogWarningFormat("[EXCENTER][BYE] ERROR STT: {0}", iStatus.ToString());
                Buddy.Behaviour.Mood = Mood.NEUTRAL;
            }
        }
    }
}
