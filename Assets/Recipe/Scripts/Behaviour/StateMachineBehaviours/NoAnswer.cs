using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER NO ANSWER");
            mCheck = false;
            /*if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0)
            {
                Debug.Log("First no answer");
                mFace.SetExpression(MoodType.SAD);
                mCheck = true;
                mTTS.Say(mDictionary.GetString("noanswerrecipe"));
            }
            else
            {*/
                mMood.Set(MoodType.TIRED, false, true);
            //}
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                Debug.Log("NO ANSWER 2");
                mTTS.Say(mDictionary.GetRandomString("noanswerrecipe2"));
                mCheck = true;
            }
            else if (mTTS.HasFinishedTalking && mCheck)
            {
                Debug.Log("Finished Talking");
                iAnimator.SetTrigger("AskRecipeAgain");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT NO ANSWER");
            GetComponent<RecipeBehaviour>().NoAnswerCount++;
        }
    }
}