using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class CamerasWindow : MonoBehaviour
    {
        [SerializeField]
        private RawImage rgbImage;

        [SerializeField]
        private RawImage depthGrayImage;

        [SerializeField]
        private RawImage depthColorImage;

        private RGBCam mRGBCam;
        private DepthCam mDepthCam;

        void Update()
        {
            if (mRGBCam.IsOpen)
                rgbImage.texture = mRGBCam.FrameTexture2D;

            if (mDepthCam.IsOpen) {
                depthColorImage.texture = mDepthCam.FrameTexture2D;
                depthGrayImage.texture = mDepthCam.DepthTexture2D;
            }
        }

        void OnEnable()
        {
            mRGBCam = BYOS.Instance.RGBCam;
            mDepthCam = BYOS.Instance.DepthCam;

            mRGBCam.Open();
            mDepthCam.Open();
        }

        void OnDisable()
        {
            mRGBCam.Close();
            mDepthCam.Close();
        }
    }
}
