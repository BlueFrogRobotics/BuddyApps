using UnityEngine;
using UnityEngine.UI;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class AskAnotherRecipe : AStateMachineBehaviour
    {
        private bool mCheck;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER ASK ANOTHER RECIPE");
            mCheck = false;
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
            GetGameObject(1).SetActive(true);
            GetGameObject(2).SetActive(false);
            GetGameObject(15).GetComponent<Animator>().SetTrigger("Open_WQuestion");
            GetGameObject(16).GetComponent<Text>().text = mDictionary.GetString("another");
            GetGameObject(18).GetComponent<Text>().text = mDictionary.GetString("yes");
            GetGameObject(19).GetComponent<Text>().text = mDictionary.GetString("no");
            mTTS.Say(mDictionary.GetString("another"));
            GetGameObject(17).GetComponent<Button>().onClick.AddListener(AnswerYes);
        }

        private void AnswerYes()
        {
            GetComponent<Animator>().SetTrigger("BackToStart");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && mTTS.HasFinishedTalking && !mVocalManager.RecognitionTriggered) {
                mCheck = true;
                mVocalManager.OnEndReco = VocalProcessing;
                mVocalManager.OnError = VocalError;
                mVocalManager.StartInstantReco();
            }
        }

        private void VocalProcessing(string iAnswer)
        {
            iAnswer = iAnswer.ToLower();
            if (iAnswer.Contains(mDictionary.GetString("yes")))
                AnswerYes();
            else if (iAnswer.Contains(mDictionary.GetString("no")))
                GetComponent<RecipeBehaviour>().Exit();
            else
                mVocalManager.StartInstantReco();
        }

        private void VocalError(STTError error)
        {
            mVocalManager.StartInstantReco();
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(15).GetComponent<Animator>().SetTrigger("Close_WQuestion");
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            mVocalManager.OnEndReco = null;
            mVocalManager.OnError = null;
            mVocalManager.StopListenBehaviour();
            mTTS.Silence(0);
            Debug.Log("EXIT ASK ANOTHER RECIPE");
        }
    }
}