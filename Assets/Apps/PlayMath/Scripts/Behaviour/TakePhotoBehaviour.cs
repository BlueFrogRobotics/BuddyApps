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
        [SerializeField]
        private Certificate mCertificate;

        private bool mFlush;

        public static event System.Action<Photograph> OnEndTakePhoto;
        public static event System.Action OnValidatePhoto;
        public static event System.Action OnCancelPhoto;

        private const float DURATION = 10f;

        void Start(){
            OnEndTakePhoto = delegate (Photograph lPhoto)
            {
                    mFlush = false;
                    BYOS.Instance.Primitive.RGBCam.Close();

                    mRawVideoTexture.texture = lPhoto.Image.texture;

                    BYOS.Instance.Notifier.Display<ConfirmationNot>(DURATION).With(
                        "DO YOU LIKE THIS PHOTO ?",
                        OnValidatePhoto,
                        OnCancelPhoto);
            };

            OnValidatePhoto = delegate
            {
                    mCertificate.UserPic = (Texture2D) mRawVideoTexture.texture;
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
            BYOS.Instance.Notifier.Display<CountdownNot>().With("", 3, null, null);
            StartCoroutine(FlushCameraTexture());
            Invoke("TakePhoto", 4.0f);
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

