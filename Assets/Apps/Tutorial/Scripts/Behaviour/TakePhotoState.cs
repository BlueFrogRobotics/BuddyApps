using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;
using System.IO;

namespace BuddyApp.Tutorial
{
    enum StepTakePhoto : int
    {
        DISPLAY_CAMERA = 0,
        TAKE_PHOTO = 1,
        DISPLAY_PHOTO_TAKEN = 2,
        SAVE_PHOTO = 3,
        DONE = 4
    }

    /// <summary>
    /// In this state we take a photo and then we save the photo in a folder and we show you how you can access to this photo
    /// </summary>
    public class TakePhotoState : AStateMachineBehaviour
    {
        private StepTakePhoto mStepTP;
        private Mat mMatSrc;
        private Texture2D mTextPhoto;
        //private string mFileName;
        //private string mFilePath;
        private bool mSentenceDone;
        private Sprite mPhotoSprite;
        private Photograph mPhotograph;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //You define the name of your photo, here I chose to put the date when I took the photo
            //mFileName = "Buddy_" + System.DateTime.Now.Day + "day" +
            //            System.DateTime.Now.Month + "month" + System.DateTime.Now.Hour + "h" +
            //            System.DateTime.Now.Minute + "min" + System.DateTime.Now.Second + "sec.png";
            //mFilePath = "";


            mStepTP = StepTakePhoto.DISPLAY_CAMERA;
            mSentenceDone = false;
            Buddy.Vocal.SayKey("tpstateintro");
            if (Buddy.Sensors.HDCamera.IsBusy)
            {
                Buddy.Sensors.HDCamera.Close();
            }
            if (!Buddy.Sensors.HDCamera.IsOpen)
            {
                Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            }
            mTextPhoto = Utils.MatToTexture2D(Buddy.Sensors.HDCamera.Frame);
            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!Buddy.Vocal.IsSpeaking && mStepTP == StepTakePhoto.DISPLAY_CAMERA)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextPhoto);
                mStepTP = StepTakePhoto.TAKE_PHOTO;
            }
            else if(mStepTP == StepTakePhoto.TAKE_PHOTO)
            {
                if(!mSentenceDone)
                {
                    Buddy.Vocal.SayKey("tpstatesteptakephoto");
                    mSentenceDone = true;
                }
                else if (!Buddy.Vocal.IsSpeaking && mSentenceDone)
                {
                    mStepTP = StepTakePhoto.DISPLAY_PHOTO_TAKEN;
                    Buddy.Sensors.HDCamera.TakePhotograph(OnFinish, true);
                }
            }
            else if(mStepTP == StepTakePhoto.DISPLAY_PHOTO_TAKEN)
            {
                if (!Buddy.GUI.Toaster.IsBusy)
                {
                    Buddy.Vocal.SayKey("tpstatephototaken");
                    Buddy.GUI.Toaster.Display<PictureToast>().With(mPhotoSprite);
                    mStepTP = StepTakePhoto.SAVE_PHOTO;
                }
            }
            else if(mStepTP == StepTakePhoto.SAVE_PHOTO)
            {
                if (!Buddy.Vocal.IsSpeaking)
                {
                    //Peut etre demander si l'utilisateur veut enregistrer mais il y a deja un exemple de widget pour le toaster oui/non plus tard donc 
                    //peut etre seulement sauvegarder l'image directement pour montrer comment le faire en code
                    //Buddy.Vocal.SayKey("");

                    //You can see your picture in your persistentDataPath/Users/"name of the user"/AppData/"id of the app"/Pictures
                    mPhotograph.Save();
                    Buddy.GUI.Toaster.Hide();
                    mStepTP = StepTakePhoto.DONE;
                }
            }
            else if (mStepTP == StepTakePhoto.DONE)
            {
                Trigger("MenuTrigger");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        private void OnFrameCaptured(Mat iInput)
        {
            mMatSrc = iInput;
            Core.flip(mMatSrc, mMatSrc, 1);
            Utils.MatToTexture2D(mMatSrc, mTextPhoto);
        }

        private void OnFinish(Photograph iMyPhoto)
        {
            mPhotograph = iMyPhoto;
            Buddy.GUI.Toaster.Hide();
            mPhotoSprite = iMyPhoto.Image;
        }
    }
}

