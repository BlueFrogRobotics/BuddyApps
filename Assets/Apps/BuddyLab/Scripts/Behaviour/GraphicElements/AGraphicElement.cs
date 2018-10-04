using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public abstract class AGraphicElement : MonoBehaviour
    {

        protected ABehaviourInstruction mInstruction;

        public ABehaviourInstruction GetInstruction(bool iSetParam)
        {
            if(iSetParam)
                SetParameter();

            return mInstruction;
        }

        public void SetInstruction(ABehaviourInstruction iInstruction)
        {
            mInstruction = iInstruction;
            SetInternalParameters();
        }

        protected abstract void SetParameter();

        protected virtual void SetInternalParameters()
        {

        }

        public abstract void Highlight();

        //public abstract void Lowlight();
    }
}