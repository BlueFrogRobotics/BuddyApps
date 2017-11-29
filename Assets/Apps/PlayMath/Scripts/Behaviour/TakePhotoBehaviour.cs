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

        void Start()
        {
            OnEndTakePhoto = delegate (Photograph lPhoto)
            {
                    mFlush = false;
                    BYOS.Instance.Primitive.RGBCam.Close();
                    mRawVideoTexture.texture = (Texture) lPhoto.Image.texture;
                    mPlayMathAnimator.SetTrigger("ValidatePhoto");
            };
        }

        public void DisplayCamera()
        {
            BYOS.Instance.Interaction.TextToSpeech.SayKey("takephotolabel", true);
                
            BYOS.Instance.Primitive.RGBCam.Open();
            mFlush = true;
            StartCoroutine(FlushCameraTexture());
            StartCoroutine(InitTakePhoto());
        }

        private IEnumerator FlushCameraTexture()
        {
            while (mFlush)
            {
                mRawVideoTexture.texture = BYOS.Instance.Primitive.RGBCam.FrameTexture2D;
                yield return null;
            }
        }

        private IEnumerator InitTakePhoto()
        {
            while (true)
            {
                if (!BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking)
                    yield return null;
                else
                {
                    BYOS.Instance.Notifier.Display<CountdownNot>().With("", 3, TakePhoto, null);
                    break;
                }
            }
        }

        private void TakePhoto()
        {
            BYOS.Instance.Primitive.RGBCam.TakePhotograph(OnEndTakePhoto);
        }
    }
}

