using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class StopState : AStateMachineBehaviour
    {
        private AudioSource mMusicPlay;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ON ENTER STOP STATE");
            GetGameObject(2).SetActive(true);
            GetGameObject(3).SetActive(false);
            
            mMusicPlay = GetGameObject(0).GetComponent<AudioSource>();
            iAnimator.GetBehaviour<LoadingMusicState>().IndexMusic = 0;
            mMusicPlay.Stop();
            GetGameObject(1).GetComponent<Text>().text = " ";
            iAnimator.GetBehaviour<PlayState>().FromPauseState = false;

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

    }

}
