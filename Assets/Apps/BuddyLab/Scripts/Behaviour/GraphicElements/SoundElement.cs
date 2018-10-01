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

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new PlaySoundBehaviourInstruction();
            PlaySoundBehaviourInstruction lSoundInstruction = (PlaySoundBehaviourInstruction)mInstruction;
            lSoundInstruction.SoundSample = sound;
        }
    }
}