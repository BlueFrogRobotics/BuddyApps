using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class LoopElement : AGraphicElement, IEditableParameter
    {
        //[SerializeField]
        public int NumLoop;

        [SerializeField]
        private Text textIteration;

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
            textIteration.text = "" + NumLoop;
            Debug.Log("setloop: "+NumLoop);
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

        public string GetEditableParameter()
        {
            return "" + NumLoop;
        }

        public void SetEditableParameter(string iParameter)
        {
            int.TryParse(iParameter, out NumLoop);
        }

        protected override void SetInternalParameters()
        {
            ForLoopBehaviourInstruction lForLoopInstruction = (ForLoopBehaviourInstruction)mInstruction;
            NumLoop = lForLoopInstruction.Iterations.Value;
            textIteration.text = "" + NumLoop;
        }
    }
}