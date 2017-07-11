using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.Jukebox
{
    public class NextMusicState : AStateMachineBehaviour
    {
        private int mIndex;
        private int mPlaylistLength;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mIndex = iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic;
            mPlaylistLength = iAnimator.GetBehaviour<LoadingMusicState>().PathLength;
            if (mIndex + 1 > mPlaylistLength - 1)
            {
                iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic = 0;
            }
            else
            {
                iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic += 1;
            }

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetTrigger("IndexDone");
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}

