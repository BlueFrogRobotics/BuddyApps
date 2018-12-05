using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class InfinityLoopElement : AGraphicElement
    {

        [SerializeField]
        private GameObject container;

        public GameObject Container { get { return container; } }


        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new InfinitLoopBehaviourInstruction();
            InfinitLoopBehaviourInstruction lInfinityLoopInstruction = (InfinitLoopBehaviourInstruction)mInstruction;

            lInfinityLoopInstruction.SubInstructions.Clear();
            foreach (Transform item in container.transform)
            {
                if (item != null && item.GetComponent<AGraphicElement>() != null)
                    lInfinityLoopInstruction.SubInstructions.Add(item.GetComponent<AGraphicElement>().GetInstruction(true));
            }

        }

    }
}