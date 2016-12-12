﻿using UnityEngine;
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
        public InputField mInputPlayerSaved;
        public Button mButtonTrain;
        public bool IsTrained { get { return mIsTrained; } }
        private RGBCam mCam;
        private Mat mMatFaces;
        private List<Mat> mListMat;
        private List<int> mListLabel;
        private List<OpenCVUnity.Rect> mFaces;
        private int mLabelAct = 0;
        public int NbLabel { get { return mLabelAct; } }
        private bool mIsLabelling = false;
        private bool mIsTrained = false;
        private LBPHfaces mLBPHFaces;
        private Texture2D mTexture;

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
            mInputPlayerSaved.text = "nombre de joueurs: " + mLabelAct;
            mFaces = mFaceTracker.TrackedObjects;
            ShowFaceTracked();
            if (mIsLabelling)
                AddInput();
            //if (mIsTrained)
             //   Predict();

        }

        public int Predict(out double oDist)
        {
            int lLabel = -1;
            oDist = 0;
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
                lLabel = lPredictor.getLabel();
                oDist=lPredictor.getDist();
                //int label = mFaceReco.Predict(dst);
                //Debug.Log("label: " + label);
            }
            return lLabel;
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
                mTexture = Utils.MatToTexture2D(dst);
                mRaw.texture = mTexture;
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