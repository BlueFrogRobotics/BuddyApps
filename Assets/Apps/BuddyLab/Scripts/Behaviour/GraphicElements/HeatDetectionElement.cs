using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class HeatDetectionElement : AGraphicElement
    {
        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ThermalInputBehaviourInstruction();
            //ThermalInputBehaviourInstruction lThermalInstruction = (ThermalInputBehaviourInstruction)mInstruction;
        }
    }
}