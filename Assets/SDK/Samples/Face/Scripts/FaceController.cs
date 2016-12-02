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
        private LED mLED;
        private TextToSpeech mTTS;

        // Use this for initialization
        void Start()
        {
            mLED = BYOS.Instance.LED;
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
            leftEyeClicked.text = "Left eye clicked ? " + mFace.ClickedLeftEye;
            rightEyeClicked.text = "Right eye clicked ? " + mFace.ClickedRightEye;
            mouthClicked.text = "Mouth clicked ? " + mFace.ClickedMouth;
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
            mFace.SetMouthEvent(MouthEvent.YAWN);
        }

        public void Screaming()
        {
            mFace.SetMouthEvent(MouthEvent.SCREAM);
        }

        public void Smile()
        {
            mFace.SetMouthEvent(MouthEvent.SMILE);
        }

        public void Neutral()
        {
            mFace.SetMood(FaceMood.NEUTRAL);
            mLED.SetBodyLight(LEDColor.BLUE_NEUTRAL);
        }

        public void Angry()
        {
            mFace.SetMood(FaceMood.ANGRY);
            mLED.SetBodyLight(LEDColor.RED_ANGRY);
        }

        public void Grumpy()
        {
            mFace.SetMood(FaceMood.GRUMPY);
            mLED.SetBodyLight(LEDColor.PURPLE_GRUMPY);
        }

        public void Happy()
        {
            mFace.SetMood(FaceMood.HAPPY);
            mLED.SetBodyLight(LEDColor.ORANGE_HAPPY);
        }

        public void Listening()
        {
            mFace.SetMood(FaceMood.LISTENING);
            mLED.SetBodyLight(LEDColor.BLUE_LISTENING);
        }

        public void Sad()
        {
            mFace.SetMood(FaceMood.SAD);
            mLED.SetBodyLight(LEDColor.PURPLE_SAD);
        }

        public void Scared()
        {
            mFace.SetMood(FaceMood.SCARED);
            mLED.SetBodyLight(LEDColor.ORANGE_SCARY);
        }

        public void Sick()
        {
            mFace.SetMood(FaceMood.SICK);
            mLED.SetBodyLight(LEDColor.GREEN_SICK);
        }

        public void Surprised()
        {
            mFace.SetMood(FaceMood.SURPRISED);
            mLED.SetBodyLight(LEDColor.YELLOW_SURPRISED);
        }

        public void Think()
        {
            mFace.SetMood(FaceMood.THINKING);
            mLED.SetBodyLight(LEDColor.GREEN_THINKING);
        }

        public void Tired()
        {
            mFace.SetMood(FaceMood.TIRED);
            mLED.SetBodyLight(LEDColor.GREY_TIRED);
        }
    }
}
