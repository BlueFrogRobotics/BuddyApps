using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.Jukebox
{
    public class PauseState : AStateMachineBehaviour
    {

        private AudioSource mMusicPlay;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(2).SetActive(true);
            GetGameObject(3).SetActive(false);
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            mMusicPlay.Pause();
            iAnimator.GetBehaviour<PlayState>().FromPauseState = true;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}

