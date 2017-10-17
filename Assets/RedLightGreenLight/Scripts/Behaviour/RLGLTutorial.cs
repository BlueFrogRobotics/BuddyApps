using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLTutorial : MonoBehaviour
    {

        [SerializeField]
        private GameObject Listener;

        private TextToSpeech mTTS;
        private SpeechToText mSTT;
        private int mCountStep;
        private bool mIsQuestionDone;

        private Dictionary mDictionnary;

        private bool mIsAnswerPlayYes;
        public bool IsAnswerPlayYes { get { return mIsAnswerPlayYes; } set { mIsAnswerPlayYes = value; } }

        private float mTimer;

        // Use this for initialization
        void Start()
        {
            mDictionnary = BYOS.Instance.Dictionary;
            mSTT = BYOS.Instance.Interaction.SpeechToText;
            mTTS = BYOS.Instance.Interaction.TextToSpeech;
            mIsQuestionDone = false;
            mCountStep = 0;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mCountStep == 4 && mTTS.HasFinishedTalking && !mIsQuestionDone)
            {
                
                mTTS.Say(mDictionnary.GetRandomString("tutoriel1"));
                mIsQuestionDone = true;
            }


            if (mCountStep == 4 && mTimer > 5.0F && mSTT.HasFinished)
            {
                mTimer = 0.0F;
                Listener.GetComponent<RLGLListener>().STTRequest(5);
            }
        }
    }

}
