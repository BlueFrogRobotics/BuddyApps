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

            Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_1056X784_30FPS_RGB, HDCameraType.FRONT);

            mVideo.gameObject.SetActive(true);
            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => { mVideo.texture = iInput.Texture; });

            mCoef = 0.5F; ;
        }

        void Update()
        {

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
            if (Input.GetKeyDown("k"))
            {
                mVideo.gameObject.SetActive(!mVideo.gameObject.activeSelf);
         
            }

            if (Input.GetKeyDown("left shift"))
            {
                mLeftShift = true;
            }
            else if (Input.GetKeyUp("left shift"))
            {
                mLeftShift = false;
            }

            //Flash
            if (Input.GetKeyDown("f"))
            {
                if (Buddy.Actuators.LEDs.FlashIntensity != 0F)
                    Buddy.Actuators.LEDs.Flash = false;
                else if (mLeftShift)
                {
                    Debug.Log("Big flash");
                    Buddy.Actuators.LEDs.FlashIntensity = 0.5F;
                }
                else
                {
                    Debug.Log("small flash");
                    Buddy.Actuators.LEDs.FlashIntensity = 0.03F;
                }
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
            if (Input.GetKeyDown("z"))
                Buddy.Actuators.Head.Yes.SetPosition(Buddy.Actuators.Head.Yes.Angle + (15F * mCoef));

            if (Input.GetKeyDown("s"))
                Buddy.Actuators.Head.Yes.SetPosition(Buddy.Actuators.Head.Yes.Angle - (15F * mCoef));


            if (Input.GetKeyDown("q"))
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.Angle + (15F * mCoef));


            if (Input.GetKeyDown("d"))
                Buddy.Actuators.Head.No.SetPosition(Buddy.Actuators.Head.No.Angle - (15F * mCoef));




            ////////////////////////////////////////////////////////////
            //BI
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Buddy.Behaviour.Interpreter.RunRandom(Mood.HAPPY);
                Buddy.Behaviour.Interpreter.Run("Sequence01");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.ANGRY);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.NEUTRAL);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.SAD);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.LOVE);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.SCARED);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.SICK);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.SURPRISED);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                Buddy.Behaviour.Interpreter.RunRandom(Mood.TIRED);


            ///////////////////////////////////////////
            ///Leds
            if (Input.GetKeyDown("w"))
            {
                Debug.Log("numpad f1");
                Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BASIC_BLINK);
            }
            else if (Input.GetKeyDown("x"))
            {
                Buddy.Actuators.LEDs.SetBodyLights(20, 150, 250);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.BREATHING);
            }
            else if (Input.GetKeyDown("c"))
            {
                Buddy.Actuators.LEDs.SetBodyLights(300, 250, 3);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.DYNAMIC);
            }
            else if (Input.GetKeyDown("v"))
            {
                Buddy.Actuators.LEDs.SetBodyLights(120, 150, 240);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.HEART_BEAT);
            }
            else if (Input.GetKeyDown("b"))
            {
                Buddy.Actuators.LEDs.SetBodyLights(90, 10, 140);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.LISTENING);
            }
            else if (Input.GetKeyDown("n"))
            {
                Buddy.Actuators.LEDs.SetBodyLights(1, 100, 250);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.NOBLINK);
            }
            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                Buddy.Actuators.LEDs.SetBodyLights(60, 100, 93);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.PEACEFUL);
            }
            else if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                Buddy.Actuators.LEDs.SetBodyLights(60, 100, 250);
                Buddy.Actuators.LEDs.SetBodyPattern(LEDPulsePattern.RECHARGE);
            }

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