using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Condition element that test if a given text has been said
    /// <para>See <see cref="ListenInputBehaviourInstruction"/>  </para>
    /// </summary>
    public sealed class SpeechRecoElement : AGraphicElement, IEditableParameter
    {
        [SerializeField]
        private BuddyLabBehaviour buddyLabBehaviour;

        private string utterance;

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ListenInputBehaviourInstruction();
            Debug.Log("speech 1");
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            Debug.Log("speech 2");
            lListenInstruction.Mode = SpeechRecognitionMode.FREESPEECH_ONLY;
            Debug.Log("speech 3");
            lListenInstruction.Credentials = "aza";// buddyLabBehaviour.FreeSpeechCredentials;
            Debug.Log("speech 4");
            Debug.Log("credentials: " + buddyLabBehaviour.FreeSpeechCredentials);
            lListenInstruction.ConditionalUtterances = new string[1];
            Debug.Log("speech 5");
            lListenInstruction.ConditionalUtterances.Value[0] = utterance;
        }

        protected override void SetInternalParameters()
        {
            base.SetInternalParameters();
            if (mInstruction == null)
                mInstruction = new ListenInputBehaviourInstruction();
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            utterance = lListenInstruction.ConditionalUtterances.Value[0];
            lListenInstruction.Credentials = buddyLabBehaviour.FreeSpeechCredentials;
        }

        public string GetEditableParameter()
        {
            return utterance;
        }

        public void SetEditableParameter(string iParameter)
        {
            utterance = iParameter;
        }
    }
}