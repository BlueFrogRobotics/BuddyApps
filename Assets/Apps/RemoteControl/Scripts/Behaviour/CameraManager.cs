using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage localVideo;

        [SerializeField]
        private GameObject videoOn;

        [SerializeField]
        private GameObject videoOff;

        [SerializeField]
        private GameObject webRTC = null;

        [SerializeField]
        private bool mPreSelection;

        private bool mActive = true;

        // Call when the user click to take the call
        // This function update the visual of the Video button (In the call view)
        public void CameraUpdateToggle()
        {
            mActive = !mActive;
            videoOn.SetActive(mActive);
            videoOff.SetActive(!mActive);
        }

        public void onToggleCamera()
        {
            if (webRTC.activeSelf || mPreSelection)
            {
                mActive = !mActive;
                videoOn.SetActive(mActive);
                videoOff.SetActive(!mActive);
                localVideo.gameObject.SetActive(mActive);
            }
        }
    }
}
