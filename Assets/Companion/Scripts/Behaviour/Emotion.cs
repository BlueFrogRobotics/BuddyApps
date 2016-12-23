using UnityEngine;
using BuddyOS;
using System.Collections;

namespace BuddyApp.Companion
{
    enum EmotionType
    {
        NEUTRAL,
        AFRAID,
        ANGRY,
        CURIOUS,
        FOCUS,
        GRUMPY,
        HAPPY,
        LISTEN,
        SAD,
        SICK,
        SLEEP,
        TIRED
    }

    enum EmotionEvent
    {
        ANGRY,
        BLINK,
        DISCOVER,
        FULLTURN,
        GRUMPY,
        LAUGH,
        LIGHTOFF,
        SAD,
        SCARED,
        SCREAM,
        SHIVERS,
        SHY,
        SIGH,
        SMILE,
        SWALLOW,
        TONGUE,
        YAWN
    }

    public class Emotion : MonoBehaviour
    {        
        //Here is the list of Animator's layer :
        //0. Neutral
        //1. Afraid
        //2. Angry
        //3. Curious
        //4. Focus
        //5. Grumpy
        //6. Happy
        //7. Listen
        //8. Sad
        //9. Sick
        //10. Sleep
        //11. Tired
        //12. BlendLookAt
        //13. BodyMove
        [SerializeField]
        private Animator mAnimator;

        private EmotionType mEmotion;
        private EmotionEvent mEvent;
        private Face mFace;
        private Mood mMood;

        void Start()
        {
            mFace = BYOS.Instance.Face;
            mMood = BYOS.Instance.Mood;
        }

        void Update()
        {

        }

        public void EnableChoregraph()
        {
            GetComponent<Choregraph>().enabled = true;
        }

        public void DisableChoregraph()
        {
            GetComponent<Choregraph>().enabled = false;
        }

        internal void SetEmotion(EmotionType iType)
        {
            ResetLayers();
            mEmotion = iType;

            switch(iType)
            {
                case EmotionType.NEUTRAL:
                    Neutral();
                    break;
                case EmotionType.AFRAID:
                    Afraid();
                    break;
                case EmotionType.ANGRY:
                    Angry();
                    break;
                case EmotionType.CURIOUS:
                    Curious();
                    break;
                case EmotionType.FOCUS:
                    Focus();
                    break;
                case EmotionType.GRUMPY:
                    Grumpy();
                    break;
                case EmotionType.HAPPY:
                    Happy();
                    break;
                case EmotionType.LISTEN:
                    Listen();
                    break;
                case EmotionType.SAD:
                    Sad();
                    break;
                case EmotionType.SICK:
                    Sick();
                    break;
                case EmotionType.SLEEP:
                    Sleep();
                    break;
                case EmotionType.TIRED:
                    Tired();
                    break;
            }
        }

        internal void SetEvent(EmotionEvent iEvent)
        {
            mEvent = iEvent;

            switch(iEvent)
            {
                case EmotionEvent.ANGRY:
                    EventAngry();
                    break;
                case EmotionEvent.BLINK:
                    Blink();
                    break;
                case EmotionEvent.DISCOVER:
                    Discover();
                    break;
                case EmotionEvent.FULLTURN:
                    FullTurn();
                    break;
                case EmotionEvent.GRUMPY:
                    EventGrumpy();
                    break;
                case EmotionEvent.LAUGH:
                    Laugh();
                    break;
                case EmotionEvent.LIGHTOFF:
                    LightOff();
                    break;
                case EmotionEvent.SAD:
                    EventSad();
                    break;
                case EmotionEvent.SCARED:
                    Scared();
                    break;
                case EmotionEvent.SCREAM:
                    Scream();
                    break;
                case EmotionEvent.SHIVERS:
                    Shivers();
                    break;
                case EmotionEvent.SHY:
                    Shy();
                    break;
                case EmotionEvent.SIGH:
                    Sigh();
                    break;
                case EmotionEvent.SMILE:
                    Smile();
                    break;
                case EmotionEvent.SWALLOW:
                    Swallow();
                    break;
                case EmotionEvent.TONGUE:
                    Tongue();
                    break;
                case EmotionEvent.YAWN:
                    Yawn();
                    break;
            }
        }

        private void ResetLayers()
        {
            for (int i = 0; i < 12; i++)
            {
                mAnimator.SetLayerWeight(i, 0F);
            }
        }

        private void Neutral()
        {
            mAnimator.SetLayerWeight(0, 1F);
            mMood.Set(MoodType.NEUTRAL);
        }

        private void Afraid()
        {
            mAnimator.SetLayerWeight(1, 1F);
            mMood.Set(MoodType.SCARED);
        }

