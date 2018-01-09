using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

namespace BuddyApp.PlayMath{
    public class TakePhotoBehaviour : AnimationSyncBehaviour {

        [SerializeField]
        private RawImage mRawVideoTexture;
        [SerializeField]
        private Animator mPlayMathAnimator;
        [SerializeField]
        private Certificate mCertificate;

        private bool mFlush;

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
            yield return new WaitUntil(() => BYOS.Instance.Interaction.TextToSpeech.HasFinishedTalking);
            BYOS.Instance.Notifier.Display<CountdownNot>().With("", 3, TakePhoto, null);
        }

        private void TakePhoto()
        {
            mFlush = false;
            BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            BYOS.Instance.Primitive.RGBCam.Close();
            mPlayMathAnimator.SetTrigger("ValidatePhoto");
        }
    }
}