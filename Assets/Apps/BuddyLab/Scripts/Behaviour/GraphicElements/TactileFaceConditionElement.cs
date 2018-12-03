using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class TactileFaceConditionElement : AGraphicElement
    {
        [SerializeField]
        private FacialPart facialPart;

        [SerializeField]
        private bool allParts;

        protected override void SetParameter()
        {

            if (mInstruction == null)
                mInstruction = new FaceInteractionInputBehaviourInstruction();
            FaceInteractionInputBehaviourInstruction lFaceInteractionInstruction = (FaceInteractionInputBehaviourInstruction)mInstruction;
            FacialPart[] lFacialParts = new FacialPart[1];
            lFacialParts[0] = facialPart;
            lFaceInteractionInstruction.ConditionalParts = lFacialParts;
            lFaceInteractionInstruction.AnyPart = allParts;
        }
    }
}