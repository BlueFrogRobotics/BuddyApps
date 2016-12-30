using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.RLGL
{
    public class ColorDotTutorial : MonoBehaviour
    {

        [SerializeField]
        private Scrollbar scrollBar;

        [SerializeField]
        private Button button1;
        [SerializeField]
        private Button button2;
        [SerializeField]
        private Button button3;
        [SerializeField]
        private Button button4;
        [SerializeField]
        private Button button5;

        private TextToSpeech mTTS;
        private Face mFace;

        [SerializeField]
        private GameObject listener;

        // Use this for initialization
        void Start()
        {
            mFace = BYOS.Instance.Face;
            //mFace.SetExpression(MoodType.NEUTRAL);
            mTTS = BYOS.Instance.TextToSpeech;
            scrollBar.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }

        public void SayAtStart()
        {
            listener.GetComponent<RLGLListener>().ErrorCount = 0;
            mFace.SetExpression(MoodType.NEUTRAL);
            if (!mTTS.HasFinishedTalking)
                mTTS.Silence(0);
            mTTS.Say("PLACE YOURSELF AT 26 FEET IN FRONT OF BUDDY");
        }

        public void OnValueChanged()
        {

            if (scrollBar.value >= 0.0F && scrollBar.value < 0.2F)
            {
                if (!mTTS.HasFinishedTalking)
                    mTTS.Silence(0);
                mTTS.Say("PLACE YOURSELF AT 26 FEET IN FRONT OF BUDDY");
                button1.GetComponent<Image>().color = new Color(0, 212, 209, 255);
                button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);

            }
            else if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
            {
                if (!mTTS.HasFinishedTalking)
                    mTTS.Silence(0);
                mTTS.Say("WHEN BUDDY SAYS GREENLIGHT YOU CAN MOVE AND TRY TO TOUCH HIS FACE BEFORE HE SAYS REDLIGHT");
                button2.GetComponent<Image>().color = new Color(0, 212, 209, 255);
                button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
            {
                if (!mTTS.HasFinishedTalking)
                    mTTS.Silence(0);
                mTTS.Say("WARNING WHEN MOVING KEEP YOUR FEET ONE IN FRONT OF THE OTHER");

                button3.GetComponent<Image>().color = new Color(0, 212, 209, 255);
                button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else if (scrollBar.value >= 0.6F && scrollBar.value <= 0.8F)
            {
                if (!mTTS.HasFinishedTalking)
                    mTTS.Silence(0);
                mTTS.Say("WHEN BUDDY SAYS REDLIGHT YOU HAVE TO STOP MOVING AND YOU HAVE TO WAIT UNTIL BUDDY SAYS GREENLIGHT");
                button4.GetComponent<Image>().color = new Color(0, 212, 209, 255);
                button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button5.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else if (scrollBar.value >= 0.8F && scrollBar.value <= 1.0F)
            {
                if (!mTTS.HasFinishedTalking)
                    mTTS.Silence(0);
                mTTS.Say("TOUCH HIS FACE WHEN BUDDY IS NOT LOOKING AT YOU");
                button5.GetComponent<Image>().color = new Color(0, 212, 209, 255);
                button1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button3.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button2.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                button4.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }


        }

        public void OnPreviousStep()
        {

            if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
            {
                scrollBar.value = 0.1F;
            }
            else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
            {
                scrollBar.value = 0.3F;
            }
            else if (scrollBar.value >= 0.6F && scrollBar.value <= 0.8F)
            {
                scrollBar.value = 0.5F;
            }
            else if (scrollBar.value >= 0.8F && scrollBar.value <= 1.0F)
            {
                scrollBar.value = 0.7F;
            }
        }

        public void OnNextStep()
        {
            if (scrollBar.value >= 0.0F && scrollBar.value < 0.20F)
            {
                scrollBar.value = 0.3F;
            }
            else if (scrollBar.value >= 0.2F && scrollBar.value < 0.4F)
            {
                scrollBar.value = 0.5F;
            }
            else if (scrollBar.value >= 0.4F && scrollBar.value < 0.6F)
            {
                scrollBar.value = 0.7F;
            }
            else if (scrollBar.value >= 0.6F && scrollBar.value < 0.8F)
            {
                scrollBar.value = 0.9F;
            }

        }

    }

}
