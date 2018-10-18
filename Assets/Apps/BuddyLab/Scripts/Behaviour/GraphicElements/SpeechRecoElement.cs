using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class SpeechRecoElement : AGraphicElement, IEditableParameter
    {
        private string utterance;

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ListenInputBehaviourInstruction();
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            lListenInstruction.ConditionalUtterances = new string[1];
            lListenInstruction.ConditionalUtterances.Value[0] = utterance;
        }

        protected override void SetInternalParameters()
        {
            base.SetInternalParameters();
            if (mInstruction == null)
                mInstruction = new ListenInputBehaviourInstruction();
            ListenInputBehaviourInstruction lListenInstruction = (ListenInputBehaviourInstruction)mInstruction;
            //if(lSayInstruction.Utterance!=null)
            utterance = lListenInstruction.ConditionalUtterances.Value[0];
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