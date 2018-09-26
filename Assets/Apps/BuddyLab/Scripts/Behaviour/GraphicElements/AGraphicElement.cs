using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public abstract class AGraphicElement : MonoBehaviour
    {

        protected ASealedBehaviourInstruction mInstruction;

        public ASealedBehaviourInstruction GetInstruction()
        {
            SetParameter();

            return mInstruction;
        }

        public void SetInstruction(ASealedBehaviourInstruction iInstruction)
        {
            mInstruction = iInstruction;
        }

        protected abstract void SetParameter();

        public abstract void Highlight();
    }
}