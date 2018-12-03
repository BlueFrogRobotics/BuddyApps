using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.BuddyLab
{
    public class SayElement : AGraphicElement, IEditableParameter
    {
        [SerializeField]
        private string key;

        private string mUtterance;


        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new SayBehaviourInstruction();
            SayBehaviourInstruction lSayInstruction = (SayBehaviourInstruction)mInstruction;

            if(key!=null)
                lSayInstruction.Key = key;
            if (mUtterance != null)
                lSayInstruction.Utterance = mUtterance;
        }

        protected override void SetInternalParameters()
        {
            base.SetInternalParameters();
            if (mInstruction == null)
                mInstruction = new SayBehaviourInstruction();
            SayBehaviourInstruction lSayInstruction = (SayBehaviourInstruction)mInstruction;
            //if(lSayInstruction.Utterance!=null)
                mUtterance = lSayInstruction.Utterance.Value;
        }

        public string GetEditableParameter()
        {
            return mUtterance;
        }

        public void SetEditableParameter(string iParameter)
        {
            mUtterance = iParameter;
        }
    }
}