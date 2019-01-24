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

        [SerializeField]
        private GameObject blankItem;

        private GameObject mBlankItem;

        public GameObject Container { get { return container; } }

        // Use this for initialization
        void Start()
        {
            mBlankItem = container.transform.GetChild(0).gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (container.transform.childCount == 0) {
                mBlankItem = Instantiate(blankItem);
                mBlankItem.transform.parent = container.transform;
            } else if (container.transform.childCount >= 2) {
                Destroy(mBlankItem);
            }
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new InfinitLoopBehaviourInstruction();
            InfinitLoopBehaviourInstruction lInfinityLoopInstruction = (InfinitLoopBehaviourInstruction)mInstruction;

            //if (lInfinityLoopInstruction.SubInstructions.Count != container.transform.childCount) 
            {
                lInfinityLoopInstruction.SubInstructions.Clear();
                foreach (Transform item in container.transform) {
                    if (item != null && item.GetComponent<AGraphicElement>() != null)
                        lInfinityLoopInstruction.SubInstructions.Add(item.GetComponent<AGraphicElement>().GetInstruction(true));
                }
            }
        }

    }
}