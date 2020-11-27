using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using OpenCVUnity;
using System.IO;

namespace BuddyApp.DiagnosticProd
{
    public sealed class CamerasWindow : MonoBehaviour
    {
        [SerializeField]
        private RawImage SelectedImage;

        [SerializeField]
        private Dropdown SelectedCameraDropdown;

        [SerializeField]
        private Dropdown SelectedResolutionDropdown;

        [SerializeField]
        private Text CPUTemperature;

        [SerializeField]
        private Text BODYTemperature;

        private HDCamera mHDCam;
        private RGBCamera mRGBCam;
        private DepthCamera mDepthCam;
        private InfraredCamera mInfraredCam;
        private float mTimeRefresh;

        public void OnEnable()
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


        private void Update()
        {
            mTimeRefresh += Time.deltaTime;
            if (mTimeRefresh >= DiagnosticProdBehaviour.REFRESH_TIMER) {
                BODYTemperature.text = Buddy.Sensors.IMU.Temperature + " °";
                CPUTemperature.text = File.ReadAllText("/sys/class/thermal/thermal_zone0/temp").Replace("000", "") + " °";
                mTimeRefresh = 0F;
            }
        }

        public void OnDisable()
        {
            SelectedCameraDropdown.onValueChanged.RemoveAllListeners();
            SelectedResolutionDropdown.onValueChanged.RemoveAllListeners();
            mHDCam.OnNewFrame.Clear();
            mHDCam.Close();
            //CloseAllCam();
        }

        private void OnCameraDropdownValueChanged()
        {
            SelectedResolutionDropdown.onValueChanged.RemoveAllListeners();
            SelectedResolutionDropdown.ClearOptions();

            switch (SelectedCameraDropdown.options[SelectedCameraDropdown.value].text)
            {
                case "HD_FOCUS":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(HDCameraMode))));
                    break;

                case "HD_WIDE_ANGLE":
                    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(HDCameraMode))));
                    break;

                    //case "RGB":
                    //    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(RGBCameraMode))));
                    //    break;

                    //case "DEPTH":
                    //    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(DepthCameraMode))));
                    //    break;

                    //case "INFRARED":
                    //    SelectedResolutionDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(InfraredCameraMode))));
                    //    break;
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
            mHDCam.OnNewFrame.Clear();
            mHDCam.Close();
            //CloseAllCam();

            switch (SelectedCameraDropdown.options[SelectedCameraDropdown.value].text)
            {

                case "HD_FOCUS":
                    mHDCam.Open(Enum.GetValues(typeof(HDCameraMode)).Cast<HDCameraMode>().Last(), HDCameraType.BACK);
                    mHDCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;

                case "HD_WIDE_ANGLE":
                    mHDCam.Open(Enum.GetValues(typeof(HDCameraMode)).Cast<HDCameraMode>().Last(), HDCameraType.FRONT);
                    mHDCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                    break;

                //case "RGB":
                //    mRGBCam.Open((RGBCameraMode)Enum.Parse(typeof(RGBCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                //    mRGBCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                //    break;

                //case "DEPTH":
                //    mDepthCam.Open((DepthCameraMode)Enum.Parse(typeof(DepthCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                //    mDepthCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                //    break;

                //case "INFRARED":
                //    mInfraredCam.Open((InfraredCameraMode)Enum.Parse(typeof(InfraredCameraMode), SelectedResolutionDropdown.options[SelectedResolutionDropdown.value].text));
                //    mInfraredCam.OnNewFrame.Add((iInput) => { SelectedImage.texture = iInput.Texture; });
                //    break;
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
