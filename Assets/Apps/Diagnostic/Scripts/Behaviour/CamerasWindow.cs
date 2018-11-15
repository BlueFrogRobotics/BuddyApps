using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.Diagnostic
{
    public sealed class CamerasWindow : MonoBehaviour
    {
        [SerializeField]
        private RawImage selectedImage;

        [SerializeField]
        private Dropdown selectedCamera;

        private RGBCamera mRGBCam;
        private HDCamera mHDCam;

        private enum E_CAMERA { HD, RGB }; // Use enum instead of strings to maintain performances.
        private E_CAMERA mECamera;

        void Update()
        {
            switch(mECamera)
            {
                case E_CAMERA.HD:
                    if (mHDCam.IsOpen) {
                        selectedImage.texture = Utils.MatToTexture2D(mHDCam.Frame);
                    }
                    break;

                case E_CAMERA.RGB:
                    if (mRGBCam.IsOpen) {
                        selectedImage.texture = Utils.MatToTexture2D(mRGBCam.Frame);
                    }
                    break;
            }
        }

        void OnEnable()
        {
            mRGBCam = Buddy.Sensors.RGBCamera;
            mHDCam = Buddy.Sensors.HDCamera;
            
            // Initialization
            DropdownValueChanged(selectedCamera);

            // Set callback when changing camera
            selectedCamera.onValueChanged.AddListener(delegate {
                DropdownValueChanged(selectedCamera);
            });
        }

        void OnDisable()
        {
            selectedCamera.onValueChanged.RemoveAllListeners();
            
            if (mRGBCam.IsBusy || mRGBCam.IsOpen)
				mRGBCam.Close();
            if (mHDCam.IsBusy || mHDCam.IsOpen)
				mHDCam.Close();
        }

        void DropdownValueChanged(Dropdown change)
        {
            switch (change.options[change.value].text) {
                case "HD":
                    mECamera = E_CAMERA.HD;
					
                    if (mRGBCam.IsBusy || mRGBCam.IsOpen)
						mRGBCam.Close();
                    
                    if (mHDCam.IsBusy)
                        mHDCam.Close();

                    if (!mHDCam.IsOpen)
                        mHDCam.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);

                    break;
                    
                case "RGB":
                    mECamera = E_CAMERA.RGB;

                    if (mRGBCam.IsBusy)
                        mRGBCam.Close();

                    if (!mRGBCam.IsOpen)
                        mRGBCam.Open(RGBCameraMode.COLOR_640x480_30FPS_RGB);
					
                    if (mHDCam.IsBusy || mHDCam.IsOpen)
						mHDCam.Close();

                    break;
            }
        }
    }
}
