using UnityEngine;
using System.Collections;
using BuddyOS.App;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class StopState : AStateMachineBehaviour
    {
        private AudioSource mMusicPlay;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

    }

}
