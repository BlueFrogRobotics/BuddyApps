using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class AskAnotherRecipe : AStateMachineBehaviour
    {
        private bool check;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            check = false;
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
            GetGameObject(1).SetActive(true);
            GetGameObject(2).SetActive(false);
            GetGameObject(15).GetComponent<Animator>().SetTrigger("Open_WQuestion");
            GetGameObject(16).GetComponent<Text>().text = mDictionary.GetString("another");
            mTTS.Say(mDictionary.GetString("another"));
            GetGameObject(17).GetComponent<Button>().onClick.AddListener(AnswerYes);
        }

        private void AnswerYes()
        {
            GetComponent<Animator>().SetTrigger("BackToStart");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!check && mTTS.HasFinishedTalking && !mVocalActivation.RecognitionTriggered)
            {
                check = true;
                mVocalActivation.VocalProcessing = VocalProcessing;
                mVocalActivation.VocalError = VocalError;
                mVocalActivation.StartInstantReco();
            }
        }

        private void VocalProcessing(string answer)
        {
            answer = answer.ToLower();
            if (answer.Contains(mDictionary.GetString("yes")))
                AnswerYes();
            else if (answer.Contains(mDictionary.GetString("no")))
                GetComponent<RecipeBehaviour>().Exit();
            else
                mVocalActivation.StartInstantReco();
        }

        private void VocalError(STTError error)
        {
            mVocalActivation.StartInstantReco();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(15).GetComponent<Animator>().SetTrigger("Close_WQuestion");
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            mVocalActivation.VocalProcessing = null;
            mVocalActivation.VocalError = null;
            mVocalActivation.StopListenBehaviour();
            mTTS.Silence(0);
        }
    }
}