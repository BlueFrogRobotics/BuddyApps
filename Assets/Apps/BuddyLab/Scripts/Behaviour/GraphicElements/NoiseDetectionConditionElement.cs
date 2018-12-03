using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class NoiseDetectionConditionElement : AGraphicElement
    {
        [SerializeField]
        private float threshold;


        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new NoiseInputBehaviourInstruction();
            NoiseInputBehaviourInstruction lNoiseInputInstruction = (NoiseInputBehaviourInstruction)mInstruction;
            lNoiseInputInstruction.Threshold = threshold;
        }
    }
}