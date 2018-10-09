using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public class MoveBodyElement : AGraphicElement, IEditableParameter
    {
        public enum Movement : int
        {
            TRANSLATION=0,
            ROTATION=1
        }

        [SerializeField]
        private float distance;

        [SerializeField]
        private float angle;

        [SerializeField]
        private float speed;

        [SerializeField]
        private Movement movement;

        [SerializeField]
        private Text textValue;

        public override void Highlight()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new MoveBodyBehaviourInstruction();
            MoveBodyBehaviourInstruction lMoveBodyInstruction = (MoveBodyBehaviourInstruction)mInstruction;
            lMoveBodyInstruction.Distance = distance;
            lMoveBodyInstruction.Angle = angle;
            lMoveBodyInstruction.Speed = speed;
        }

        public string GetEditableParameter()
        {
            string lParam = "";
            switch(movement)
            {
                case Movement.ROTATION:
                    lParam = "" + angle;
                    break;
                case Movement.TRANSLATION:
                    lParam = "" + distance;
                    break;
                default:
                    lParam = "";
                    break;
            }
            return lParam;
        }

        public void SetEditableParameter(string iParameter)
        {
            
            switch (movement)
            {
                case Movement.ROTATION:
                    float.TryParse(iParameter, out angle);
                    break;
                case Movement.TRANSLATION:
                    float.TryParse(iParameter, out distance);
                    break;
                default:
                    break;
            }
            
        }

        protected override void SetInternalParameters()
        {
            MoveBodyBehaviourInstruction lMovementInstruction = (MoveBodyBehaviourInstruction)mInstruction;
            float lValue = 0.0F;
            switch (movement)
            {
                case Movement.ROTATION:
                    angle = lMovementInstruction.Angle.Value;
                    lValue = angle;
                    break;
                case Movement.TRANSLATION:
                    distance = lMovementInstruction.Distance.Value;
                    lValue = distance;
                    break;
                default:
                    break;
            }
            textValue.text = "" + (lValue*GetComponent<BMLParameterManager>().DivisionCoeff);
        }
    }
}