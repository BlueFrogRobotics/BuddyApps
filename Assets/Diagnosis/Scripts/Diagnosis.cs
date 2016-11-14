using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OpenCVUnity;
using BuddyOS;

public class Diagnosis : MonoBehaviour
{
    [SerializeField]
    RawImage mRGBCamRawImage;

    private RGBCam mRGBCam;
    private Mat mRGBMat;
	void Start ()
    {
        mRGBCam = BYOS.Instance.RGBCam;
        mRGBCam.Open();
        mRGBMat = new Mat();
    }
	
	void Update ()
    {
        mRGBMat = mRGBCam.FrameMat;
        if (mRGBMat != null)
        {
            mRGBCamRawImage.texture = mRGBCam.FrameTexture2D;
        }
    }
}
