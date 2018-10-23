using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class ColorDetectionConditionElement : AGraphicElement
    {
        [SerializeField]
        private Color32 color;

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ColorInputBehaviourInstruction();
            ColorInputBehaviourInstruction lColorInputInstruction = (ColorInputBehaviourInstruction)mInstruction;
            lColorInputInstruction.ConditionalColors = new Color32[1];
            lColorInputInstruction.ConditionalColors.Value[0] = color;
        }
    }
}