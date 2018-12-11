using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
    public class ECMoveForwardState : StateMachineBehaviour
    {

        private AnimatorManager mAnimatorManager;
        private MoveForwardBehaviour mBehaviour;

        private bool mAddReco;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("[EXCENTER] on state ECMoveForwardState");
            mAnimatorManager = GameObject.Find("AIBehaviour").GetComponent<AnimatorManager>();
            mBehaviour = GameObject.Find("AIBehaviour").GetComponent<MoveForwardBehaviour>();
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            Buddy.Vocal.EnableTrigger = true;
            Buddy.Vocal.StopListening();
            mBehaviour.InitBehaviour();
            mAddReco = false;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (mBehaviour.behaviourEnd && !mAddReco)
            {
                //if (ExperienceCenterData.Instance.VoiceTrigger)
                //   BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
                //Buddy.Vocal.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
                Buddy.Vocal.EnableTrigger = true;
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
                //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);

                //VOCON
                //BYOS.Instance.Interaction.VocalManager.UseVocon = true;
                //BYOS.Instance.Interaction.VocalManager.AddGrammar("experiencecenter", LoadContext.APP);
                Buddy.Vocal.OnEndListening.Add(SpeechToTextCallback);
                Buddy.Vocal.Listen("experiencecenter", SpeechRecognitionMode.GRAMMAR_ONLY);
                Buddy.Vocal.OnListeningStatus.Add((iInput) => { SpeechToTextError(iInput); });
                //BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
                //BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
                //BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
                mAddReco = true;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBehaviour.StopBehaviour();
            Buddy.Vocal.StopListening();
            //BYOS.Instance.Interaction.VocalManager.UseVocon = false;
            //BYOS.Instance.Interaction.VocalManager.RemoveGrammar("experiencecenter", LoadContext.APP);
        }

        public void SpeechToTextCallback(SpeechInput iSpeech)
        {
            if (!string.IsNullOrEmpty(iSpeech.Utterance))
            {
                Debug.LogFormat("[EXCENTER] MoveForward - SpeechToText {0}: ", iSpeech.Utterance);
                bool lClauseFound = false;
                string[] lPhonetics = Buddy.Resources.GetPhoneticStrings("movego");

                foreach (string lClause in lPhonetics)
                {
                    if (!string.IsNullOrEmpty(iSpeech.Utterance) && iSpeech.Utterance.Contains(lClause))
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

        public void SpeechToTextError(SpeechInputStatus iEvent)
        {
            if (iEvent.IsError)
            {
                Debug.LogWarningFormat("[EXCENTER] ERROR STT: {0}", iEvent.ToString());
                Buddy.Behaviour.Mood = Mood.NEUTRAL;
            }
        }

    }
}
