﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class LoopElement : AGraphicElement
    {
        //[SerializeField]
        public int NumLoop;

        [SerializeField]
        private GameObject container;

        public GameObject Container { get { return container; } }

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
                mInstruction = new ForLoopBehaviourInstruction();
            ForLoopBehaviourInstruction lForLoopInstruction = (ForLoopBehaviourInstruction)mInstruction;
            lForLoopInstruction.Iterations = NumLoop;
            Debug.Log("setloop");
            //if (lForLoopInstruction.SubInstructions.Count != container.transform.childCount)
            //{
                lForLoopInstruction.SubInstructions.Clear();
                foreach (Transform item in container.transform)
                {
                    if (item != null && item.GetComponent<AGraphicElement>() != null)
                        lForLoopInstruction.SubInstructions.Add(item.GetComponent<AGraphicElement>().GetInstruction(true));
                }
            //}

        }
    }
}