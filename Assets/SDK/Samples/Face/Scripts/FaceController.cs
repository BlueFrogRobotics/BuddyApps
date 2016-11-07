using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddySample
{
    public class FaceController : MonoBehaviour
    {
        private Face mFace;
        private LED mLED;

        // Use this for initialization
        void Start()
        {
            mLED = BYOS.Instance.LED;
            mFace = BYOS.Instance.Face;
        }

        public void Neutral()
        {
            mFace.SetMood(FaceMood.NEUTRAL);
            mLED.SetBodyLight(MoodColor.BLUE_NEUTRAL);
        }

        public void Happy()
        {
            mFace.SetMood(FaceMood.HAPPY);
            mLED.SetBodyLight(MoodColor.ORANGE_HAPPY);
        }

        public void Sad()
        {
            mFace.SetMood(FaceMood.SAD);
            mLED.SetBodyLight(MoodColor.PURPLE_SAD);
        }

        public void Angry()
        {
            mFace.SetMood(FaceMood.ANGRY);
            mLED.SetBodyLight(MoodColor.RED_ANGRY);
        }
    }
}