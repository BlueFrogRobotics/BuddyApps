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
    public class TestGridEyeBehaviour : MonoBehaviour
    {

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestGridEyeData mAppData;

        //private GE_Image_Data mThermalImageData;
        //private GE_Background_Image mBackGroundImage;
        //private GE_People_Detector mPeopleDetector;
        private Sprite mSprite;
        private int mNbFrame;
        //private Mat mThermalMat;
        private float[] mThermalSensorDataArray;
        private float[] mThermalSensorDataArrayCandidate;
        private Texture2D mTexture;

        [SerializeField]
        private Text Score;

        [SerializeField]
        private Text Position;
        
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
        }

        void Start()
        {
            TestGridEyeActivity.Init(null);

            mDisplay = true;
            mMotion = false;
            mNbFrame = 0;
            mScore = -1;
            //mThermalImageData = new GE_Image_Data();
            //mBackGroundImage = new GE_Background_Image();
            //mPeopleDetector = new GE_People_Detector();
            //mThermalMat = new Mat();
            mTexture = new Texture2D(8, 8) {

                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };


            mThermalSensorDataArray = new float[64];
            mThermalSensorDataArrayCandidate = new float[64];

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);

            mAverageThermal = new CircularBuffer<float>(8);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            mNbFrame++;
            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);
            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArrayCandidate);

            mAverageThermal.Enqueue(mThermalSensorDataArray.Min());

            //mAverageLow = ((mNbFrame - 1) * mAverageLow + mThermalSensorDataArray.Min()) / mNbFrame;

            //mAverageLow = mAverageThermal.Average();


            //Debug.LogWarning("AverageLow " + mAverageLow);
            //Debug.LogWarning(" Max: " + lMax);
            //Debug.LogWarning(" Min: " + mThermalSensorDataArray.Min());
            

            int lIndexMax = ComputeScore();

            // if human
            if (lIndexMax > -1) {

                // Set max temp in black
                mTexture.SetPixel(lIndexMax % 8, 7 - (lIndexMax / 8), new Color(1F, 1F, 1F, 1F));

                // TODO: center of human is at center of shape
                //Debug.LogWarning("Score human: " + mScore);
                //Debug.LogWarning("position human: x " + lIndexMax % 8 + " y " + lIndexMax % 8 + " i " + lIndexMax);

                Score.text = "Score: " + mScore;
                Position.text = "Position: " + lIndexMax % 8 + ":" + lIndexMax / 8;

                if (mMotion || mFollow)
                    OnHumanDetect(lIndexMax % 8);

            } else {

                // Remove the green pixels
                float lMax = mThermalSensorDataArray.Max();
                float lMin = mThermalSensorDataArray.Min();
                for (int i = 0; i < mThermalSensorDataArray.Length; i++) {
                    //lTexture.SetPixel(i%8, i/8, new Color(mThermalSensorDataArray[i]*5, 100F, 255F - (mThermalSensorDataArray[i] * 5), 1F));

                    float lNormalizedTemp = (mThermalSensorDataArray[i] - lMin) / (lMax - lMin);

                    mTexture.SetPixel(i % 8, 7 - i / 8, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
                }

            }

            mTexture.Apply();

            mSprite = Sprite.Create(mTexture, new UnityEngine.Rect(0.0f, 0.0f, 8, 8), new Vector2(0.5f, 0.5f), 1.0F);

            if (!Buddy.GUI.Toaster.IsBusy && mDisplay)
                Buddy.GUI.Toaster.Display<PictureToast>().With(mSprite,
                    () => {
                        Buddy.GUI.Toaster.Hide();
                        mDisplay = false;
                    });

        }

        private bool OnHumanDetect(int iRow)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            mTimeHumanDetected = Time.time;
            float lCentered = iRow / 7F;
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

            float lMax = mThermalSensorDataArray.Max();
            float lCurrentCandidateTemp = mThermalSensorDataArrayCandidate.Max();
            float lMin = mThermalSensorDataArray.Min();

            for (int i = 0; i < mThermalSensorDataArray.Length; i++) {
                //lTexture.SetPixel(i%8, i/8, new Color(mThermalSensorDataArray[i]*5, 100F, 255F - (mThermalSensorDataArray[i] * 5), 1F));

                float lNormalizedTemp = (mThermalSensorDataArray[i] - lMin) / (lMax - lMin);

                mTexture.SetPixel(i % 8, 7 - i / 8, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
            }




            if (lCurrentCandidateTemp - lMin < float.Parse(mTempMax.text)) {
                //Debug.LogWarning("No human");
                Score.text = "No human";
                return -1;
            } else {
                int lScore = 0;
                int lIndexMax = mThermalSensorDataArrayCandidate.ToList().IndexOf(lCurrentCandidateTemp);


                // give more importance to verticality
                for (int i = 1; i < 8; i++) {

                    //Try pixel below
                    int lTestIndex = lIndexMax - i * 8;
                    if (lTestIndex >= 0)
                        if (mThermalSensorDataArray[lTestIndex] - lMin > float.Parse(mTempNeighbours.text)) {
                            mTexture.SetPixel(lTestIndex % 8, 7 - lTestIndex / 8, new Color(0F, 1F, 0F, 1F));
                            lScore++;

                            // Try neighbours
                            if ((lTestIndex) % 8 > 0)
                                if (mThermalSensorDataArray[lTestIndex - 1] - lMin > float.Parse(mTempNeighbours.text)) {
                                    mTexture.SetPixel((lTestIndex - 1) % 8, 7 - lTestIndex / 8, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                            if (lTestIndex % 8 < 7)
                                if (mThermalSensorDataArray[lTestIndex + 1] - lMin > float.Parse(mTempNeighbours.text)) {
                                    mTexture.SetPixel((lTestIndex + 1) % 8, 7 - lTestIndex / 8, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                        } else
                            break;
                }

                for (int i = 7; i >= 0; i--) {
                    // Try pixel above
                    int lTestIndex = lIndexMax + i * 8;
                    if (lTestIndex < 63)
                        if (mThermalSensorDataArray[lTestIndex] - lMin > float.Parse(mTempNeighbours.text)) {
                            mTexture.SetPixel(lTestIndex % 8, 7 - lTestIndex / 8, new Color(0F, 1F, 0F, 1F));
                            lScore++;

                            // Try neighbours
                            if (lTestIndex % 8 > 0)
                                if (mThermalSensorDataArray[lTestIndex - 1] - lMin > float.Parse(mTempNeighbours.text)) {
                                    mTexture.SetPixel((lTestIndex - 1) % 8, 7 - lTestIndex / 8, new Color(0F, 1F, 0F, 1F));
                                    lScore++;
                                }
                            if (lTestIndex % 8 < 7)
                                if (mThermalSensorDataArray[lTestIndex + 1] - lMin > float.Parse(mTempNeighbours.text)) {
                                    mTexture.SetPixel((lTestIndex + 1) % 8, 7 - lTestIndex  / 8, new Color(0F, 1F, 0F, 1F));
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
                    mThermalSensorDataArrayCandidate[lIndexMax] = mThermalSensorDataArray.Min();
                    return ComputeScore();

                }
            }
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
