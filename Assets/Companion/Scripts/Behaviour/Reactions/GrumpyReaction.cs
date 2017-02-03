using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Buddy makes some animation after his head being forced
    /// </summary>
    public class GrumpyReaction : MonoBehaviour
    {
        public float AnimationTime { get { return mAnimTime; } }

        [SerializeField]
        private Emotion mEmotion;
        
        private float mAnimTime;
        private float mAnimStartTime;
        private TextToSpeech mTTS;
        private Dictionary mDict;

        void Start()
        {
            mTTS = BYOS.Instance.TextToSpeech;
            mDict = BYOS.Instance.Dictionary;
        }

        void OnEnable()
        {
            if (mTTS == null)
                Start();

            mEmotion.EnableChoregraph();
            mAnimStartTime = Time.time;
            //At random, Buddy is either grumpy, mad or is happy because of "cuddles"
            switch(Random.Range(0, 3))
            {
                case 0:
                    mAnimTime = 6.0F;
                    mEmotion.SetEmotion(EmotionType.GRUMPY);
                    mEmotion.SetEvent(EmotionEvent.GRUMPY);
                    break;
                case 1:
                    mAnimTime = 4.5F;
                    mEmotion.SetEmotion(EmotionType.ANGRY);
                    mEmotion.SetEvent(EmotionEvent.ANGRY);
                    break;
                case 2:
                    mAnimTime = 4.0F;
                    mEmotion.SetEmotion(EmotionType.HAPPY);
                    mEmotion.SetEvent(EmotionEvent.LAUGH);
                    mTTS.Say(mDict.GetString("cuddle"));
                    break;
            }
        }
        
        void Update()
        {
            if(Time.time - mAnimStartTime > mAnimTime) {
                mEmotion.SetEmotion(EmotionType.NEUTRAL);
                enabled = false;
            }
        }

        void OnDisable()
        {
            mEmotion.DisableChoregraph();
            GetComponent<Reaction>().ActionFinished();
        }
    }
}