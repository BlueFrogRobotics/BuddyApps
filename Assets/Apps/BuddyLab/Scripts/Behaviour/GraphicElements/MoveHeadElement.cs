using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class MoveHeadElement : AGraphicElement, IEditableParameter
    {
        public enum Axe : int
        {
            YES = 0,
            NO = 1
        }

        [SerializeField]
        private float angle;

        [SerializeField]
        private float speed;

        [SerializeField]
        private Axe axe;

        [SerializeField]
        private Text textValue;


        protected override void SetParameter()
        {
            Debug.Log("set param move head: "+angle);
            if (mInstruction == null)
                mInstruction = new MoveHeadBehaviourInstruction();
            MoveHeadBehaviourInstruction lMoveHeadInstruction = (MoveHeadBehaviourInstruction)mInstruction;
            lMoveHeadInstruction.CustomMove = true;
            if (axe == Axe.YES)
            {
                lMoveHeadInstruction.YesAngle = angle;
                lMoveHeadInstruction.YesSpeed = speed;
                lMoveHeadInstruction.NoAngle = float.MinValue;
                lMoveHeadInstruction.NoSpeed = 0F;
            }
            else
            {
                lMoveHeadInstruction.NoAngle = angle;
                lMoveHeadInstruction.NoSpeed = speed;
                lMoveHeadInstruction.YesAngle = float.MinValue;
                lMoveHeadInstruction.YesSpeed = 0F;
            }


        }

        public string GetEditableParameter()
        {
            string lParam = "";
            lParam = "" + angle;

            return lParam;
        }

        public void SetEditableParameter(string iParameter)
        {
            Debug.Log("set editable head");
            float.TryParse(iParameter, out angle);
        }

        protected override void SetInternalParameters()
        {
            Debug.Log("set internal parameter head");
            MoveHeadBehaviourInstruction lMoveHeadInstruction = (MoveHeadBehaviourInstruction)mInstruction;
            float lValue = 0.0F;
            switch (axe)
            {
                case Axe.YES:
                    angle = lMoveHeadInstruction.YesAngle.Value;
                    lValue = angle;
                    break;
                case Axe.NO:
                    angle = lMoveHeadInstruction.NoAngle.Value;
                    lValue = angle;
                    break;
                default:
                    break;
            }
            textValue.text = "" + (lValue * GetComponent<BMLParameterManager>().DivisionCoeff);
        }
    }
}