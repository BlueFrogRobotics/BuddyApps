using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.Jukebox
{
    public class PauseState : AStateMachineBehaviour
    {

        private AudioSource mMusicPlay;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(2).SetActive(true);
            GetGameObject(3).SetActive(false);
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            mMusicPlay.Pause();
            iAnimator.GetBehaviour<PlayState>().FromPauseState = true;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}

