using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class ChooseWithScreen : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated) {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(2).SetActive(false);
                GetGameObject(1).SetActive(true);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
            GetGameObject(13).GetComponent<Text>().text = mDictionary.GetString("starter");
            GetGameObject(14).GetComponent<Text>().text = mDictionary.GetString("dish");
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Open_WCategory");
            mTTS.Say(mDictionary.GetRandomString("chooserecipecategory"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mTTS.HasFinishedTalking)
                mTTS.Silence(0);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WCategory");
        }
    }
}