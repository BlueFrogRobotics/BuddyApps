using UnityEngine;
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
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated)
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(2).SetActive(false);
                GetGameObject(1).SetActive(true);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Open_WList");
            mTTS.Say("Quelle type de recette veux tu faire ?");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mTTS.HasFinishedTalking())
                mTTS.Silence(0, false);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WList");
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(2).SetActive(true);
            GetGameObject(1).SetActive(false);
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
        }
    }
}