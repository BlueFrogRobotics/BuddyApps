using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Buddy;
using Buddy.UI;
using OpenCVUnity;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionDetection : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private MotionDetection mMotion;
        private Mat mMat;
        Texture2D mTexture;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Interaction.TextToSpeech.Silence(1000);
            mMotion = Perception.Motion;
            mMotion.enabled = true;
            mCam = Primitive.RGBCam;
            mMotion.OnDetect(OnMovementDetected, 15f);
            mTexture = mCam.FrameTexture2D;
            mMat = Utils.Texture2DToMat(mTexture, OpenCVUnity.CvType.CV_8UC3);
            Toaster.Display<PictureToast>().With(Dictionary.GetString("lookphoto"), Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
            //StartCoroutine(Defeat());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mMotion.StopOnDetect(OnMovementDetected);
        }

        private IEnumerator Defeat()
        {
            
            //Texture2D lTexture = LoadPNG("C:/Users/Walid/Pictures/buddy.png");
            
            
            Interaction.Mood.Set(MoodType.SAD);
            yield return new WaitForSeconds(3);
            Interaction.Mood.Set(MoodType.HAPPY);
            Toaster.Hide();
            Trigger("StartGame");
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            Debug.Log("detection mouvement");
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMat, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), -1);
            }
            mTexture = Utils.MatToTexture2D(mMat);
            return true;
        }

     }

}
