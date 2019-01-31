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
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            lListenInstruction.Mode = SpeechRecognitionMode.FREESPEECH_ONLY;
            //Debug.Log("speech 1");
            lListenInstruction.Credentials = buddyLabBehaviour.FreeSpeechCredentials;
            //Debug.Log("speech 2");
            lListenInstruction.ConditionalUtterances = new string[2];
            lListenInstruction.ConditionalUtterances.Value[0] = utterance.ToLower();
            if (!string.IsNullOrEmpty(utterance))
                lListenInstruction.ConditionalUtterances.Value[1] = utterance[0].ToString().ToUpper() + utterance.Substring(1);
            else
                lListenInstruction.ConditionalUtterances.Value[1] = utterance;
        }

        protected override void SetInternalParameters()
        {
            base.SetInternalParameters();
            if (mInstruction == null)
                mInstruction = new ListenInputBehaviourInstruction();
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            utterance = lListenInstruction.ConditionalUtterances.Value[0];
            //Debug.Log("speech 1 bis");
            lListenInstruction.Credentials = buddyLabBehaviour.FreeSpeechCredentials;
            //Debug.Log("speech 2 bis");
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