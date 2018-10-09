﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class InfinityLoopElement : AGraphicElement
    {

        [SerializeField]
        private GameObject container;

        public GameObject Container { get { return container; } }


        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new InfinitLoopBehaviourInstruction();
            InfinitLoopBehaviourInstruction lInfinityLoopInstruction = (InfinitLoopBehaviourInstruction)mInstruction;
            //if (lForLoopInstruction.SubInstructions.Count != container.transform.childCount)
            //{
            lInfinityLoopInstruction.SubInstructions.Clear();
            foreach (Transform item in container.transform)
            {
                if (item != null && item.GetComponent<AGraphicElement>() != null)
                    lInfinityLoopInstruction.SubInstructions.Add(item.GetComponent<AGraphicElement>().GetInstruction(true));
            }
            //}

        }

    }
}