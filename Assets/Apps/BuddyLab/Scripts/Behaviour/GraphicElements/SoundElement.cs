using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class SoundElement : AGraphicElement
    {
        [SerializeField]
        private SoundSample sound;


        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new PlaySoundBehaviourInstruction();
            PlaySoundBehaviourInstruction lSoundInstruction = (PlaySoundBehaviourInstruction)mInstruction;
            lSoundInstruction.SoundSample = sound;
        }
    }
}