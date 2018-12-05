using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class LoopElement : AGraphicElement, IEditableParameter
    {
        //[SerializeField]
        public int NumLoop;

        [SerializeField]
        private Text textIteration;

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
            if(container.transform.childCount==0)
            {
                mBlankItem = Instantiate(blankItem);
                mBlankItem.transform.parent = container.transform;
            }
            else if(container.transform.childCount>=2)
            {
                Destroy(mBlankItem);
            }
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new ForLoopBehaviourInstruction();
            ForLoopBehaviourInstruction lForLoopInstruction = (ForLoopBehaviourInstruction)mInstruction;
            lForLoopInstruction.Iterations = NumLoop;
            textIteration.text = "" + NumLoop;
            Debug.Log("setloop: "+NumLoop);
            if (lForLoopInstruction.SubInstructions.Count != container.transform.childCount)
            {
                lForLoopInstruction.SubInstructions.Clear();
                foreach (Transform item in container.transform)
                {
                    if (item != null && item.GetComponent<AGraphicElement>() != null)
                        lForLoopInstruction.SubInstructions.Add(item.GetComponent<AGraphicElement>().GetInstruction(true));
                }
            }

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