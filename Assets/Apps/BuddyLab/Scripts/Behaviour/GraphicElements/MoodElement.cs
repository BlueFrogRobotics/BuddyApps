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


        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new SetMoodBehaviourInstruction();
            SetMoodBehaviourInstruction lMoodInstruction = (SetMoodBehaviourInstruction)mInstruction;
            //lMoodInstruction.Duration = 1.5F;
            lMoodInstruction.Mood = mood;
        }

    }
}