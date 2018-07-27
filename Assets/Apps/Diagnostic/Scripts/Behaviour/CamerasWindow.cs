using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

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

        private RGBCamera mRGBCam;
        private DepthCamera mDepthCam;

        void Update()
        {
            if (mRGBCam.IsOpen)
                rgbImage.texture = mRGBCam.TexFrame;

            if (mDepthCam.IsOpen) {
                depthColorImage.texture = mDepthCam.TexFrame;
                depthGrayImage.texture = mDepthCam.TexFrame;
            }
        }

        void OnEnable()
        {
            mRGBCam = Buddy.Sensors.RGBCamera;
            mDepthCam = Buddy.Sensors.DepthCamera;

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
