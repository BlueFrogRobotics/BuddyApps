using UnityEngine;
using UnityEngine.UI;
using BuddyAPI;
using OpenCVUnity;

public class DepthCamScript : MonoBehaviour
{

    [SerializeField]
    private DepthCam mDepthCam;
    //[SerializeField]
    //private RawImage mRawDepthImage;

    private Mat mFrameDepth;

    public void OpenDephCam()
    {
        if (!mDepthCam.IsOpen)
        {
            mDepthCam.Open(DepthMode.MODE_9_10FPS_1000);
        }
    }

    // Use this for initialization
    void Start()
    {
        //mFrameDepth = new Mat();
    }

    // Update is called once per frame
    void Update()
    {
        OpenDephCam();
        //if (mDepthCam.Width == 0)
        //   return;

        //  mRawDepthImage.texture = mDepthCam.FrameTexture2D;
        //}
        // mPointCloud = mDepthCam.GetPointCloud().points;
    }
}
