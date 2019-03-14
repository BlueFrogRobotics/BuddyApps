using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCVUnity;

namespace BuddyApp.TestGridEye
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestGridEyeBehaviourV2 : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestGridEyeData mAppData;

        private const float DIRECTION_OFFSET_LIMIT = 0.2F;
        private const float DIRECTION_THRESHOLD = 0.2F;
        private const float ANGULAR_SPEED = 80F;
        private const float LINEAR_SPEED = 0.3F;
        private const int MEASURE_NUMBER = 2;

        private double mLastDirection;
        private float mObstacleCount;
        private int mMeasure;

        private Sprite mSprite;
        private float[] mThermalSensorDataArray;
        private float mAverageLow;
        private Texture2D mLiTexture;
        private Texture2D mLiTexture30;
        private float[] mThermalSensorDataArray15;

        [SerializeField]
        private Text Score;

        [SerializeField]
        private Text Position;

        [SerializeField]
        private Text AverageLow;

        [SerializeField]
        private InputField mTempMax;

        [SerializeField]
        private InputField mTempNeighbours;

        [SerializeField]
        private InputField mScoreThresh;

        CircularBuffer<float> mAverageThermal;
        private int mScore;

        private bool mDisplay;
        private bool mMotion;
        private bool mFollow;
        private float mTimeHumanDetected;
        private bool mPreviousLeft;
        private bool mStopped;

        public void ToggleDisplay()
        {
            mDisplay = !mDisplay;
        }

        public void ToggleMotion()
        {
            mMotion = !mMotion;
            mFollow = false;
            mStopped = true;
            Buddy.Actuators.Wheels.Stop();
        }

        public void ToggleFollow()
        {
            mFollow = !mFollow;
            mMotion = false;
            mStopped = true;
            Buddy.Actuators.Wheels.Stop();

            // Init variables
            mObstacleCount = 0;
            mMeasure = 0;
            mLastDirection = 0;
        }

        void Start()
        {
            TestGridEyeActivity.Init(null);

            mDisplay = true;
            mMotion = false;
            mScore = -1;
            mLastDirection = 0;

            // array for all data of the 8*8 matrix
            mThermalSensorDataArray = new float[8*8];

            mLiTexture = new Texture2D(15, 15) {
                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };

            mLiTexture30 = new Texture2D(31, 20) {
                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };

            // array of 15*15 for linear interpolation of the 8*8 
            mThermalSensorDataArray15 = new float[15 * 15];

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);

            mAverageThermal = new CircularBuffer<float>(8);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);
            // interpolate into mThermalSensorDataArray15
            LinearInterpolation();

            // To get an average of the lower temperature of the 8 last frames
            mAverageThermal.Enqueue(mThermalSensorDataArray15.Min());
            mAverageLow = mAverageThermal.Average();
            AverageLow.text = "AverageLow " + mAverageLow + /*" Max: " + lMax +*/ " Min: " + mThermalSensorDataArray15.Min();

            float lMax = mThermalSensorDataArray15.Max();
            float lMin = mThermalSensorDataArray15.Min();

            int lIndexMax = ComputeScore();

            // if human detected - draw heatest point & launch callback if needed
            if (lIndexMax > -1) {

                // Set max temp in white
                mLiTexture.SetPixel(lIndexMax % 15, 14 - (lIndexMax / 15), new Color(1F, 1F, 1F, 1F));

                // TODO: center of human is at center of shape

                Score.text = "Score: " + mScore;
                Position.text = "Position: " + lIndexMax % 15 + ":" + lIndexMax / 15;

                if (mMotion)
                    OnHumanDetect(lIndexMax % 15);
                else if (mFollow)
                    FollowHuman(lIndexMax % 15);

            } else { // No human detect - redraw the image because latest may have some green point
                // interpolate into mThermalSensorDataArray15
                LinearInterpolation();

                // Render the heat data on 15*15 Texture - (shade of blue / red)
                for (int i = 0; i < mThermalSensorDataArray15.Length; i++) {
                    // Calcul ratio of : TemperatureRelativeToTheRange / RangeTemperature
                    float lNormalizedTemp = (mThermalSensorDataArray15[i] - lMin) / (lMax - lMin);
                    // This ratio is used to calcul each shade
                    mLiTexture.SetPixel(i % 15, 14 - i / 15, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
                }
            }

            mLiTexture.Apply();

            // Render the final Sprite, split into two image: Left : processed image , right: original
            for (int i = 0; i < mLiTexture.height + 5; i++)
                for (int j = 0; j < mLiTexture.width; j++) {

                    if (i < mLiTexture.height) {
                        float lNormalizedTemp = (mThermalSensorDataArray[(7 - (i / 2)) * 8 + j / 2] - lMin) / (lMax - lMin);

                        // processed image
                        mLiTexture30.SetPixel(j, i, mLiTexture.GetPixel(j, i));

                        // Draw separation between images
                        mLiTexture30.SetPixel(15, i, new Color(0.0F, 0.0F, 0.0F, 0.85F));

                        // raw image
                        mLiTexture30.SetPixel(j + 16, i, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));

                        // Add some pixel to get full image
                    } else {
                        // Bottom black line
                        mLiTexture30.SetPixel(j, i, new Color(0.0F, 0.0F, 0.0F, 0.85F));
                        // Draw separation between images
                        mLiTexture30.SetPixel(15, i, new Color(0.0F, 0.0F, 0.0F, 0.85F));
                        // Right black line
                        mLiTexture30.SetPixel(j + 16, i, new Color(0.0F, 0.0F, 0.0F, 0.85F));
                    }
                }

            mLiTexture30.Apply();

            mSprite = Sprite.Create(mLiTexture30, new UnityEngine.Rect(0.0f, 0.0f, 31, 15), new Vector2(0.5f, 0.5f), 15.0F);

            if (!Buddy.GUI.Toaster.IsBusy && mDisplay)
                Buddy.GUI.Toaster.Display<PictureToast>().With(mSprite,
                    () => {
                        Buddy.GUI.Toaster.Hide();
                        mDisplay = false;
                    });

        }

        private void Update()
        {
            if (mFollow)
            {
                // Add the average of sensor that found an obstacle
                mObstacleCount += UserInFrontScore();

                // When mMeasurNumber is reach, a new average is compute to reduce false positive
                if (mMeasure == MEASURE_NUMBER)
                {
                    Debug.LogWarning("Obstacle:" + mObstacleCount / (float)MEASURE_NUMBER);
                    // Finally if an obstacle is detected by several sensor, during several frame, we stop.
                    if ((mObstacleCount / (float)MEASURE_NUMBER) > 0.4F && mLastDirection < 9F)
                    {
                        mStopped = true;
                        Buddy.Actuators.Wheels.Stop();
                    }
                    mObstacleCount = 0;
                    mMeasure = 0;
                }
            }
        }

        // Average of Sensor - (Value ​​were chosen empirically)
        private float UserInFrontScore()
        {
            int lScore = 0;
            mMeasure++;
            if (Buddy.Sensors.UltrasonicSensors.Left.Value < 700)
                lScore++;
            if (Buddy.Sensors.UltrasonicSensors.Right.Value < 700)
                lScore++;

            if (Buddy.Sensors.TimeOfFlightSensors.Front.Value < 700)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Right.Value < 600)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Left.Value < 600)
                lScore++;

            if (Buddy.Sensors.TimeOfFlightSensors.Forehead.Value < 850)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Chin.Value < 700)
                lScore++;
            return ((float)lScore / 7F);
        }

        private void MoveToward(float iDirection)
        {
            float lAngularVelocity;

            // If the direction changes enough, update the robot direction
            if (Math.Abs(mLastDirection - iDirection) > DIRECTION_OFFSET_LIMIT)
            {
                mLastDirection = iDirection;
                lAngularVelocity = -iDirection * ANGULAR_SPEED;
                // If the direction is included in the middle range, stop to rotate.
                if (Math.Abs(iDirection) < DIRECTION_THRESHOLD)
                    lAngularVelocity = 0F;
                Buddy.Actuators.Wheels.SetVelocities(LINEAR_SPEED, lAngularVelocity);
            }
        }

        private void FollowHuman(int iHumanHeatestPointX)
        {
            float lCentered = iHumanHeatestPointX / 15F;
            Debug.LogWarning("Humancenter: " + lCentered + " / " + (lCentered - 0.5F));
            MoveToward((lCentered - 0.5F) * -1);
        }

        private bool OnHumanDetect(int iRow)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            mTimeHumanDetected = Time.time;
            float lCentered = iRow / 14F;
            Debug.LogWarning("Humancenter " + lCentered);
            if (lCentered < 0.6F && lCentered > 0.4F) {
                Debug.LogWarning("Stop " + mStopped);
                mStopped = true;
                Buddy.Actuators.Wheels.Stop();

                return false;

                // otherwise, try to put the human in the center
            } else if (lCentered < 0.6F && (!mPreviousLeft || mStopped /*!Buddy.Actuators.Wheels.IsBusy*/)) {
                Debug.LogWarning("Go to left " + mStopped);
                Buddy.Actuators.Wheels.SetVelocities(0F, -35F);
                mStopped = false;
                mPreviousLeft = true;
            } else if (lCentered > 0.4F && (mPreviousLeft || mStopped/*|| !Buddy.Actuators.Wheels.IsBusy*/)) {
                Debug.LogWarning("Go to right " + mStopped);
                Buddy.Actuators.Wheels.SetVelocities(0F, 35F);
                mStopped = false;
                mPreviousLeft = false;
            }
            return true;
        }


        private int ComputeScore()
        {

            float lMax = mThermalSensorDataArray15.Max();
            float lMin = mThermalSensorDataArray15.Min();

            // Render the heat data on 15*15 Texture - (shade of blue / red)
            for (int i = 0; i < mThermalSensorDataArray15.Length; i++) {
                // Calcul ratio of : TemperatureRelativeToTheRange / RangeTemperature
                float lNormalizedTemp = (mThermalSensorDataArray15[i] - lMin) / (lMax - lMin);
                // This ratio is used to calcul each shade
                mLiTexture.SetPixel(i % 15, 14 - i / 15, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
            }

            // If the gap between min & max is lower than mTempMax (Choosen constant) don't perform check and stop the algo
            if (lMax - mAverageLow < float.Parse(mTempMax.text) || lMax - lMin < float.Parse(mTempMax.text)) {
                Score.text = "No human";
                return -1;
            } else {
                int lScore = 0;
                int lIndexMax = mThermalSensorDataArray15.ToList().IndexOf(lMax);

                // give more importance to verticality
                for (int i = 1; i < 15; i++) {

                    //Try pixel below
                    int lTestIndex = lIndexMax - i * 15;
                    if (lTestIndex >= 0)
                        if (mThermalSensorDataArray15[lTestIndex] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                            mLiTexture.SetPixel(lTestIndex % 15, 14 - lTestIndex / 15, new Color(0F, 1F, 0F, 1F));
                            lScore++;

                            // Try neighbours
                            if ((lTestIndex) % 15 > 0)
                                if (mThermalSensorDataArray15[lTestIndex - 1] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                                    mLiTexture.SetPixel((lTestIndex - 1) % 15, 14 - (lTestIndex - 1) / 15, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                            if (lTestIndex % 15 < 14)
                                if (mThermalSensorDataArray15[lTestIndex + 1] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                                    mLiTexture.SetPixel((lTestIndex + 1) % 15, 14 - (lTestIndex + 1) / 15, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                        } else
                            break;
                }

                for (int i = 14; i >= 0; i--) {
                    // Try pixel above
                    int lTestIndex = lIndexMax + i * 15;
                    if (lTestIndex < 225)
                        if (mThermalSensorDataArray15[lTestIndex] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                            mLiTexture.SetPixel(lTestIndex % 15, 14 - lTestIndex / 15, new Color(0F, 1F, 0F, 1F));
                            lScore++;

                            // Try neighbours
                            if (lTestIndex % 15 > 0)
                                if (mThermalSensorDataArray15[lTestIndex - 1] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                                    mLiTexture.SetPixel((lTestIndex - 1) % 15, 14 - (lTestIndex - 1) / 15, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                            if (lTestIndex % 15 < 14)
                                if (mThermalSensorDataArray15[lTestIndex + 1] - mAverageLow > float.Parse(mTempNeighbours.text)) {
                                    mLiTexture.SetPixel((lTestIndex + 1) % 15, 14 - (lTestIndex + 1) / 15, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                        } else
                            break;
                }

                // Score good enough, stop here
                if (lScore > int.Parse(mScoreThresh.text)) {
                    mScore = lScore;
                    return lIndexMax;
                } else {

                    // Try another point
                    mThermalSensorDataArray15[lIndexMax] = mThermalSensorDataArray15.Min();
                    return ComputeScore();

                }
            }
        }

        // Check bad calcul on interpolate box is gone (the short cast has been delete)
        private bool LinearInterpolation()
        {
            const byte c_ucImgWidth = 15;
            const byte c_ucImgHeight = 15;
            bool bRet = false;

            //Debug.Log("lol");
            byte ucX = 0;
            byte ucY = 0;
            for (ucY = 0; ucY < c_ucImgHeight; ucY += 2) {
                //STEP1
                for (ucX = 0; ucX < c_ucImgWidth; ucX += 2) {
                    // calcul of 15*15 index
                    byte ucSnr = (byte)(ucX / 2 + ucY / 2 * 8);
                    // calcul of 15*15 index
                    byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                    mThermalSensorDataArray15[ucImg] = mThermalSensorDataArray[ucSnr];
                    //Debug.Log("ashmachin: " + this.g_ashSnrAveTemp[ucSnr]);
                }
                //STEP2
                for (ucX = 1; ucX < c_ucImgWidth; ucX += 2) {
                    byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                    mThermalSensorDataArray15[ucImg] = ((mThermalSensorDataArray15[ucImg - 1] + mThermalSensorDataArray15[ucImg + 1]) / 2);
                }
            }
            for (ucY = 1; ucY < c_ucImgHeight; ucY += 2) {
                for (ucX = 0; ucX < c_ucImgWidth; ucX++) {
                    byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                    mThermalSensorDataArray15[ucImg] = ((mThermalSensorDataArray15[ucImg - c_ucImgWidth] + mThermalSensorDataArray15[ucImg + c_ucImgWidth]) / 2);

                }
            }
            bRet = true;
            return (bRet);
        }

    }
}



//private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
//{
//    Debug.LogWarning("new thermal frame");

//    iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);

//    // create a byte array and copy the floats into it...
//    var lByteArray = new byte[mThermalSensorDataArray.Length * 4];
//    Buffer.BlockCopy(mThermalSensorDataArray, 0, lByteArray, 0, lByteArray.Length);

//    Debug.LogWarning("add new data");
//    mThermalImageData.AddNewData(lByteArray);

//    mThermalMat = iThermalFrame.Mat.clone();

//    if (mNbFrame >= 8) {
//        Debug.LogWarning("8 thermal frame");
//        if (mNbFrame == 8)
//            mBackGroundImage.initializeBackgroundImage(mThermalImageData);
//        CheckHumanPresence();
//    } else
//        mNbFrame++;
//}

//private bool CheckHumanPresence()
//{
//    int lNbDetect = mPeopleDetector.detectPeople(mThermalImageData, mBackGroundImage);



//    // Resets the center of heats array

//    Debug.Log("************** Values before ************** " + lNbDetect);

//    for (int i = 0; i < 64; i += 8)
//        Debug.Log("" + mThermalSensorDataArray[i] + " "
//           + mThermalSensorDataArray[i + 1] + " "
//           + mThermalSensorDataArray[i + 2] +  " "
//           + mThermalSensorDataArray[i + 3] + " "
//           + mThermalSensorDataArray[i + 4] + " "
//           + mThermalSensorDataArray[i + 5] + " "
//           + mThermalSensorDataArray[i + 6] + " "
//           + mThermalSensorDataArray[i + 7] + " ");


//    for (int i = 0; i < lNbDetect; i++) {
//        mThermalMat.put(mPeopleDetector.astrCenterOfHeat[i].ashCOH[0], mPeopleDetector.astrCenterOfHeat[i].ashCOH[1], 300F);
//    }




//    Debug.Log("************** Values after ************** " + lNbDetect);


//    mThermalMat.get(0, 0, mThermalSensorDataArray);

//    for (int i = 0; i < 64; i+=8)
//        Debug.Log("" + mThermalSensorDataArray[i] + " " +
//            mThermalSensorDataArray[i+1] + " "
//           + mThermalSensorDataArray[i+2] + " "
//           + mThermalSensorDataArray[i+3] + " "
//           + mThermalSensorDataArray[i+4] + " "
//           + mThermalSensorDataArray[i+5] + " "
//           + mThermalSensorDataArray[i+6] + " "
//           + mThermalSensorDataArray[i+7] + " ");

//    Texture2D lTexture = Utils.MatToTexture2D(mThermalMat);

//    Sprite lSprite = Sprite.Create(lTexture, new UnityEngine.Rect(0.0f, 0.0f, lTexture.width, lTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

//    if (!Buddy.GUI.Toaster.IsBusy)
//        Buddy.GUI.Toaster.Display<PictureToast>().With(lSprite);


//    mBackGroundImage.upgradeBackgroundTemperatures(mPeopleDetector);

//    return false;
//}
