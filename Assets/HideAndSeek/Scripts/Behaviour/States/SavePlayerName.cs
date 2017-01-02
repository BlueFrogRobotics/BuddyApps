using UnityEngine;
using BuddyOS.App;
using BuddyTools;
using System.Collections;

namespace BuddyApp.HideAndSeek
{
    public class SavePlayerName : AStateMachineBehaviour
    {
        private SaveNameWindow mSaveNameWindow;
        private Players mPlayers;
        private bool mHasValidated = false;
        private bool mHasClosed = false;
        private bool mIsListening = false;

        public override void Init()
        {
            mSaveNameWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<SaveNameWindow>();
            mPlayers = GetComponent<Players>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>().SetAppWhite();
            mSaveNameWindow.Open();
            mSaveNameWindow.ButtonYes.onClick.AddListener(ValidateName);
            mSaveNameWindow.ButtonNo.onClick.AddListener(ResetName);
            mSaveNameWindow.InputName.text = "";
            mHasValidated = false;
            mHasClosed = false;
            mIsListening = false;
            mSaveNameWindow.ImageToDisplay.texture = Utils.MatToTexture2D(mPlayers.GetPlayer(mPlayers.NumPlayer - 1).FaceMat);
            mTTS.Say(mDictionary.GetString("askPlayerName"));
            mSTT.OnBestRecognition.Add(VocalProcessing);
            mSTT.OnBeginning.Add(StartListening);
            mSTT.OnEnd.Add(StopListening);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Debug.Log("input text: " + mSaveNameWindow.InputName.text);
            if (!mHasValidated && mTTS.HasFinishedTalking && mSaveNameWindow.InputName.text=="")
            {
                //Debug.Log("dans if");
                if (!mIsListening )
                {
                    //mVocalActivation.StartInstantReco();
                    mSTT.Request();
                }
            }
            else if (mHasValidated && !mHasClosed)
            {
                mSaveNameWindow.Close();
                mHasClosed = true;
            }
            else if(mHasClosed && mSaveNameWindow.IsOff())
                iAnimator.SetTrigger("ChangeState");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
            mSaveNameWindow.ButtonYes.onClick.RemoveAllListeners();
            mSaveNameWindow.ButtonNo.onClick.RemoveAllListeners();
            mSTT.OnBestRecognition.Remove(VocalProcessing);
            mSTT.OnBeginning.Remove(StartListening);
            mSTT.OnEnd.Remove(StopListening);
        }

        private void ValidateName()
        {
            mHasValidated = true;
            mPlayers.GetPlayer(mPlayers.NumPlayer-1).Name = mSaveNameWindow.InputName.text;
        }

        private void ResetName()
        {
            mSaveNameWindow.InputName.text = "";
        }

        private void VocalProcessing(string iRequest)
        {
            if(iRequest!="" && mSaveNameWindow.InputName.text=="")
            {
                string lName = iRequest.ToLower();
                lName = lName.Replace("my name is", "").Replace("je suis", "").Replace("i am", "").Replace("i'm", "").Replace("name", "");
                mSaveNameWindow.InputName.text = lName;
            }
        }

        private void StartListening()
        {
            mIsListening = true;
        }

        private void StopListening()
        {
            mIsListening = false;
        }
    }
}