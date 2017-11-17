using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

namespace BuddyApp.PlayMath{
    public class TakePhotoBehaviour : MonoBehaviour {

        [SerializeField]
        private RawImage mRawVideoTexture;
        [SerializeField]
        private Animator mPlayMathAnimator;

        private bool mFlush;

        public static event System.Action<Photograph> OnEndTakePhoto;
        public static event System.Action OnValidatePhoto;
        public static event System.Action OnCancelPhoto;

        void Start(){
            OnEndTakePhoto = delegate (Photograph lPhoto)
            {
                    mFlush = false;
                    mRawVideoTexture.texture = lPhoto.Image.texture;
                    BYOS.Instance.Primitive.RGBCam.Close();

                    BYOS.Instance.Notifier.Display<ConfirmationNot>().With(
                        "DO YOU LIKE THIS PHOTO ?",
                        OnValidatePhoto,
                        OnCancelPhoto);
            };

            OnValidatePhoto = delegate
            {
                    mPlayMathAnimator.SetTrigger("Certificate");
            };

            OnCancelPhoto = delegate
            {
                    DisplayCamera();
            };
        }

        public void DisplayCamera()
        {
            BYOS.Instance.Primitive.RGBCam.Open();
            mFlush = true;
            StartCoroutine("FlushCameraTexture");
            Invoke("TakePhoto", 2.0f);
        }

        private IEnumerator FlushCameraTexture()
        {
            while (mFlush)
            {
                mRawVideoTexture.texture = BYOS.Instance.Primitive.RGBCam.FrameTexture2D;
                yield return null;
            }
            
        }

        private void TakePhoto()
        {
            BYOS.Instance.Primitive.RGBCam.TakePhotograph(OnEndTakePhoto);
        }
            
    }
}

