using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Allows to detect some interactions with Buddy's eyes
    /// </summary>
    public class BuddyFaceDetector : MonoBehaviour
    {
        [SerializeField]
        private Button leftEye;

        [SerializeField]
        private Button rightEye;

        [SerializeField]
        private Slider caressBar;

        /// <summary>  
        ///  This is to detect when Buddy's face is touched once
        /// </summary> 
        public bool EyeTouched { get { return mFaceTouched; } }
        /// <summary>  
        ///  This is to detect when Buddy's face is touched a lot
        /// </summary> 
        public bool FaceSmashed { get { return mFaceSmashed; } }
        
        private bool mFaceTouched;
        private bool mFaceSmashed;
        private bool mCarressed;
        private short mTimesFaceTouched;
        private float mLastTimeFaceTouched;
        private Face mFace;

        void Start()
        {
            mFace = BYOS.Instance.Face;
            mFaceTouched = false;
            mTimesFaceTouched = 0;
            mLastTimeFaceTouched = Time.time;

            leftEye.onClick.AddListener(LeftEyeClicked);
            rightEye.onClick.AddListener(RightEyeClicked);
        }

        void Update()
        {
            //if (EyeTouched)
            //    FacePoked();

            if (Time.time - mLastTimeFaceTouched >= 0.3F)
                mFaceTouched = false;

            if (Time.time - mLastTimeFaceTouched >= 3F) {
                mTimesFaceTouched = 0;
                mFaceSmashed = false;
            }

            //if (mFaceSmashed)
            //    Debug.Log("Face smashed !!");

            //if (Input.GetMouseButtonDown(0))
            //{
            //    Vector2 pos = Input.mousePosition;
            //    Debug.Log("Got a click at point X : " + pos.x + " / Y : " + pos.y);

            //    if (pos.x > Screen.width * 0.8 && LeftSideTouched != null)
            //        LeftSideTouched();
            //    else if (pos.x < Screen.width * 0.2 && RightSideTouched != null)
            //        RightSideTouched();
            //}
        }

        //Called by button on Buddy's left eye
        private void LeftEyeClicked()
        {
            mFace.SetEvent(FaceEvent.BLINK_LEFT);
            FacePoked();
        }

        //Called by button on Buddy's right eye
        private void RightEyeClicked()
        {
            mFace.SetEvent(FaceEvent.BLINK_RIGHT);
            FacePoked();
        }

        //Counts the number of time Buddy's eyes were poked
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