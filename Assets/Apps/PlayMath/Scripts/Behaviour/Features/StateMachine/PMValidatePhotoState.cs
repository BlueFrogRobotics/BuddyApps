using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

namespace BuddyApp.PlayMath{
    public class PMValidatePhotoState : AStateMachineBehaviour {

        private Animator mBackgroundAnimator;
        private Animator mPlayMathAnimator;

        private Certificate mCertificate;
        private Texture mVideoTexture;

        public static event System.Action OnValidatePhoto;
        public static event System.Action OnCancelPhoto;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

            OnValidatePhoto = delegate {
                mCertificate.UserPic = (Texture2D) mVideoTexture;
                mPlayMathAnimator.SetTrigger("Certificate");
            };

            OnCancelPhoto = delegate {
                mPlayMathAnimator.SetTrigger("TakePhoto");
            };

            mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
            mBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
            mBackgroundAnimator.SetTrigger("close");

            mCertificate = GameObject.Find("UI/EndGame_Certificate").GetComponent<Certificate>();
            mVideoTexture = GameObject.Find("UI/Take_Photo/Raw_Video").GetComponent<RawImage>().texture;

            BYOS.Instance.Interaction.TextToSpeech.SayKey("validationphoto");

            Sprite toastSprite = Sprite.Create((Texture2D)mVideoTexture, 
                                                new Rect(0, 0, mVideoTexture.width, mVideoTexture.height),
                                                new Vector2(0.5f, 0.5f));
            
            BYOS.Instance.Toaster.Display<PictureToast>().With(
                BYOS.Instance.Dictionary.GetString("validationphoto").ToUpper(),
                toastSprite,
                OnValidatePhoto,
                OnCancelPhoto);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            mBackgroundAnimator.SetTrigger("open");
        }
    }
}