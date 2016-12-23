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

        public override void Init()
        {
            mSaveNameWindow = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponentInChildren<SaveNameWindow>();
            mPlayers = GetComponent<Players>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mSaveNameWindow.Open();
            mSaveNameWindow.ButtonYes.onClick.AddListener(ValidateName);
            mHasValidated = false;
            mHasClosed = false;
            mSaveNameWindow.ImageToDisplay.texture = Utils.MatToTexture2D(mPlayers.GetPlayer(mPlayers.NumPlayer - 1).FaceMat);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(mHasValidated && !mHasClosed)
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
        }

        private void ValidateName()
        {
            mHasValidated = true;
            mPlayers.GetPlayer(mPlayers.NumPlayer-1).Name = mSaveNameWindow.InputName.text;
        }
    }
}