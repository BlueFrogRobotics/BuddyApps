using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class HeadForcedDetector : MonoBehaviour
    {
        public bool HeadForcedDetected { get { return mHeadForcedDetected; } }

        [SerializeField]
        private Text currentAngle;

        [SerializeField]
        private Text destinationAngle;

        [SerializeField]
        private Text targetSpeed;

        private bool mHeadForcedDetected;
        private Hinge mHinge;

        void Start()
        {
            mHinge = BYOS.Instance.Motors.NoHinge;
            mHeadForcedDetected = false;
        }
        
        void Update()
        {
            currentAngle.text = mHinge.CurrentAnglePosition.ToString();
            destinationAngle.text = mHinge.DestinationAnglePosition.ToString();
            targetSpeed.text = mHinge.TargetSpeed.ToString();

            if (Mathf.Abs(mHinge.CurrentAnglePosition - mHinge.DestinationAnglePosition) > 5.0F)
                mHeadForcedDetected = true;
            else
                mHeadForcedDetected = false;
        }

        public void MoveHeadLeft()
        {
            float lDestination = mHinge.DestinationAnglePosition + 10.0F;
            mHinge.SetPosition(lDestination);
        }

        public void MoveHeadRight()
        {
            float lDestination = mHinge.DestinationAnglePosition - 10.0F;
            mHinge.SetPosition(lDestination);
        }
    }
}