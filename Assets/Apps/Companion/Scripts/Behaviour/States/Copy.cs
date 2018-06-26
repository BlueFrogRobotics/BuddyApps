using UnityEngine;
using Buddy;
using OpenCVUnity;
using System;

namespace BuddyApp.Companion
{
    public class Copy : AStateMachineBehaviour
    {
        private RGBCam mCam;
        private float mTimeRotation;
        private float mTime;
        private MoodType mMood;
        private float mHeadPosition;
        private int mFrame;

        // Use this for initialization
        public override void Start()
        {
            mTimeRotation = 0F;
            mTime = 0F;
            mFrame = 0;
            mMood = MoodType.NEUTRAL;

            mCam = BYOS.Instance.Primitive.RGBCam;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(0F, 400F);

            // We set YesHinge Position to -30 to have a better vision of the face
            mHeadPosition = CompanionData.Instance.mHeadPosition;
            CompanionData.Instance.mHeadPosition = -30F;
            if (!mCam.IsOpen)
            {
                mCam.Open(RGBCamResolution.W_320_H_240);
                BYOS.Instance.Perception.FaceDetect.OnDetect(RecoverFaceEntity);
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            if (mCam.IsOpen)
            {
                mTime += Time.deltaTime;
                mTimeRotation += Time.deltaTime;
                ++mFrame;

                // Set Mood every 5 frames
                if (mFrame == 5)
                {
                    if ((int)mMood != -1)
                        BYOS.Instance.Interaction.Mood.Set(mMood);
                    mFrame = 0;
                }

                // Reset YesHinge position to -30
                if (BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition > -20)
                    BYOS.Instance.Primitive.Motors.YesHinge.SetPosition(-30F, 400F);

                // Reset the NoHinge to 0
                if (mTimeRotation > 1F && Math.Abs(BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition) > 2)
                {
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(0F, 400F);
                    mTimeRotation = 0F;
                }

                if (mTime > 60F)
                    Trigger("VOCALCOMMAND");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iAnimatorStateInfo, int iLayerIndex)
        {
            if (mCam.IsOpen)
                mCam.Close();

            mMood = MoodType.NEUTRAL;
            BYOS.Instance.Interaction.Mood.Set(mMood);

            mTimeRotation = 0F;
            mTime = 0F;
            mFrame = 0;

            CompanionData.Instance.mHeadPosition = mHeadPosition;
            BYOS.Instance.Perception.FaceDetect.StopOnDetect(RecoverFaceEntity);

        }

        /// <summary>
        /// CallBack to recover the movement of the face
        /// </summary>
        /// <param name="iEntities">Face details of the user</param>
        /// <returns></returns>
        private bool RecoverFaceEntity(FaceEntity[] iEntities)
        {

            //Recover the Frame and set the mood of the user on the robot
            Mat lMat = BYOS.Instance.Primitive.RGBCam.FrameMat.clone();
            mMood = BYOS.Instance.Perception.FaceDetect.RecognizeEmotion(iEntities[0], lMat);

            switch (iEntities[0].Pose)
            {
                case PoseOrientation.LEFTPROFILE:
                    Debug.Log("LEFT");
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(90F, 200F);
                    break;
                case PoseOrientation.RIGHTPROFILE:
                    Debug.Log("RIGHT");
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(-90F, 200F);
                    break;
                case PoseOrientation.FACIAL:
                    Debug.Log("FACIAL");
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(0F, 200F);
                    break;
                case PoseOrientation.ROTATED30:
                    Debug.Log("ROTATED30");
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(45F, 200F);
                    break;
                case PoseOrientation.ROTATED330:
                    Debug.Log("ROTATED330");
                    BYOS.Instance.Primitive.Motors.NoHinge.SetPosition(-45F, 200F);
                    break;
                default:
                    break;
            }

            return (true);
        }
    }
}