using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    /// <summary>
    /// Abstract class that enables to edit and add BehaviourInstruction within the editor
    /// </summary>
    public abstract class AGraphicElement : MonoBehaviour
    {

        /// <summary>
        /// Instruction referenced
        /// </summary>
        protected ABehaviourInstruction mInstruction;

        /// <summary>
        /// Returns the behaviour instruction and set its parameters if asked
        /// </summary>
        /// <param name="iSetParam">Will set the instructions parameters if set to true</param>
        /// <returns></returns>
        public ABehaviourInstruction GetInstruction(bool iSetParam)
        {
            if(iSetParam)
                SetParameter();

            return mInstruction;
        }

        /// <summary>
        /// Set the internal behaviour instruction
        /// </summary>
        /// <param name="iInstruction">the instruction</param>
        public void SetInstruction(ABehaviourInstruction iInstruction)
        {
            mInstruction = iInstruction;
            SetInternalParameters();
        }

        /// <summary>
        /// Set the instruction parameters
        /// </summary>
        protected abstract void SetParameter();

        /// <summary>
        /// Set the element parameters using the behaviour instruction parameters
        /// </summary>
        protected virtual void SetInternalParameters()
        {

        }

        
    }
}