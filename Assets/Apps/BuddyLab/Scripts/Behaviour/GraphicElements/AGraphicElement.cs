using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public abstract class AGraphicElement : MonoBehaviour
    {

        protected ABehaviourInstruction mInstruction;

        public ABehaviourInstruction GetInstruction()
        {
            SetParameter();

            return mInstruction;
        }

        public void SetInstruction(ABehaviourInstruction iInstruction)
        {
            mInstruction = iInstruction;
        }

        protected abstract void SetParameter();

        public abstract void Highlight();
    }
}