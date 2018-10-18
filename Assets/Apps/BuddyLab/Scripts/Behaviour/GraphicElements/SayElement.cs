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

        //[SerializeField]
        //private string utterance;

        public string Utterance { get; set; }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new SayBehaviourInstruction();
            SayBehaviourInstruction lSayInstruction = (SayBehaviourInstruction)mInstruction;

            if(key!=null)
                lSayInstruction.Key = key;
            if (Utterance != null)
                lSayInstruction.Utterance = Utterance;
        }

        protected override void SetInternalParameters()
        {
            base.SetInternalParameters();
            if (mInstruction == null)
                mInstruction = new SayBehaviourInstruction();
            SayBehaviourInstruction lSayInstruction = (SayBehaviourInstruction)mInstruction;
            //if(lSayInstruction.Utterance!=null)
                Utterance = lSayInstruction.Utterance.Value;
        }

        public string GetEditableParameter()
        {
            return Utterance;
        }

        public void SetEditableParameter(string iParameter)
        {
            Utterance = iParameter;
        }
    }
}