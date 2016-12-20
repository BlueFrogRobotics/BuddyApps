using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;

namespace BuddySample
{
    public class FaceController : MonoBehaviour
    {
        [SerializeField]
        private Slider sliderX;

        [SerializeField]
        private Slider sliderY;

        [SerializeField]
        private Text feedbackX;

        [SerializeField]
        private Text feedbackY;

        [SerializeField]
        private Text leftEyeClicked;

        [SerializeField]
        private Text rightEyeClicked;

        [SerializeField]
        private Text mouthClicked;

        private Face mFace;
        private TextToSpeech mTTS;
        private Mood mMood;

        // Use this for initialization
        void Start()
        {
            mMood = BYOS.Instance.Mood;
            mFace = BYOS.Instance.Face;
            mTTS = BYOS.Instance.TextToSpeech;

            sliderX.wholeNumbers = true;
            sliderX.minValue = 0;
            sliderX.maxValue = Screen.width;
            sliderX.value = Screen.width / 2;

            sliderY.wholeNumbers = true;
            sliderY.minValue = 0;
            sliderY.maxValue = Screen.height;
            sliderY.value = Screen.height / 2;
        }

        void Update()
        {
            int lX = (int)sliderX.value;
            int lY = (int)sliderY.value;
            leftEyeClicked.text = mFace.ClickedLeftEye ? "Left eye clicked !" : "Left eye";
            rightEyeClicked.text = mFace.ClickedRightEye ? "Right eye clicked !" : "Right eye";
            mouthClicked.text = mFace.ClickedMouth ? "Mouth clicked clicked !" : "Mouth ";
            feedbackX.text = "X = " + lX;
            feedbackY.text = "Y = " + lY;
            sliderX.maxValue = Screen.width;
            sliderY.maxValue = Screen.height;
            mFace.LookAt(lX, lY);
        }

        public void Speak()
        {
            mTTS.Say("Here a long sentence to make Buddy speak something for a simple example. It seems useless but it is not. Actually, I like trains.");
        }

        public void Yawn()
        {
            mFace.SetEvent(FaceEvent.YAWN);
        }

        public void Screaming()
        {
            mFace.SetEvent(FaceEvent.SCREAM);
        }

        public void Smile()
        {
            mFace.SetEvent(FaceEvent.SMILE);
        }

        public void BlinkRight()
        {
            mFace.SetEvent(FaceEvent.BLINK_RIGHT);
        }

        public void BlinkLeft()
        {
            mFace.SetEvent(FaceEvent.BLINK_LEFT);
        }

        public void BlinkDouble()
        {
            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
        }

        public void Neutral()
        {
            mMood.Set(MoodType.NEUTRAL);
        }

        public void Angry()
        {
            mMood.Set(MoodType.ANGRY);
        }

        public void Grumpy()
        {
            mMood.Set(MoodType.GRUMPY);
        }

        public void Love()
        {
            mMood.Set(MoodType.LOVE);
        }

        public void Happy()
        {
            mMood.Set(MoodType.HAPPY);
        }

        public void Listening()
        {
            mMood.Set(MoodType.LISTENING);
        }

        public void Sad()
        {
            mMood.Set(MoodType.SAD);
        }

        public void Scared()
        {
            mMood.Set(MoodType.SCARED);
        }

        public void Sick()
        {
            mMood.Set(MoodType.SICK);
        }

        public void Surprised()
        {
            mMood.Set(MoodType.SURPRISED);
        }

        public void Think()
        {
            mMood.Set(MoodType.THINKING);
        }

        public void Tired()
        {
            mMood.Set(MoodType.TIRED);
        }
    }
}
