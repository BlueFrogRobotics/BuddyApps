using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;
using BuddyFeature.Vision;
using OpenCVUnity;

public class FaceTrackerTest : MonoBehaviour {

    public RawImage mRaw;
    public FaceCascadeTracker mFaceTracker;
    private RGBCam mCam;
    private Mat mMatFaces;

	// Use this for initialization
	void Start () {
        mCam = BYOS.Instance.RGBCam;
        mMatFaces = new Mat();
        if (!mCam.IsOpen)
            mCam.Open();
	}
	
	// Update is called once per frame
	void Update () {
        List<OpenCVUnity.Rect> lFaces = mFaceTracker.TrackedObjects;
        //for(int i=0; i<lFaces.Count; i++)
        //{

        //}
        mRaw.texture = mFaceTracker.FrameTexture2D;
	}
}
