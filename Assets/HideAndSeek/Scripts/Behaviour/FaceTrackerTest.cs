using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyFeature.Vision;
using OpenCVUnity;
using BuddyTools;

namespace BuddyApp.HideAndSeek
{
    public class FaceTrackerTest : MonoBehaviour
    {

        public RawImage mRaw;
        public FaceCascadeTracker mFaceTracker;
        public FaceRecognizer mFaceReco;
        private RGBCam mCam;
        private Mat mMatFaces;
        private List<Mat> mListMat;
        private List<int> mListLabel;
        private List<OpenCVUnity.Rect> mFaces;
        private int mLabelAct = 0;
        private bool mIsLabelling = false;
        private bool mIsTrained = false;
        private LBPHfaces mLBPHFaces;

        // Use this for initialization
        void Start()
        {
            mLBPHFaces = new LBPHfaces(1, 8, 8, 8, 123);
            mListMat = new List<Mat>();
            mListLabel = new List<int>();
            mCam = BYOS.Instance.RGBCam;
            mMatFaces = new Mat();
            if (!mCam.IsOpen)
                mCam.Open();
        }

        // Update is called once per frame
        void Update()
        {
            mFaces = mFaceTracker.TrackedObjects;
            ShowFaceTracked();
            if (mIsLabelling)
                AddInput();
            if (mIsTrained)
                Predict();

        }

        void Predict()
        {
            Mat lSub = new Mat();
            for (int i = 0; i < mFaces.Count; i++)
            {
                lSub = mCam.FrameMat.submat(mFaces[i]);
            }
            if (mFaces.Count > 0)
            {
                Mat dst = new Mat();
                Imgproc.cvtColor(lSub, dst, Imgproc.COLOR_RGB2GRAY);
                Imgproc.resize(dst, dst, new Size(100, 100));
                
                Imgproc.equalizeHist(dst, dst);
                MinDistancePredictCollector lPredictor = new MinDistancePredictCollector(200);
                mLBPHFaces.predict(dst, lPredictor, 0);
                int label = lPredictor.getLabel();
                //int label = mFaceReco.Predict(dst);
                Debug.Log("label: " + label);
            }
        }

        void ShowFaceTracked()
        {

            Mat lSub = new Mat();
            for (int i = 0; i < mFaces.Count; i++)
            {
                lSub = mCam.FrameMat.submat(mFaces[i]);
            }
            if (mFaces.Count > 0)
            {
                Mat dst = new Mat();
                Imgproc.resize(lSub, dst, new Size(100, 100));
                Texture2D texture = Utils.MatToTexture2D(dst);
                mRaw.texture = texture;
            }
        }

        void AddInput()
        {
            Mat lSub = new Mat();
            for (int i = 0; i < mFaces.Count; i++)
            {
                lSub = mCam.FrameMat.submat(mFaces[i]);
            }
            if (mFaces.Count > 0)
            {
                Mat dst = new Mat();
                Imgproc.cvtColor(lSub, dst, Imgproc.COLOR_RGB2GRAY);
                Imgproc.resize(dst, dst, new Size(100, 100));
                Imgproc.equalizeHist(dst, dst);

                mListMat.Add(dst);
                mListLabel.Add(mLabelAct);
                Debug.Log("label added: " + mLabelAct);
            }
        }

        public void StartLabel()
        {
            mIsLabelling = true;
            Debug.Log("labelisation start");
        }

        public void StopLabel()
        {
            mIsLabelling = false;
            mLabelAct++;
            Debug.Log("labelisation stopped");
        }

        public void Train()
        {
            //mFaceReco.Train(mListMat, mListLabel);
            Debug.Log(Utils.CollectionToString(mListLabel));
            mLBPHFaces.train(mListMat, mListLabel);
            mIsTrained = true;
        }
    }
}