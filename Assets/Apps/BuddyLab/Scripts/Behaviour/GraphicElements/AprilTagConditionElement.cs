using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class AprilTagConditionElement : AGraphicElement
    {
        [SerializeField]
        private int id;

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new AprilTagInputBehaviourInstruction();
            AprilTagInputBehaviourInstruction lColorInputInstruction = (AprilTagInputBehaviourInstruction)mInstruction;
            lColorInputInstruction.ConditionalContents = new int[1];
            lColorInputInstruction.ConditionalContents.Value[0] = id;
        }
    }
}