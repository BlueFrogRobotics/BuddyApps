using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.RemoteControl
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mMicroOn;

        [SerializeField]
        private GameObject mMicroOff;

        [SerializeField]
        private GameObject mWebRtc = null;

        [SerializeField]
        private bool mPreSelection;

        private bool mActive = true;

        public void SoundUpdateToggle()
        {
            mActive = !mActive;
            mMicroOn.SetActive(mActive);
            mMicroOff.SetActive(!mActive);
        }

        public void onToggleSound()
        {
            if (mWebRtc.activeSelf || mPreSelection) {
                mActive = !mActive;
                mMicroOn.SetActive(mActive);
                mMicroOff.SetActive(!mActive);

                try {
                    using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc")) {
                        cls.CallStatic("setSoundActive", mActive);
                    }
                } catch (System.Exception ex) {
                    Debug.LogWarning("------ EXCEPTION onToggleSound: " + ex.Message + " ------");
                }
            }
        }
    }
}