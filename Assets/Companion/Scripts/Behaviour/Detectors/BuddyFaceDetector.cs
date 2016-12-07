using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class BuddyFaceDetector : MonoBehaviour
    {
        /// <summary>  
        ///  This is to detect when Buddy's face is touched once
        /// </summary> 
        public bool FaceTouched { get { return mFaceTouched; } }
        /// <summary>  
        ///  This is to detect when Buddy's face is touched a lot
        /// </summary> 
        public bool FaceSmashed { get { return mFaceSmashed; } }

        [SerializeField]
        private Button mLeftEye;

        [SerializeField]
        private Button mRightEye;

        private bool mFaceTouched;
        private bool mFaceSmashed;
        private short mTimesFaceTouched;
        private float mLastTimeFaceTouched;
        private Face mFace;
        
        void Start()
        {
            mFace = BYOS.Instance.Face;
            mFaceTouched = false;
            mTimesFaceTouched = 0;
            mLastTimeFaceTouched = Time.time;

            mLeftEye.onClick.AddListener(LeftEyePoked);
            mRightEye.onClick.AddListener(RightEyePoked);
        }
        
        void Update()
        {
            if (Time.time - mLastTimeFaceTouched >= 0.3F)
                mFaceTouched = false;

            if (Time.time - mLastTimeFaceTouched >= 3F) {
                mTimesFaceTouched = 0;
                mFaceSmashed = false;
            }

            if (mFaceSmashed)
                Debug.Log("Face smashed !!");
        }

        private void LeftEyePoked()
        {
            mFace.SetEyeEvent(EyeEvent.BLINK_LEFT);
            FacePoked();
        }

        private void RightEyePoked()
        {
            mFace.SetEyeEvent(EyeEvent.BLINK_RIGHT);
            FacePoked();
        }

        private void FacePoked()
        {
            Debug.Log("Face poked");
            mTimesFaceTouched++;
            mFaceTouched = true;

            if (mTimesFaceTouched >= 3)
                mFaceSmashed = true;

            mLastTimeFaceTouched = Time.time;
        }
    }
}