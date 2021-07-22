using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.BuddyRemote
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BuddyRemoteBehaviour : MonoBehaviour
    {

        [SerializeField]
        private RawImage mVideo;

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private BuddyRemoteData mAppData;
        private bool mForward;
        private bool mLeft;
        private bool mRight;
        private bool mBackward;
        private bool mLeftShift;
        private float mCoef;
        private float mTimeSinceMovement;
        private int mPreviousRotate;
        private int mPreviousTranslate;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            BuddyRemoteActivity.Init(null);

            Buddy.GUI.Header.DisplayParametersButton(false);

            /*
			* Init your app data
			*/
            mAppData = BuddyRemoteData.Instance;

            if (Buddy.Sensors.HDCamera.IsBusy)
            {
                Buddy.Sensors.HDCamera.Close();
            }

            //Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_1056X784_30FPS_RGB, HDCameraType.FRONT);

            //mVideo.gameObject.SetActive(true);
            //Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => { mVideo.texture = iInput.Texture; });

            mCoef = 0.9F; ;
        }

        void Update()
        {
            //Debug.Log("[BuddyRemote] Update");

            bool lNewMotion = false;

            // hide / display icons
            if (Input.GetKeyDown("h"))
            {
                //hide icons
                Buddy.GUI.Header.Hide();
            }
            if (Input.GetKeyDown("j"))
            {
                //display icons
                Buddy.GUI.Header.Display();
            }

            //Camera feedback
            /*if (Input.GetKeyDown("k"))
            {
                mVideo.gameObject.SetActive(!mVideo.gameObject.activeSelf);
            }*/

            if (Input.GetKeyDown("left shift"))
            {
                mLeftShift = true;
            }
            else if (Input.GetKeyUp("left shift"))
            {
                mLeftShift = false;
            }


            //Adapt intensity of motion
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {

                mCoef -= 0.1F;
                Debug.LogWarning("Coef " + mCoef);
            }

            if (Input.GetKeyDown(KeyCode.Period))
            {
                mCoef += 0.1F;
                Debug.LogWarning("Coef " + mCoef);
            }

            if (mCoef < 0.1F)
                mCoef = 0.1F;
            if (mCoef > 1F)
                mCoef = 1F;

            //Move
            if (Input.GetKeyDown("up"))
            {
                mForward = true;
                lNewMotion = true;
                Debug.LogWarning("forward " + mForward);
            }
            else if (Input.GetKeyUp("up"))
            {
                mForward = false;
                lNewMotion = true;
            }


            if (Input.GetKeyDown("down"))
            {
                Debug.LogWarning("backward " + mForward);
                mBackward = true;
                lNewMotion = true;
            }
            else if (Input.GetKeyUp("down"))
            {
                mBackward = false;
                lNewMotion = true;
            }


            if (Input.GetKeyDown("right"))
            {
                mRight = true;
                lNewMotion = true;
            }
            else if (Input.GetKeyUp("right"))
            {
                mRight = false;
                lNewMotion = true;
            }


            if (Input.GetKeyDown("left"))
            {
                mLeft = true;
                lNewMotion = true;
            }
            else if (Input.GetKeyUp("left"))
            {
                mLeft = false;
                lNewMotion = true;
            }

            if (!mForward && !mBackward && !mRight && !mLeft && Buddy.Actuators.Wheels.IsBusy)
            {
                Debug.LogWarning("Stop! no motion");
                mPreviousRotate = 0;
                mPreviousTranslate = 0;
                mTimeSinceMovement = -1F;
                Buddy.Actuators.Wheels.SetVelocities(0F, 0F, AccDecMode.HIGH);
            }

            // if new motion event, update command
            if (lNewMotion)
                ExecuteMotion();


            ///////////////////////////////////////////////////////////////
            //Move Head
            /*if (Input.GetKeyDown("z"))
                Buddy.Actuators.Head.Yes.SetPosition(Buddy.Actuators.Head.Yes.Angle + (15F * mCoef));

            if (Input.GetKeyDown("s"))
                Buddy.Actuators.Head.Yes.SetPosition(Buddy.Actuators.Head.Yes.Angle - (15F * mCoef));


            if (Input.GetKeyDown("q"))
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.Angle + (15F * mCoef));


            if (Input.GetKeyDown("d"))
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.Angle - (15F * mCoef));
            */



            ////////////////////////////////////////////////////////////
            //BI
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.HAPPY)
                Buddy.Behaviour.Interpreter.Run("Sequence00");
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.HAPPY);
                Buddy.Behaviour.Interpreter.Run("Sequence01");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.ANGRY);
                Buddy.Behaviour.Interpreter.Run("Sequence02");
            if (Input.GetKeyDown(KeyCode.Alpha3))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.NEUTRAL);
                Buddy.Behaviour.Interpreter.Run("Sequence03");
            if (Input.GetKeyDown(KeyCode.Alpha4))
                Buddy.Behaviour.Interpreter.Run("Speak00");
            //Buddy.Behaviour.Interpreter.RunRandom(Mood.SAD);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                Buddy.Behaviour.Interpreter.Run("Speak01");
            //Buddy.Behaviour.Interpreter.RunRandom(Mood.LOVE);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SCARED);
                Buddy.Behaviour.Interpreter.Run("Speak02");
            if (Input.GetKeyDown(KeyCode.Alpha7))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SICK);
                Buddy.Behaviour.Interpreter.Run("Speak03");
            if (Input.GetKeyDown(KeyCode.Alpha8))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SURPRISED);
                Buddy.Behaviour.Interpreter.Run("Speak04");
            if (Input.GetKeyDown(KeyCode.Alpha9))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.TIRED);
                Buddy.Behaviour.Interpreter.Run("Speak05");
            
           // if (Input.GetKeyDown("°"))
           //     Buddy.Behaviour.Interpreter.Run("SetBack");
            if (Input.GetKeyDown("p"))
                Buddy.Behaviour.Interpreter.Run("SetBack");
            //if (Input.GetKeyDown("`"))
            //    Buddy.Behaviour.Interpreter.Run("SetBack");
            //if (Input.GetKeyDown("²"))
            //    Buddy.Behaviour.Interpreter.Run("SetBack");
            //if (Input.GetKeyDown("#"))
            //    Buddy.Behaviour.Interpreter.Run("SetBack");
            if (Input.GetKeyDown("q"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-01");
            if (Input.GetKeyDown("a"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-01");
            if (Input.GetKeyDown("w"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-02");
            if (Input.GetKeyDown("z"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-02");
            if (Input.GetKeyDown("e"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-03");
            if (Input.GetKeyDown("r"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-04");
            if (Input.GetKeyDown("t"))
                Buddy.Behaviour.Interpreter.Run("Sequence01-05");


            /*if (Input.GetKeyDown("1"))
            {
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.HAPPY);
                Buddy.Behaviour.Interpreter.Run("Sequence00");
            }
            if (Input.GetKeyDown("2"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.ANGRY);
                Buddy.Behaviour.Interpreter.Run("Sequence01");
            if (Input.GetKeyDown("3"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.NEUTRAL);
                Buddy.Behaviour.Interpreter.Run("Sequence02");
            if (Input.GetKeyDown("4"))
                Buddy.Behaviour.Interpreter.Run("Sequence03");
            //Buddy.Behaviour.Interpreter.RunRandom(Mood.SAD);
            if (Input.GetKeyDown("5"))
                Buddy.Behaviour.Interpreter.Run("Speak00");
            //Buddy.Behaviour.Interpreter.RunRandom(Mood.LOVE);
            if (Input.GetKeyDown("6"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SCARED);
                Buddy.Behaviour.Interpreter.Run("Speak01");
            if (Input.GetKeyDown("7"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SICK);
                Buddy.Behaviour.Interpreter.Run("Speak02");
            if (Input.GetKeyDown("8"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.SURPRISED);
                Buddy.Behaviour.Interpreter.Run("Speak03");
            if (Input.GetKeyDown("9"))
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.TIRED);
                Buddy.Behaviour.Interpreter.Run("Speak04");*/


            if (OneOrMoreCliff())
                Buddy.Actuators.Wheels.UnlockWheels();

        }


        public bool OneOrMoreCliff()
        {
            return BackCliff() || FrontCliff();
        }
        public bool BackCliff()
        {
            return Buddy.Sensors.CliffSensors.BackLeftFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackRightFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackLeftWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.BackRightWheel.Value > 65F;
        }
        public bool FrontCliff()
        {
            return Buddy.Sensors.CliffSensors.FrontFreeWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.FrontLeftWheel.Value > 65F ||
                Buddy.Sensors.CliffSensors.FrontRightWheel.Value > 65F;
        }


        private void ExecuteMotion()
        {
            Debug.LogWarning("forward " + mForward);
            Debug.LogWarning("backward " + mBackward);
            Debug.LogWarning("mPreviousTranslate " + mPreviousTranslate);
            int lTranslate = 0;
            int lRotate = 0;

            if (mForward && !mBackward)
                lTranslate = 1;
            else if (!mForward && mBackward)
                lTranslate = -1;

            if (mRight && !mLeft)
                lRotate = -1;
            else if (!mRight && mLeft)
                lRotate = 1;

            Debug.LogWarning("ltranslate " + lTranslate);

            if ((lRotate != 0 && lRotate != mPreviousRotate) || (lTranslate != 0 && lTranslate != mPreviousTranslate))
            {
                mPreviousRotate = lRotate;
                mPreviousTranslate = lTranslate;
                mTimeSinceMovement = Time.time;

                // Adapt when moving backward
                if (lTranslate < -0.17)
                {
                    // inverse angles of rotation
                    lRotate = -lRotate;
                    // Limit vitess wen going backward

                }

                if (lTranslate < -0.4F)
                    Buddy.Actuators.Wheels.SetVelocities(Wheels.MAX_LIN_VELOCITY * Mathf.Pow(lTranslate * mCoef / 2, 3),
                    Wheels.MAX_ANG_VELOCITY / 10 * Mathf.Pow(lRotate * mCoef, 3), AccDecMode.HIGH);
                else
                    Buddy.Actuators.Wheels.SetVelocities(Wheels.MAX_LIN_VELOCITY * Mathf.Pow(lTranslate * mCoef, 3),
                    Wheels.MAX_ANG_VELOCITY / 10 * Mathf.Pow(lRotate * mCoef, 3), AccDecMode.HIGH);

                Debug.LogWarning("Wheels! " + (Wheels.MAX_LIN_VELOCITY * Mathf.Pow(lTranslate * mCoef, 3)) + " " +
                (Wheels.MAX_ANG_VELOCITY / 10 * Mathf.Pow(lRotate * mCoef, 3)));

            }
            else if (lRotate == 0 && lTranslate == 0)
            {
                mPreviousRotate = 0;
                mPreviousTranslate = 0;
                mTimeSinceMovement = -1F;
                Debug.LogWarning("Stop! in Execute");
                Buddy.Actuators.Wheels.SetVelocities(0F, 0F, AccDecMode.HIGH);
            }


        }


    }
}