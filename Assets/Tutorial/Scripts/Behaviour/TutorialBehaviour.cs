using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.Tutorial
{
    public partial class TutorialBehaviour : MonoBehaviour
    {
        [SerializeField]
        private RawImage rawImage;

        /*
         * API of the robot
         */
        private RGBCam mRGBCam;
        private Motors mMotors;
        private Mood mMood;

        /*
         * Init refs to API and to your app data
         */
        void Start()
        {
            mMotors = BYOS.Instance.Primitive.Motors;
            mRGBCam = BYOS.Instance.Primitive.RGBCam;
            mMood = BYOS.Instance.Interaction.Mood;
        }

        void Update()
        {
            rawImage.texture = mRGBCam.FrameTexture2D;
        }

        /*
        * Basic forward order with a medium speed (degrees / sec) for 2 secs.
        */
        public void Forward()
        {
            mMotors.Wheels.SetWheelsSpeed(150F, 150F, 2000);
        }

        /*
        * Basic backward order with a medium speed (degrees / sec) for 2 secs.
        */
        public void Backward()
        {
            mMotors.Wheels.SetWheelsSpeed(-150F, -150F, 2000);
        }

        /*
        * Change the mood of the robot randomly
        */
        public void RandomMood()
        {
            MoodType lMood = (MoodType)Random.Range(0, 11);
            mMood.Set(lMood);
        }
    }
}