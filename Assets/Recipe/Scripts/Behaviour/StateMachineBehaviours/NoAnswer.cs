using UnityEngine;
using BuddyOS.App;
using System.Collections;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {
        bool mCheck;
        bool mtest;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER NO ANSWER");
            mtest = false;
            mCheck = false;
            if (GetComponent<RecipeBehaviour>().NoAnswerCount == 0)
            {
                StartCoroutine(test());
                /*mCheck = true;
                mMood.Set(MoodType.SAD);
                mTTS.Say(mDictionary.GetString("noanswerrecipe"));*/
                
            }
            else
                mMood.Set(MoodType.TIRED, false, true);
        }

        private IEnumerator test()
        {
            mCheck = true;
            //mMood.Set(MoodType.SAD);
            Debug.Log("before wait");
            yield return new WaitForSeconds(2F);
            Debug.Log("after wait");
            /*mTTS.Say(mDictionary.GetString("noanswerrecipe"));
            mtest = true;*/
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mSpeaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                Debug.Log("NO ANSWER 2");
                mTTS.Say(mDictionary.GetString("noanswerrecipe2"));
                mCheck = true;
                mtest = true;
            }
            if (mTTS.HasFinishedTalking && mCheck && mtest)
            {
                Debug.Log("EXIT NO ANSWER");
                iAnimator.SetTrigger("AskRecipeAgain");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().NoAnswerCount++;
        }
    }
}