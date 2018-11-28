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

        [SerializeField]
        private Dropdown SelectedResolutionDropdown;

        private HDCamera mHDCam;
        private RGBCamera mRGBCam;
        private DepthCamera mDepthCam;
        private InfraredCamera mInfraredCam;
        
        void OnEnable()
        {
            mRGBCam = Buddy.Sensors.RGBCamera;
            mHDCam = Buddy.Sensors.HDCamera;
            mDepthCam = Buddy.Sensors.DepthCamera;
            mInfraredCam = Buddy.Sensors.InfraredCamera;

            // Initialization
            OnCameraDropdownValueChanged();

            SelectedCameraDropdown.onValueChanged.AddListener(delegate {
                OnCameraDropdownValueChanged();
            });
        }

        void OnDisable()
        {
            SelectedCameraDropdown.onValueChanged.RemoveAllListeners();
            SelectedResolutionDropdown.onValueChanged.RemoveAllListeners();
            CloseAllCam();
        }

        private void OnCameraDropdownValueChanged()
        {
            SelectedResolutionDropdown.onValueChanged.RemoveAllListeners();
            SelectedResolutionDropdown.ClearOptions();

            switch (SelectedCameraDropdown.options[SelectedCameraDropdown.value].text)
            {
                case "HD":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(HDCameraMode))));
                    break;

                case "RGB":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(RGBCameraMode))));
                    break;

                case "DEPTH":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(DepthCameraMode))));
                    break;

                case "INFRARED":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(InfraredCameraMode))));
                    break;
            }

            SelectedResolutionDropdown.value = 0;

            // Set callback when changing resolution
            SelectedResolutionDropdown.onValueChanged.AddListener(delegate {
                OnResolutionDropdownValueChanged();
            });

            OnResolutionDropdownValueChanged();
        }

        private void OnResolutionDropdownValueChanged()
        {
            CloseAllCam();
            
            switch (SelectedCameraDropdown.options[SelectedCameraDropdown.value].text)
            {
                case "HD":
                    mHDCam.Open((HDCameraMode)Enum.Parse(typeof(HDCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                    mHDCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;

                case "RGB":
                    mRGBCam.Open((RGBCameraMode)Enum.Parse(typeof(RGBCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                    mRGBCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;

                case "DEPTH":
                    mDepthCam.Open((DepthCameraMode)Enum.Parse(typeof(DepthCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                    mDepthCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;

                case "INFRARED":
                    mInfraredCam.Open((InfraredCameraMode)Enum.Parse(typeof(InfraredCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                    mInfraredCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;
            }
        }

        private void CloseAllCam()
        {
            mHDCam.OnNewFrame.Clear();
            mRGBCam.OnNewFrame.Clear();
            mDepthCam.OnNewFrame.Clear();
            mInfraredCam.OnNewFrame.Clear();

            mHDCam.Close();
            mRGBCam.Close();
            mDepthCam.Close();
            mInfraredCam.Close();
        }
    }
}
