using UnityEngine;

// Use this to access API through any application
using BuddyOS;

namespace BuddySample
{
    public class BasicAIBehaviour : MonoBehaviour
    {
        private TextToSpeech mTextToSpeech;
        private Motors mMotors;
        private Face mFace;

        // Use this for initialization
        void Start()
        {
            mMotors = BYOS.Instance.Motors;
            mTextToSpeech = BYOS.Instance.TextToSpeech;
            mFace = BYOS.Instance.Face;
        }

        public void Speak()
        {
            mTextToSpeech.Say("Hello world");
        }

        public void Forward()
        {
            mMotors.Wheels.SetWheelsSpeed(150F, 150F, 2000);
        }

        public void Backward()
        {
            mMotors.Wheels.SetWheelsSpeed(-150F, -150F, 2000);
        }

        public void RandomMood()
        {
            FaceMood lMood = (FaceMood)Random.Range(0, 11);
            mFace.SetMood(lMood);
        }
    }
}