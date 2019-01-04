using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Condition element that test if the corresponding color is shown
    /// <para>See <see cref="ColorInputBehaviourInstruction"/>  </para>
    /// </summary>
    public sealed class ColorDetectionConditionElement : AGraphicElement
    {
        /// <summary>
        /// The color to recognize
        /// </summary>
        [SerializeField]
        private Color32 color;

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ColorInputBehaviourInstruction();
            ColorInputBehaviourInstruction lColorInputInstruction = (ColorInputBehaviourInstruction)mInstruction;
            lColorInputInstruction.ConditionalColors = new Color32[1];
            lColorInputInstruction.ConditionalColors.Value[0] = color;
            Debug.Log("shade color: "+ Utils.GetNearestColor(color));
        }
    }
}