        private void Angry()
        {
            mAnimator.SetLayerWeight(2, 1F);
            mMood.Set(MoodType.ANGRY);
        }

        private void Curious()
        {
            mAnimator.SetLayerWeight(3, 1F);
            mMood.Set(MoodType.NEUTRAL);
        }

        private void Focus()
        {
            mAnimator.SetLayerWeight(4, 1F);
            mMood.Set(MoodType.THINKING);
        }

        private void Grumpy()
        {
            mAnimator.SetLayerWeight(5, 1F);
            mMood.Set(MoodType.GRUMPY);
        }

        private void Happy()
        {
            mAnimator.SetLayerWeight(6, 1F);
            mMood.Set(MoodType.HAPPY);
        }

        private void Listen()
        {
            mAnimator.SetLayerWeight(7, 1F);
            mMood.Set(MoodType.LISTENING);
        }

        private void Sad()
        {
            mAnimator.SetLayerWeight(8, 1F);
            mMood.Set(MoodType.SAD);
        }

        private void Sick()
        {
            mAnimator.SetLayerWeight(9, 1F);
            mMood.Set(MoodType.SICK);
        }

        private void Sleep()
        {
            mAnimator.SetLayerWeight(10, 1F);
            mMood.Set(MoodType.TIRED);
        }

        private void Tired()
        {
            mAnimator.SetLayerWeight(11, 1F);
            mMood.Set(MoodType.TIRED);
        }
                

        private void EventAngry()
        {
            if(mEmotion == EmotionType.ANGRY)
                mAnimator.SetTrigger("Angry");
        }

        private void Blink()
        {
            mAnimator.SetTrigger("Blink");
            mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
        }

        private void Discover()
        {
            mAnimator.SetTrigger("Discover");
        }

        private void FullTurn()
        {
            mAnimator.SetTrigger("FullTurn");
        }

        private void EventGrumpy()
        {
            if(mEmotion == EmotionType.GRUMPY)
                mAnimator.SetTrigger("Grumpy");
        }

        private void Laugh()
        {
            if (mEmotion == EmotionType.HAPPY) {
                mAnimator.SetTrigger("Laugh");
                mFace.SetEvent(FaceEvent.SCREAM);
            }
        }

        private void LightOff()
        {
            if (mEmotion == EmotionType.SLEEP)
                mAnimator.SetTrigger("LightOFF");
        }

        private void EventSad()
        {
            if (mEmotion == EmotionType.SAD)
                mAnimator.SetTrigger("Sad");
        }

        private void Scared()
        {
            if(mEmotion == EmotionType.AFRAID)
                mAnimator.SetTrigger("Scared");
        }

        private void Scream()
        {
            if (mEmotion == EmotionType.ANGRY) {
                mAnimator.SetTrigger("Scream");
                mFace.SetEvent(FaceEvent.SCREAM);
            }
        }

        private void Shivers()
        {
            if(mEmotion == EmotionType.SICK)
                mAnimator.SetTrigger("Shivers");
        }

        private void Shy()
        {
            mAnimator.SetTrigger("Shy");
        }

        private void Sigh()
        {
            if(mEmotion != EmotionType.ANGRY || mEmotion != EmotionType.CURIOUS || mEmotion != EmotionType.GRUMPY
                || mEmotion != EmotionType.SICK || mEmotion != EmotionType.SLEEP) {
                mAnimator.SetTrigger("Sigh");
                mFace.SetEvent(FaceEvent.YAWN);
            }
        }

        private void Smile()
        {
            if(mEmotion == EmotionType.NEUTRAL || mEmotion == EmotionType.CURIOUS || mEmotion == EmotionType.FOCUS
                || mEmotion == EmotionType.HAPPY || mEmotion == EmotionType.LISTEN || mEmotion == EmotionType.TIRED) {
                mAnimator.SetTrigger("Smile");
                mFace.SetEvent(FaceEvent.SMILE);
            }
        }
        
        private void Swallow()
        {
            if(mEmotion == EmotionType.AFRAID) {
                mAnimator.SetTrigger("Swallow");
                mFace.SetEvent(FaceEvent.SCREAM);
            }
        }

        private void Tongue()
        {
            if(mEmotion == EmotionType.GRUMPY) {
                mAnimator.SetTrigger("Tongue");
                mFace.SetEvent(FaceEvent.SCREAM);
            }
        }

        private void Yawn()
        {
            if(mEmotion == EmotionType.TIRED) {
                mAnimator.SetTrigger("Yawn");
                mFace.SetEvent(FaceEvent.YAWN);
            }
        }
    }
}