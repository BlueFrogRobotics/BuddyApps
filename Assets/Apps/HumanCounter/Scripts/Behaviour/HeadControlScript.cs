using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.HumanCounter
{
    public class HeadControlScript : MonoBehaviour
    {
        private float HEAD_ANGLE_INCREMENT = 5F;

        [SerializeField]
        private Animator mAnimator;

        private void Start()
        {
            HumanCounterData.Instance.HeadPosition = Buddy.Actuators.Head.Yes.Angle;
        }

        private void HeadYesSetPosition(float iAngle)
        {
            if (iAngle < 0 || iAngle > 90)
            {
                Debug.Log("Choose an angle between 0 and 90 degrees.");
                return;
            }
            try
            {
                Buddy.Actuators.Head.Yes.SetPosition(iAngle);
            }
            catch
            {
                Debug.Log("HeadYesSetPosition: Enter a new float");
            }
        }

        public void HeadUp()
        {
            HumanCounterData.Instance.HeadPosition += HEAD_ANGLE_INCREMENT;
            if (HumanCounterData.Instance.HeadPosition > 90)
                HumanCounterData.Instance.HeadPosition = 90;
            DebugColor("HEAD UP: " + HumanCounterData.Instance.HeadPosition, "blue") ;
            HeadYesSetPosition(HumanCounterData.Instance.HeadPosition);
        }

        public void HeadDown()
        {
            HumanCounterData.Instance.HeadPosition -= HEAD_ANGLE_INCREMENT;
            if (HumanCounterData.Instance.HeadPosition < 0)
                HumanCounterData.Instance.HeadPosition = 0;
            DebugColor("HEAD DOWN: " + HumanCounterData.Instance.HeadPosition, "red");
            HeadYesSetPosition(HumanCounterData.Instance.HeadPosition);
        }

        public void HeadValidate()
        {
            DebugColor("VALIDATE", "green");
            mAnimator.SetTrigger("ObservationSettings");
        }

        public void HeadReset()
        {
            DebugColor("RESET POSITION", "green");
            HumanCounterData.Instance.HeadPosition = 0F;
            HeadYesSetPosition(HumanCounterData.Instance.HeadPosition);
        }

        // TMP
        public void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }
    }
}
