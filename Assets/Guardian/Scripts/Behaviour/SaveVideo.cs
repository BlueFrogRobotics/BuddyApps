using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCVUnity;
using Buddy;
using System.IO;

namespace BuddyApp.Guardian
{
    public class SaveVideo : MonoBehaviour
    {
        private float mFPS = 15;
        private float mNumSec = 10;
        private float mTime = 0.0f;
        private Queue<Mat> mBufferVideo;
        private float mMaxBufferSize;
        private RGBCam mCam;

        // Use this for initialization
        void Start()
        {

            mBufferVideo = new Queue<Mat>();
            mMaxBufferSize = mFPS * mNumSec;
            mCam = BYOS.Instance.Primitive.RGBCam;
            if (!mCam.IsOpen)
                mCam.Open(RGBCamResolution.W_176_H_144);
        }

        // Update is called once per frame
        void Update()
        {

            mTime += Time.deltaTime;
            if (mTime > 1.0f / mFPS) {

                if (mCam.FrameMat == null)
                    return;

                Mat lMatCam = mCam.FrameMat.clone();
                //Debug.Log("frame id: " + mCam.FrameID);
                mTime = 0.0f;
                mBufferVideo.Enqueue(lMatCam);
                if (mBufferVideo.Count > mMaxBufferSize)
                    mBufferVideo.Dequeue();
            }
        }

        public void Save(string iFilename)
        {
			// TODO path to raw not for writing
			//Utils.Save(BYOS.Instance.Resources.PathToRaw("monitoring.avi"), mBufferVideo.ToArray(), mFPS);
			string lDirectoryPath = Path.GetDirectoryName(BYOS.Instance.Resources.PathToRaw("monitoring.avi"));
			Directory.CreateDirectory(lDirectoryPath);
			Utils.Save(BYOS.Instance.Resources.PathToRaw("monitoring.avi"), mBufferVideo.ToArray(), mFPS);
			//Save("monitoring.avi", mFPS);
		}

		public void Save(string iFilename, float iFps)
        {
            Mat[] iListMat = mBufferVideo.ToArray();
            //Mat mMatRed = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3, new Scalar(254, 0, 0));
            if (iListMat.Length > 0) {
                int lCodec = VideoWriter.fourcc('M', 'J', 'P', 'G');
                string lFilepath = BYOS.Instance.Resources.PathToRaw(iFilename);//Utils.GetStreamingAssetFilePath(filename);
                VideoWriter lVideoWriter = new VideoWriter(lFilepath, lCodec, iFps, iListMat[0].size());

                if (!lVideoWriter.isOpened()) {
                    lVideoWriter.open(lFilepath, lCodec, iFps, iListMat[0].size());
                }
                if (lVideoWriter.isOpened()) {
                    for (int i = 0; i < iListMat.Length; i++) {
                        Mat lFrame = new Mat();
                        //if (i % 2 == 0)
                        //{
                        Imgproc.cvtColor(iListMat[i], lFrame, Imgproc.COLOR_RGB2BGR);
                        Imgproc.putText(lFrame, "recorded by Buddy", new Point(lFrame.width() - 100, lFrame.height() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.28, new Scalar(0, 212, 209, 255));
                        // }
                        // else
                        //    lFrame = mMatRed;

                        lVideoWriter.write(lFrame);
                    }
                }
                lVideoWriter.Dispose();
                lVideoWriter = null;
            }
        }
    }
}