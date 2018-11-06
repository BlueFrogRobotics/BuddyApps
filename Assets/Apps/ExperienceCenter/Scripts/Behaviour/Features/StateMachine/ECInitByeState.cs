﻿using System.Collections;
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
                Buddy.Vocal.EnableTrigger = false;
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
                //BYOS.Instance.Interaction.VocalManager.AddGrammar("experiencecenter", LoadContext.APP);
                Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
                Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);
                //BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
                mAddReco = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Vocal.Stop();
            //BYOS.Instance.Interaction.VocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);  
            //BYOS.Instance.Interaction.VocalManager.UseVocon = false;
        }

        public void SpeechToTextCallback(SpeechInput iSpeech)
        {
            if (string.IsNullOrEmpty(iSpeech.Utterance))
            {
                Debug.LogFormat("[EXCENTER] ByeBye - SpeechToText : {0}", iSpeech);
                bool lClauseFound = false;
                string[] lPhonetics = Buddy.Resources.GetPhoneticStrings("byecome");
                foreach (string lClause in lPhonetics)
                {
                    if (iSpeech.Utterance.Contains(lClause))
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

        //public void SpeechToTextError(STTError iError)
        //{
        //    Debug.LogWarningFormat("[EXCENTER][BYE] ERROR STT: {0}", iError.ToString());
        //    BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
        //}
    }
}
