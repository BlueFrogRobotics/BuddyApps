using UnityEngine;

using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage mLocalVideo;

        [SerializeField]
        private GameObject mVideoOn;

        [SerializeField]
        private GameObject mVideoOff;

        [SerializeField]
        private GameObject mWebRtc = null;

        [SerializeField]
        private bool mPreSelection;

        private bool mActive = true;

        // Call when the user click to take the call
        // This function update the visual of the Video button (In the call view)
        public void CameraUpdateToggle()
        {
            mActive = !mActive;
            mVideoOn.SetActive(mActive);
            mVideoOff.SetActive(!mActive);
        }

        public void onToggleCamera()
        {
            if (mWebRtc.activeSelf || mPreSelection) {
                mActive = !mActive;
                mVideoOn.SetActive(mActive);
                mVideoOff.SetActive(!mActive);
                mLocalVideo.gameObject.SetActive(mActive);
            }
        }
    }
}
