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
        private RawImage SelectedImage;

        [SerializeField]
        private Dropdown SelectedCameraDropdown;

        private HDCamera mHDCam;
        private RGBCamera mRGBCam;
        private DepthCamera mDepthCam;
        private InfraredCamera mInfraredCam;
        private ThermalCamera mThermalCam;

        private enum E_CAMERA { HD, RGB, DEPTH, INFRARED, THERMAL }; // Use enum instead of strings to maintain performances.
        private E_CAMERA mECamera;
        
        void OnEnable()
        {
            mRGBCam = Buddy.Sensors.RGBCamera;
            mHDCam = Buddy.Sensors.HDCamera;
            mDepthCam = Buddy.Sensors.DepthCamera;
            mInfraredCam = Buddy.Sensors.InfraredCamera;
            mThermalCam = Buddy.Sensors.ThermalCamera;

            // Initialization
            DropdownValueChanged(SelectedCameraDropdown);

            // Set callback when changing camera
            SelectedCameraDropdown.onValueChanged.AddListener(delegate {
                DropdownValueChanged(SelectedCameraDropdown);
            });
        }

        void OnDisable()
        {
            SelectedCameraDropdown.onValueChanged.RemoveAllListeners();

            CloseAllCam();
        }

        void DropdownValueChanged(Dropdown iChange)
        {
            switch (iChange.options[iChange.value].text)
            {
                case "HD":
                    mECamera = E_CAMERA.HD;

                    CloseAllCam();

                    if (!mHDCam.IsOpen)
                        mHDCam.Open(HDCameraMode.COLOR_1408x792_15FPS_RGB);

                    mHDCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });

                    break;

                case "RGB":
                    mECamera = E_CAMERA.RGB;

                    CloseAllCam();

                    if (!mRGBCam.IsOpen)
                        mRGBCam.Open(RGBCameraMode.COLOR_640x480_30FPS_RGB);

                    mRGBCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });

                    break;

                case "DEPTH":
                    mECamera = E_CAMERA.DEPTH;

                    CloseAllCam();

                    if (!mDepthCam.IsOpen)
                        mDepthCam.Open(DepthCameraMode.DEPTH_640x480_30FPS_1MM);

                    mDepthCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });

                    break;

                case "INFRARED":
                    mECamera = E_CAMERA.INFRARED;

                    CloseAllCam();
                    
                    if (!mInfraredCam.IsOpen)
                        mInfraredCam.Open(InfraredCameraMode.IR_640x480_30FPS_GRAY16);

                    mInfraredCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });

                    break;

                case "THERMAL":
                    mECamera = E_CAMERA.THERMAL;

                    CloseAllCam();

                    mThermalCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;
            }
        }

        private void CloseAllCam()
        {
            mHDCam.OnNewFrame.Clear();
            mRGBCam.OnNewFrame.Clear();
            mDepthCam.OnNewFrame.Clear();
            mInfraredCam.OnNewFrame.Clear();
            mThermalCam.OnNewFrame.Clear();

            mHDCam.Close();
            mRGBCam.Close();
            mDepthCam.Close();
            mInfraredCam.Close();
        }
    }
}
