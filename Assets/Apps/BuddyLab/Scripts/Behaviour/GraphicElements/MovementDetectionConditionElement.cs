using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class MovementDetectionConditionElement : AGraphicElement
    {
        [SerializeField]
        private float threshold;

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new MotionInputBehaviourInstruction();
            MotionInputBehaviourInstruction lMotionInputInstruction = (MotionInputBehaviourInstruction)mInstruction;
            lMotionInputInstruction.Threshold = threshold;
        }
    }
}