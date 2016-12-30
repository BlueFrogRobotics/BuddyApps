using UnityEngine;
using BuddyOS;
using BuddyOS.Command;
using System.Collections;

namespace BuddyApp.Companion
{
    public class IdleReaction : MonoBehaviour
    {
        public float HeadPosition { get { return mHeadPos; } set { mHeadPos = value; } }

        [SerializeField]
        private Emotion mEmotion;

        private bool mInitialized;
        private float mHeadMoveTime;
        private float mEmoteTime;
        private float mHeadPos;
        private float mTTSTime;
        private float mRandomSpeechTime;

        private Dictionary mDict;
        private Face mFace;
        private Mood mMood;
        private TextToSpeech mTTS;
        private NoHinge mNoHinge;
        private YesHinge mYesHinge;

        void Start()
        {
            mInitialized = true;
            mHeadPos = -10F;
            mDict = BYOS.Instance.Dictionary;
            mFace = BYOS.Instance.Face;
            mMood = BYOS.Instance.Mood;
            mTTS = BYOS.Instance.TextToSpeech;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mYesHinge = BYOS.Instance.Motors.YesHinge;
        }

        void OnEnable()
        {
            if(!mInitialized) {
                Start();
                mInitialized = true;
            }

            mHeadMoveTime = Time.time;
            mEmoteTime = Time.time;
            mMood.Set(MoodType.NEUTRAL);
        }

        void Update()
        {
            if (Time.time - mHeadMoveTime < 1.5F)
                return;

            mHeadMoveTime = Time.time;

            if (Random.Range(0, 2) == 0) {
                TurnHeadNo();
            } else {
                TurnHeadYes();
            }

            if (Time.time - mTTSTime > mRandomSpeechTime)
                SaySomething();
            
            if (Time.time - mEmoteTime > 8F) {
                switch(Random.Range(0, 7)) {
                    case 0:
                        mFace.SetEvent(FaceEvent.SMILE);
                        break;
                    case 1:
                        mFace.SetEvent(FaceEvent.YAWN);
                        break;
                    case 2:
                        mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
                        break;
                    case 3:
                        StartCoroutine(HappyFaceCo());
                        break;
                    case 4:
                        mMood.Set(MoodType.SAD);
                        break;
                    case 5:
                        mMood.Set(MoodType.HAPPY);
                        break;
                    case 6:
                        mMood.Set(MoodType.NEUTRAL);
                            break;
                }
                mEmoteTime = Time.time;
            }
        }

        void OnDisable()
        {
            mNoHinge.SetPosition(0F);
            mYesHinge.SetPosition(0F);
        }

        private void TurnHeadYes()
        {
            float lHeadYes = Random.Range(mHeadPos - 10F, mHeadPos + 20F);
            mYesHinge.SetPosition(lHeadYes);
        }

        private void TurnHeadNo()
        {
            float lHeadNo = Random.Range(5F, 35F);

            if (mNoHinge.CurrentAnglePosition > 0F)
                lHeadNo = -lHeadNo;

            mNoHinge.SetPosition(lHeadNo);
        }

        private IEnumerator HappyFaceCo()
        {
            mMood.Set(MoodType.HAPPY);
            yield return new WaitForSeconds(1.0F);
            mMood.Set(MoodType.NEUTRAL);
        }

        private void SaySomething()
        {
            string lSentence = "";
            switch (Random.Range(0, 4))
            {
                case 0:
                    lSentence = mDict.GetString("iAmBored");
                    break;
                case 1:
                    lSentence = mDict.GetString("playWithMe");
                    break;
                case 2:
                    lSentence = mDict.GetString("iWantCuddle");
                    break;
                case 3:
                    lSentence = mDict.GetString("iWantToTalk");
                    break;
            }
            mTTS.Say(lSentence, true);
            mTTSTime = Time.time;
            mRandomSpeechTime = Random.Range(40F, 80F);
        }
    }
}