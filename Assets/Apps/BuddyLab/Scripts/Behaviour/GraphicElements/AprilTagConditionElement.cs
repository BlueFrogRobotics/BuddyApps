using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Condition element that test if the corresponding april tag is shown
    /// <para>See <see cref="AprilTagInputBehaviourInstruction"/>  </para>
    /// </summary>
    public sealed class AprilTagConditionElement : AGraphicElement
    {
        /// <summary>
        /// Id of the april tag
        /// </summary>
        [SerializeField]
        private int id;

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