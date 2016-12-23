using UnityEngine;

namespace BuddyApp.Companion
{
    public class GrumpyReaction : MonoBehaviour
    {
        public float AnimationTime { get { return mAnimTime; } }

        [SerializeField]
        private Emotion mEmotion;

        private float mAnimTime;

        private float mAnimStartTime;

        void OnEnable()
        {
            mEmotion.EnableChoregraph();
            mAnimStartTime = Time.time;
            switch(Random.Range(0, 2))
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