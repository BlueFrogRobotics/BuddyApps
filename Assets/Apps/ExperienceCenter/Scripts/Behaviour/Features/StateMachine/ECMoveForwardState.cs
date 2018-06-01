using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
    public class ECMoveForwardState : StateMachineBehaviour
    {

        private AnimatorManager mAnimatorManager;
        private MoveForwardBehaviour mBehaviour;

        private bool mAddReco;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mAnimatorManager = GameObject.Find("AIBehaviour").GetComponent<AnimatorManager>();
            mBehaviour = GameObject.Find("AIBehaviour").GetComponent<MoveForwardBehaviour>();
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            BYOS.Instance.Interaction.VocalManager.StopAllCoroutines();
            mBehaviour.InitBehaviour();
            mAddReco = false;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (mBehaviour.behaviourEnd && !mAddReco)
            {
                //if (ExperienceCenterData.Instance.VoiceTrigger)
                //   BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
                //BYOS.Instance.Interaction.VocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
                BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
                //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);

                //VOCON
                BYOS.Instance.Interaction.VocalManager.UseVocon = true;
                BYOS.Instance.Interaction.VocalManager.AddGrammar("experiencecenter", LoadContext.APP);
                BYOS.Instance.Interaction.VocalManager.OnVoconBest = SpeechToTextCallback;

                //BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
                BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
                //BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
                mAddReco = true;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBehaviour.StopBehaviour();
            BYOS.Instance.Interaction.VocalManager.UseVocon = false;
            BYOS.Instance.Interaction.VocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);
        }

        public void SpeechToTextCallback(VoconResult iSpeech)
        {
            if (string.IsNullOrEmpty(iSpeech.Utterance))
            {
                Debug.LogFormat("[EXCENTER] MoveForward - SpeechToText {0}: ", iSpeech.Utterance);
                bool lClauseFound = false;
                string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings("movego");

                foreach (string lClause in lPhonetics)
                {
                    if (iSpeech.Utterance.Contains(lClause))
                    {
                        lClauseFound = true;
                        break;
                    }
                }

                if (!lClauseFound)
                    Debug.Log("[EXCENTER] MoveForward - SpeechToText : Not Found");
                else
                    mAnimatorManager.ActivateCmd((byte)(Command.IOT));
            }
        }

        public void SpeechToTextError(STTError iError)
        {
            Debug.LogWarningFormat("[EXCENTER] ERROR STT: {0}", iError.ToString());
            BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
        }

    }
}
