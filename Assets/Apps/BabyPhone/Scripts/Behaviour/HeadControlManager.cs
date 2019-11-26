using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BabyPhone
{
    public class HeadControlManager : MonoBehaviour
    {
        private float HEAD_ANGLE_INCREMENT = 5F;

        [SerializeField]
        private Animator mAnimator;

        private void Start()
        {
            BabyPhoneData.Instance.HeadPosition = Buddy.Actuators.Head.Yes.Angle;
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
            BabyPhoneData.Instance.HeadPosition += HEAD_ANGLE_INCREMENT;
            if (BabyPhoneData.Instance.HeadPosition > 90)
                BabyPhoneData.Instance.HeadPosition = 90;
            HeadYesSetPosition(BabyPhoneData.Instance.HeadPosition);
            DebugColor("HEAD UP: " + BabyPhoneData.Instance.HeadPosition, "blue");
        }

        public void HeadDown()
        {
            BabyPhoneData.Instance.HeadPosition -= HEAD_ANGLE_INCREMENT;
            if (BabyPhoneData.Instance.HeadPosition < 0)
                BabyPhoneData.Instance.HeadPosition = 0;
            HeadYesSetPosition(BabyPhoneData.Instance.HeadPosition);
            DebugColor("HEAD DOWN: " + BabyPhoneData.Instance.HeadPosition, "red");
        }

        public void HeadValidate()
        {
            mAnimator.SetTrigger("ObservationSettings");
        }

        public void HeadReset()
        {
            BabyPhoneData.Instance.HeadPosition = 0F;
            HeadYesSetPosition(BabyPhoneData.Instance.HeadPosition);
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
