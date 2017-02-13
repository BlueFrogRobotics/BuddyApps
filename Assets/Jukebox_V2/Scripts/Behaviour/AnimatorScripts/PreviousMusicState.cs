using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.Jukebox
{
    public class PreviousMusicState : AStateMachineBehaviour
    {
        private int mIndex;
        private int mPlaylistLength;
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mIndex = iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic;
            mPlaylistLength = iAnimator.GetBehaviour<LoadingMusicState>().PathLength;

            if (mIndex - 1 < 0)
            {
                iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic = mPlaylistLength - 1;
            }  
            else
            {
                iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic -= 1;
            }
                

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetTrigger("IndexDone");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}

