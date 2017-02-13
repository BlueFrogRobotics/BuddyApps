using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class GlobalJukeboxAI : MonoBehaviour
    {
        [SerializeField]
        private AudioSource mSource;

        [SerializeField]
        private Animator mAnimator;

        [SerializeField]
        private Text mText;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StopMusic()
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayState"))
            {
                mAnimator.SetTrigger("Stop");
            }
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PauseState"))
            {
                mAnimator.SetTrigger("PauseToStop");
            }

        }

        public void PlayMusic()
        {
            if(mAnimator.GetCurrentAnimatorStateInfo(0).IsName("StopState"))
            {
                mAnimator.SetTrigger("Replay");
            }
        }

        public void NextMusic()
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayState"))
            {
                mAnimator.SetTrigger("Next");
            }
        }

        public void PreviousMusic()
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayState"))
            {
                mAnimator.SetTrigger("Previous");
            }
        }

        public void PauseMusic()
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayState"))
            {
                mAnimator.SetTrigger("Pause");
            }
            if(mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PauseState"))
            {
                mAnimator.SetTrigger("UnPause");
            }
        }

        public void Playlist()
        {
            if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayState"))
            {
                mAnimator.SetTrigger("Playlist");
            }
        }
    }
}

