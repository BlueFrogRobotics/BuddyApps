using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class MoodElement : AGraphicElement
    {

        [SerializeField]
        private Mood mood;

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
                mInstruction = new SetMoodBehaviourInstruction();
            SetMoodBehaviourInstruction lMoodInstruction = (SetMoodBehaviourInstruction)mInstruction;
            lMoodInstruction.Mood = mood;
        }

    }
}