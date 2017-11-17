using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class TakePhotoBehaviour : MonoBehaviour {

        [SerializeField]
        private RawImage mRawVideoTexture;
        [SerializeField]
        private Animator mPlayMathAnimator;

        private bool mFlush;

        public static event System.Action<Photograph> OnEndTakePhoto;

        void Start(){
            OnEndTakePhoto = delegate (Photograph lPhoto)
            {
                    mFlush = false;
                    mRawVideoTexture = lPhoto.Image.texture;
                    BYOS.Instance.Primitive.RGBCam.Close();

                    Invoke("MoveToCertificate",5.0f);
            };
        }

        public void DisplayCamera()
        {
            BYOS.Instance.Primitive.RGBCam.Open();
            mFlush = true;
            StartCoroutine("FlushCameraTexture");
            Invoke("TakePhoto", 2.0f);
        }

        private void FlushCameraTexture()
        {
            while (mFlush)
                mRawVideoTexture = BYOS.Instance.Primitive.RGBCam.FrameTexture2D;
        }

        private void TakePhoto()
        {
            BYOS.Instance.Primitive.RGBCam.TakePhotograph(OnEndTakePhoto);
        }

        private void MoveToCertificate()
        {
            mPlayMathAnimator.SetTrigger("Certificate");
        }
    }
}

