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
    public class TestGridEyeBehaviourV3 : MonoBehaviour
    {

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestGridEyeData mAppData;
        
        private Sprite mSprite;
        private int mNbFrame;
        private float[] mThermalSensorDataAverageArray;
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

        CircularBuffer<float[]> mAverageThermal;
        private int mScore;

        private bool mDisplay;
        private bool mMotion;
        private bool mFollow;
        private float mTimeHumanDetected;
        private bool mPreviousLeft;
        private bool mStopped;
        private float mMax;
        private float mMin;
        private int mMeanX;
        private int mMeanY;
        private int mPreviousIndexMax;

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
            mPreviousIndexMax = -1;
            mTexture = new Texture2D(8, 8) {
                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };


            mThermalSensorDataAverageArray = new float[64];
            mThermalSensorDataArrayCandidate = new float[64];

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);

            mAverageThermal = new CircularBuffer<float[]>(3);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            mNbFrame++;


            float[] lThermalSensorDataArray;
            lThermalSensorDataArray = new float[64];

            //Reset average data
            Array.Clear(mThermalSensorDataAverageArray, 0, mThermalSensorDataAverageArray.Length);
            Array.Clear(mThermalSensorDataArrayCandidate, 0, mThermalSensorDataArrayCandidate.Length);

            iThermalFrame.Mat.get(0, 0, lThermalSensorDataArray);
            mAverageThermal.Enqueue(lThermalSensorDataArray);

            for (int i = 0; i < lThermalSensorDataArray.Length; ++i)
                for (int j = 0; j < mAverageThermal.Count; ++j) {
                    mThermalSensorDataAverageArray[i] += mAverageThermal[j][i] / mAverageThermal.Count;
                    mThermalSensorDataArrayCandidate[i] += mAverageThermal[j][i] / mAverageThermal.Count;
                }


            mMax = mThermalSensorDataAverageArray.Max();
            mMin = mThermalSensorDataAverageArray.Min();
            Debug.LogWarning("min max: " + mMin + " " + mMax);

            //RemoveWrongCorners();


            int lIndexMax = ComputeScore(true);

            // if human
            if (lIndexMax > -1) {

                mPreviousIndexMax = lIndexMax;

                // Set max temp in black
                mTexture.SetPixel(lIndexMax % mTexture.width, (mTexture.height - 1) - (lIndexMax / mTexture.width), new Color(1F, 1F, 1F, 1F));
                mTexture.SetPixel(mMeanX / mScore, (mTexture.height - 1) - (mMeanY / mScore), new Color(0F, 0F, 0F, 1F));
                
                Score.text = "Score: " + mScore;
                Position.text = "Position: " + lIndexMax % mTexture.width + ":" + lIndexMax / mTexture.width;

                if (mMotion || mFollow)
                    OnHumanDetect(((float)mMeanX / mScore) / (mTexture.width - 1));

            } else {
                mPreviousIndexMax = -1;
                // if no human
                // Remove the green pixels
                for (int i = 0; i < mThermalSensorDataAverageArray.Length; i++) {
                    //lTexture.SetPixel(i%mTexture.width, i/mTexture.width, new Color(mThermalSensorDataArray[i]*5, 100F, 255F - (mThermalSensorDataArray[i] * 5), 1F));

                    float lNormalizedTemp = (mThermalSensorDataAverageArray[i] - mMin) / (mMax - mMin);

                    mTexture.SetPixel(i % mTexture.width, 7 - i / mTexture.width, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
                }

            }

            mTexture.Apply();

            mSprite = Sprite.Create(mTexture, new UnityEngine.Rect(0.0f, 0.0f, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f), 1.0F);

            if (!Buddy.GUI.Toaster.IsBusy && mDisplay)
                Buddy.GUI.Toaster.Display<PictureToast>().With(mSprite,
                    () => {
                        Buddy.GUI.Toaster.Hide();
                        mDisplay = false;
                    });

        }

        //private void RemoveWrongCorners()
        //{  
        //    // Remove corners because too hot
        //    //TOP LEFT
        //    mThermalSensorDataArrayCandidate[0] = mMin;
        //    mThermalSensorDataArrayCandidate[1] = mMin;
        //    mThermalSensorDataArrayCandidate[2] = mMin;
        //    mThermalSensorDataArrayCandidate[3] = mMin;
        //    mThermalSensorDataArrayCandidate[8] = mMin;
        //    mThermalSensorDataArrayCandidate[9] = mMin;

        //    //TOP RIGHT
        //    mThermalSensorDataArrayCandidate[4] = mMin;
        //    mThermalSensorDataArrayCandidate[5] = mMin;
        //    mThermalSensorDataArrayCandidate[6] = mMin;
        //    mThermalSensorDataArrayCandidate[7] = mMin;
        //    mThermalSensorDataArrayCandidate[14] = mMin;
        //    mThermalSensorDataArrayCandidate[15] = mMin;

        //    //BOTTOM LEFT
        //    mThermalSensorDataArrayCandidate[48] = mMin;
        //    mThermalSensorDataArrayCandidate[49] = mMin;
        //    mThermalSensorDataArrayCandidate[56] = mMin;
        //    mThermalSensorDataArrayCandidate[57] = mMin;
        //    mThermalSensorDataArrayCandidate[58] = mMin;
        //    mThermalSensorDataArrayCandidate[59] = mMin;

        //    //BOTTOM RIGHT
        //    mThermalSensorDataArrayCandidate[54] = mMin;
        //    mThermalSensorDataArrayCandidate[55] = mMin;
        //    mThermalSensorDataArrayCandidate[60] = mMin;
        //    mThermalSensorDataArrayCandidate[61] = mMin;
        //    mThermalSensorDataArrayCandidate[62] = mMin;
        //    mThermalSensorDataArrayCandidate[63] = mMin;

        //    // Remove corners because too hot
        //    //TOP LEFT
        //    mThermalSensorDataAverageArray[0] = mMin;
        //    mThermalSensorDataAverageArray[1] = mMin;
        //    mThermalSensorDataAverageArray[2] = mMin;
        //    mThermalSensorDataAverageArray[3] = mMin;
        //    mThermalSensorDataAverageArray[8] = mMin;
        //    mThermalSensorDataAverageArray[9] = mMin;

        //    //TOP RIGHT
        //    mThermalSensorDataAverageArray[4] = mMin;
        //    mThermalSensorDataAverageArray[5] = mMin;
        //    mThermalSensorDataAverageArray[6] = mMin;
        //    mThermalSensorDataAverageArray[7] = mMin;
        //    mThermalSensorDataAverageArray[14] = mMin;
        //    mThermalSensorDataAverageArray[15] = mMin;

        //    //BOTTOM LEFT
        //    mThermalSensorDataAverageArray[48] = mMin;
        //    mThermalSensorDataAverageArray[49] = mMin;
        //    mThermalSensorDataAverageArray[56] = mMin;
        //    mThermalSensorDataAverageArray[57] = mMin;
        //    mThermalSensorDataAverageArray[58] = mMin;
        //    mThermalSensorDataAverageArray[59] = mMin;

        //    //BOTTOM RIGHT
        //    mThermalSensorDataAverageArray[54] = mMin;
        //    mThermalSensorDataAverageArray[55] = mMin;
        //    mThermalSensorDataAverageArray[60] = mMin;
        //    mThermalSensorDataAverageArray[61] = mMin;
        //    mThermalSensorDataAverageArray[62] = mMin;
        //    mThermalSensorDataAverageArray[63] = mMin;
        //}

        private bool OnHumanDetect(float iCentered)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            mTimeHumanDetected = Time.time;
            //float lCentered = ((float) iRow) / (mTexture.width - 1);
            if (iCentered < 0.6F && iCentered > 0.4F) {
                Debug.LogWarning("Stop " + mStopped);
                mStopped = true;
                Buddy.Actuators.Wheels.Stop();

                return false;

                // otherwise, try to put the human in the center
            } else if (iCentered < 0.6F && (!mPreviousLeft || mStopped /*!Buddy.Actuators.Wheels.IsBusy*/)) {
                Debug.LogWarning("Go to left " + mStopped);
                Buddy.Actuators.Wheels.SetVelocities(0F, -65F * iCentered);
                mStopped = false;
                mPreviousLeft = true;
            } else if (iCentered > 0.4F && (mPreviousLeft || mStopped /*|| !Buddy.Actuators.Wheels.IsBusy*/)) {
                Debug.LogWarning("Go to right " + mStopped);
                Buddy.Actuators.Wheels.SetVelocities(0F, 65F * (1 - iCentered));
                mStopped = false;
                mPreviousLeft = false;
            }

            return true;
        }

        // TODO: make generic
        private int GetNextMiddleCandidate()
        {
            for (int i = 0; i < mThermalSensorDataArrayCandidate.Length; i++) {
                // if we are on the side, go to center

                float lCentered = ((float)(i % mTexture.width)) / (mTexture.width - 1);

                if (lCentered < 0.65F && lCentered > 0.35F) {

                    // TODO: why is it going through this???
                    if (i < mThermalSensorDataArrayCandidate.Length)
                        if (mThermalSensorDataArrayCandidate[i] - mMin > float.Parse(mTempMax.text)) {
                            // if temp high enough, return this candidate
                            return i;
                        }
                } else
                    continue;
            }
            return -1;
        }

        private int ComputeScore(bool iMiddleCandidate)
        {
            float lCurrentCandidateTemp;
            int lIndexMax = -1;

            if (mThermalSensorDataArrayCandidate.Min() > -50F)
                // This is first try
                lIndexMax = mPreviousIndexMax;

            else if (iMiddleCandidate)
                lIndexMax = GetNextMiddleCandidate();

            // if we have middle candidate or previous
            if (lIndexMax != -1) {
                lCurrentCandidateTemp = mThermalSensorDataArrayCandidate[lIndexMax];
            }

            // If no candidate in center, Just get the next hotter pixel
            else {
                iMiddleCandidate = false;
                lCurrentCandidateTemp = mThermalSensorDataArrayCandidate.Max();
                lIndexMax = mThermalSensorDataArrayCandidate.ToList().IndexOf(lCurrentCandidateTemp);
            }


            // Is the current candidate ok?
            if (lCurrentCandidateTemp - mMin < float.Parse(mTempMax.text)) {
                Score.text = "No human";
                return -1;


                // Check the score of the current candidate
            } else {
                // Draw the current texture
                for (int i = 0; i < mThermalSensorDataAverageArray.Length; i++) {

                    float lNormalizedTemp = (mThermalSensorDataAverageArray[i] - mMin) / (mMax - mMin);

                    // "7 -" to turn the image upside down
                    mTexture.SetPixel(i % mTexture.width, (mTexture.height - 1) - i / mTexture.width, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
                }



                mScore = 1;
                mMeanX = lIndexMax % mTexture.width;
                mMeanY = lIndexMax / mTexture.width;
                // give more importance to verticality
                for (int i = 0; i < mTexture.height; i++) {
                    //Try pixel below
                    int lTestIndex = lIndexMax - i * mTexture.width;
                    if (lTestIndex >= 0) {
                        int PixelScore = TryPixel(lTestIndex);
                        if (PixelScore == 0)
                            break;
                        else
                            mScore += PixelScore;
                    }
                }

                // Remove i = 0 to avoid taking into account twice the max pixel
                for (int i = 1; i < mTexture.height; i++) {
                    // Try pixel above
                    int lTestIndex = lIndexMax + i * mTexture.width;
                    if (lTestIndex < mTexture.height * mTexture.width) {
                        int PixelScore = TryPixel(lTestIndex);
                        if (PixelScore == 0)
                            break;
                        else
                            mScore += PixelScore;
                    }
                }

                // Score good enough, stop here
                if (mScore > int.Parse(mScoreThresh.text)) {
                    return lIndexMax;
                } else {
                    Debug.LogWarning("Score not enough:" + mScore);
                    // Try another point
                    mThermalSensorDataArrayCandidate[lIndexMax] = -100F;
                    return ComputeScore(iMiddleCandidate);

                }
            }
        }

        private int TryPixel(int iTestIndex)
        {
            int lScore = 0;
            if (mThermalSensorDataAverageArray[iTestIndex] - mMin > float.Parse(mTempNeighbours.text)) {
                mTexture.SetPixel(iTestIndex % mTexture.width, (mTexture.height - 1) - iTestIndex / mTexture.width, new Color(0F, 1F, 0F, 1F));

                // this pixel cannot be a candidate
                mThermalSensorDataArrayCandidate[iTestIndex] = -100F;

                // Update Barycenter
                UpdateBarycenter(iTestIndex);

                lScore++;

                // Try neighbours


                // previous neighbour is not in previous colon?
                if (iTestIndex % mTexture.width > 0)
                    if (mThermalSensorDataAverageArray[iTestIndex - 1] - mMin > float.Parse(mTempNeighbours.text)) {

                        //Update texture
                        mTexture.SetPixel((iTestIndex - 1) % mTexture.width, (mTexture.height - 1) - iTestIndex / mTexture.width, new Color(0F, 1F, 0F, 1F));

                        // Update Barycenter
                        UpdateBarycenter(iTestIndex - 1);

                        lScore++;
                    }

                // next neighbour is not in next colon?
                if (iTestIndex % mTexture.width < (mTexture.width - 1))
                    if (mThermalSensorDataAverageArray[iTestIndex + 1] - mMin > float.Parse(mTempNeighbours.text)) {

                        //Update texture
                        mTexture.SetPixel((iTestIndex + 1) % mTexture.width, (mTexture.height - 1) - iTestIndex / mTexture.width, new Color(0F, 1F, 0F, 1F));

                        // Update Barycenter
                        UpdateBarycenter(iTestIndex + 1);

                        lScore++;
                    }

                return lScore;
            } else
                return 0;
        }


        // Update Barycenter
        private void UpdateBarycenter(int iIndex)
        {
            mMeanX += (iIndex % mTexture.width);
            mMeanY += (iIndex / mTexture.width);
        }

    }
}