using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;
using System;

namespace BuddyApp.TestHDCam
{
    public class TestHDCam : MonoBehaviour
    {

        //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam ");
        //Utils.DeleteFile(Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
        //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 1");
        //mRTCManager.MuteVideo(true);
        //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 4");
        //mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.FRONT);
        //Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 5");
        //Texture2D iPhotoFromRobotTest;
        //mHDCam.OnNewFrame.Add((iInput) => {
        //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 6");
        //    if(iInput.Texture != null)
        //    {
        //        Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 6 : height " + iInput.Texture.height + " width : " + iInput.Texture.width);
        //        iPhotoFromRobotTest = iInput.Texture;
        //        Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 7 : " + Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");
        //        Utils.SaveTextureToFile(iPhotoFromRobotTest, Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg");

        //    }
        //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 8");
        //    string iPathPhotoSavedTest = Buddy.Resources.AppRawDataPath + "phototaken" + ".jpg";
        //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 9 : " + iPathPhotoSavedTest);
        //    Buddy.WebServices.Agoraio.SendPicture(mRTMManager.IdConnectionTablet, iPathPhotoSavedTest);
        //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 10");
        //    mHDCam.OnNewFrame.Clear();
        //    mHDCam.Close();
        //    mRTCManager.MuteVideo(false);
        //    Debug.LogError("*****CALLSTATE TAKE PHOTO WITH test hdcam 11");

        //});

        private HDCamera mHDCam;
        [SerializeField]
        private RawImage mRaw;

        [SerializeField]
        private Dropdown Dropdown;

        private float timer;
        private string pathPhoto;
        private bool isFront;
        private bool IsPhotoTaken;

        // Start is called before the first frame update
        void Start()
        {
            //HDCameraMode.COLOR_1056X784_30FPS_RGB;
            //HDCameraMode.COLOR_1056X784_60FPS_RGB;
            //HDCameraMode.COLOR_132X98_15FPS_RGB;
            //HDCameraMode.COLOR_132X98_60FPS_RGB;
            //HDCameraMode.COLOR_2112x1568_15FPS_RGB;
            //HDCameraMode.COLOR_2112x1568_30FPS_RGB;
            //HDCameraMode.COLOR_2112x1568_60FPS_RGB;
            //HDCameraMode.COLOR_264X196_15FPS_RGB;
            //HDCameraMode.COLOR_264X196_60FPS_RGB;
            //HDCameraMode.COLOR_528X392_30FPS_RGB;
            //HDCameraMode.COLOR_528X392_60FPS_RGB;

            Dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(HDCameraMode))));
            pathPhoto = "";
            IsPhotoTaken = false;
            timer = 0F;
            isFront = true;
            mHDCam = Buddy.Sensors.HDCamera;
            //Debug.LogError("input camera 0");
            //mHDCam.Close();
            //Debug.LogError("input camera 1");
            //if(isFront)
            //    mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.FRONT);
            //else
            //    mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.BACK);
            //Debug.LogError("input camera 2");
            //mHDCam.OnNewFrame.Clear();
            //Debug.LogError("input camera 2.1");

            //mHDCam.OnNewFrame.Clear();

            //mHDCam.TakePhotograph(OnPhotoTaken);
            




            //mHDCam.OnNewFrame.Add((iInput) => {
            //    Debug.LogError("input camera 3");
            //    //mRaw.texture.height = iInput.Texture.height;
            //    //mRaw.texture.width = iInput.Texture.width;
            //    mRaw.texture = iInput.Texture;
            //});
            
        
        }

        // Update is called once per frame
        void Update()
        {
            //timer += Time.deltaTime;
            //if(timer > 6F && !IsPhotoTaken)
            //{
            //    IsPhotoTaken = true;
            //    Utils.DeleteFile(pathPhoto); 
            //    mHDCam.Close();
            //    Debug.LogError("input camera update close");
            //    if (isFront)
            //        mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.FRONT);
            //    else
            //        mHDCam.Open(HDCameraMode.COLOR_4224x3136_30FPS_RGB, HDCameraType.BACK);
            //    Debug.LogError("input camera update open 4224*3136");
            //    mHDCam.OnNewFrame.Clear();
            //    Debug.LogError("input camera update clear");

            //    //mHDCam.OnNewFrame.Clear();
            //    mHDCam.TakePhotograph(OnPhotoTaken);
            //}
        }


        private void OnPhotoTaken(Photograph iMyPhoto)
        {
            if (isFront)
                isFront = !isFront;
            Debug.LogError("TAKE PHOTOGRAPH ISFRONT : " + isFront);
            if (iMyPhoto == null)
            {
                Debug.Log("OnFinish take photo, iPhoto null");
                return;
            }
            iMyPhoto.Save();
            Debug.LogError("TAKE PHOTOGRAPH save");
            pathPhoto = iMyPhoto.FullPath;
            
            //Buddy.GUI.Toaster.Display<PictureToast>().With(iMyPhoto.Image);
            Debug.LogError("Path photo taken : " + iMyPhoto.FullPath);
            
            Debug.LogError("TAKE PHOTOGRAPH clear");
            mHDCam.Close();
            Debug.LogError("TAKE PHOTOGRAPH close");
            timer = 0F;
            IsPhotoTaken = false;
        }

        private IEnumerator TestakePhoto()
        {
            yield return new WaitForSeconds(4F);
            mHDCam.TakePhotograph(OnPhotoTaken);
        }

        public void TakePhoto()
        {
            Debug.LogError("TESTHDCAM TAKEPHOTO");
            if (!IsPhotoTaken)
            {
                IsPhotoTaken = true;
                StartCoroutine(TestakePhoto());
            }
        }

        public void OnChangeResolution()
        {
            mHDCam.OnNewFrame.Clear();
            mHDCam.Close();

            mHDCam.Open((HDCameraMode)Enum.Parse(typeof(HDCameraMode),Dropdown.options[Dropdown.value].text), HDCameraType.FRONT);
            Debug.LogError("TESTHDCAM Change resolution : " + Dropdown.options[Dropdown.value].text);
        }
    }

}
