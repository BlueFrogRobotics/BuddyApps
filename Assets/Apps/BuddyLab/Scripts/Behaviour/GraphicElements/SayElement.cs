using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.BuddyLab
{
    public class SayElement : AGraphicElement
    {
        [SerializeField]
        private string key;

        [SerializeField]
        private string utterance;

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
            lSayInstruction.Key = key;
            lSayInstruction.Utterance = utterance;
        }
    }
}