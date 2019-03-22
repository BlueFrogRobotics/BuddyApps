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
    public class TestGridEyeBehaviourV4 : MonoBehaviour
    {

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

        private bool mDisplay;
        private bool mMotion;
        private bool mFollow;
        private bool mPreviousLeft;
        private bool mStopped;
        private int mMeanX;
        private int mMeanY;
        private int mPreviousIndexMax;

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestGridEyeData mAppData;

        private Sprite mSprite;
        private Texture2D mTexture;
        private List<int> mHumanCurrentPose;
        private float[] mThermalSensorDataArrayCandidate;
        private float[] mThermalSensorDataMinArray;
        private float[] mThermalSensorDataArray;

        private CircularBuffer<float[]> mMinThermal;
        private int mNbFrame;
        private bool mHumanPresent;
        private List<int> mRemovedCandidates;

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
            mHumanPresent = false;
            mPreviousIndexMax = -1;
            mTexture = new Texture2D(8, 8) {

                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };
            mNbFrame = 0;
            mRemovedCandidates = new List<int>();
            mHumanCurrentPose = new List<int>();
            mThermalSensorDataMinArray = new float[64];
            mThermalSensorDataArrayCandidate = new float[64];
            mThermalSensorDataArray = new float[64];

            mMinThermal = new CircularBuffer<float[]>(50);

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            mNbFrame++;
            mRemovedCandidates.Clear();

            // Init ring buffers
            if (mNbFrame == 1) {
                iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);
                //mAverageThermal.Enqueue(lThermalSensorDataArray);

                float[] lCopy = new float[mThermalSensorDataArray.Length];
                Array.Copy(mThermalSensorDataArray, lCopy, mThermalSensorDataArray.Length);
                mMinThermal.Enqueue(lCopy);
                return;
            }

            //Reset data
            Array.Clear(mThermalSensorDataMinArray, 0, mThermalSensorDataMinArray.Length);


            //Compute min for each pixel:
            for (int i = 0; i < mThermalSensorDataMinArray.Length; ++i)
                for (int j = 0; j < mMinThermal.Count; ++j) {
                    if ((mThermalSensorDataMinArray[i] > mMinThermal[j][i] || mThermalSensorDataMinArray[i] < 1F) && (mMinThermal[j][i] > 1F)) {
                        mThermalSensorDataMinArray[i] = mMinThermal[j][i];
                    }
                }


            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);
            //iThermalFrame.Mat.get(0, 0, mThermalSensorDataArrayCandidate);




            // Get diff as candidates:
            for (int i = 0; i < mThermalSensorDataArrayCandidate.Length; ++i)
                mThermalSensorDataArrayCandidate[i] = mThermalSensorDataArray[i] - mThermalSensorDataMinArray[i];


            // If every pixel is far from min, we need to recalibrate:
            //if (mThermalSensorDataArrayCandidate.Min() > 1F && !mHumanPresent) {
            //    Debug.LogWarning("NEED TO RECALIBRATE BECAUSE MIN AT: " + mThermalSensorDataArrayCandidate.Min());
            //    mNbFrame = 0;
            //    mMinThermal.Clear();
            //    float[] lCopy = new float[mThermalSensorDataArray.Length];
            //    Array.Copy(mThermalSensorDataArray, lCopy, mThermalSensorDataArray.Length);
            //    mMinThermal.Enqueue(lCopy);
            //    return;
            //}


            //PrintArray(mThermalSensorDataMinArray, "**************Min array!*************");
            //PrintArray(mThermalSensorDataArrayCandidate, "**************Candidate array!*************");


            // Draw the current texture
            for (int i = 0; i < mThermalSensorDataArray.Length; i++) {

                float lNormalizedTemp = (mThermalSensorDataArrayCandidate[i] - mThermalSensorDataArrayCandidate.Min() - 0) / (10 - 0);
                //float lNormalizedTemp = (lThermalSensorDataArray[i] - lMin) / (lMax - lMin);
                // "(mTexture.height - 1) -" to turn the image upside down
                mTexture.SetPixel(i % mTexture.width, (mTexture.height - 1) - i / mTexture.width, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
            }

            // Try to find the human
            int lIndexMax = ComputeScore(true);

            // if human
            if (lIndexMax > -1) {

                // Update position of human
                mPreviousIndexMax = lIndexMax;

                //Draw human Position:
                DrawHuman(lIndexMax);

                Score.text = "Score: " + mHumanCurrentPose.Count;
                Position.text = "Position: " + lIndexMax % mTexture.width + ":" + lIndexMax / mTexture.width;

                // Follow if activated
                if (mMotion || mFollow)
                    OnHumanDetect(((float)mMeanX / mHumanCurrentPose.Count) / (mTexture.width - 1));



                mHumanPresent = true;

            } else {
                mHumanPresent = false;
                mPreviousIndexMax = -1;
                mHumanCurrentPose.Clear();

                // Add last array to min if no human
                float[] lCopy = new float[mThermalSensorDataArray.Length];
                Array.Copy(mThermalSensorDataArray, lCopy, mThermalSensorDataArray.Length);
                mMinThermal.Enqueue(lCopy);
            }

            // Update texture to render
            mTexture.Apply();

            mSprite = Sprite.Create(mTexture, new UnityEngine.Rect(0.0f, 0.0f, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f), 1.0F);

            if (!Buddy.GUI.Toaster.IsBusy && mDisplay)
                Buddy.GUI.Toaster.Display<PictureToast>().With(mSprite,
                    () => {
                        Buddy.GUI.Toaster.Hide();
                        mDisplay = false;
                    });

        }

        private void DrawHuman(int iIndexMax)
        {
            for (int i = 0; i < mHumanCurrentPose.Count(); ++i)
                mTexture.SetPixel(mHumanCurrentPose[i] % mTexture.width, (mTexture.height - 1) - mHumanCurrentPose[i] / mTexture.width, new Color(0F, 1F, 0F, 1F));


            // Set max temp in white
            mTexture.SetPixel(iIndexMax % mTexture.width, (mTexture.height - 1) - (iIndexMax / mTexture.width), new Color(1F, 1F, 1F, 1F));
            mHumanCurrentPose.Add(iIndexMax);

            // Set Barycenter in black
            mTexture.SetPixel((mMeanX + 1) / mHumanCurrentPose.Count, (mTexture.height - 1) - ((mMeanY + 1) / mHumanCurrentPose.Count), new Color(0F, 0F, 0F, 1F));
        }

        private bool OnHumanDetect(float iCentered)
        {
            Buddy.Behaviour.SetMood(Mood.HAPPY);
            // mTimeHumanDetected = Time.time;
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


        private int ComputeScore(bool iMiddleCandidate)
        {
            float lCurrentCandidateTemp;
            int lIndexCurrentCandidate = -1;
            mMeanX = 0;
            mMeanY = 0;

            mHumanCurrentPose.Clear();

            if (mRemovedCandidates.Count == 0)
                // This is first try
                lIndexCurrentCandidate = mPreviousIndexMax;

            else if (iMiddleCandidate)
                lIndexCurrentCandidate = GetNextMiddleCandidate();

            // if we have middle candidate or previous
            if (lIndexCurrentCandidate != -1) {
                lCurrentCandidateTemp = mThermalSensorDataArrayCandidate[lIndexCurrentCandidate];
            }

            // If no candidate in center, Just get the next hotter pixel
            else {
                iMiddleCandidate = false;
                lIndexCurrentCandidate = NextMaxCandidate();

                //No more candidate
                if (lIndexCurrentCandidate == -1)
                    return -1;

                lCurrentCandidateTemp = mThermalSensorDataArrayCandidate[lIndexCurrentCandidate];
            }


            // Is the current candidate ok?
            if (lCurrentCandidateTemp - mThermalSensorDataArrayCandidate.Min() < float.Parse(mTempMax.text)) {

                Debug.LogWarning("No human because temp at " + lIndexCurrentCandidate + " with temp " + lCurrentCandidateTemp);
                Score.text = "No human";
                return -1;


                // Check the score of the current candidate
            } else {
                // give more importance to verticality
                for (int i = 0; i < mTexture.height; i++) {
                    //Try pixel below
                    int lTestIndex = lIndexCurrentCandidate - i * mTexture.width;
                    if (lTestIndex >= 0) {
                        if (!TryPixel(lTestIndex, lCurrentCandidateTemp))
                            break;
                    }
                }

                // Remove i = 0 to avoid taking into account twice the max pixel
                for (int i = 1; i < mTexture.height; i++) {
                    // Try pixel above
                    int lTestIndex = lIndexCurrentCandidate + i * mTexture.width;
                    if (lTestIndex < mTexture.height * mTexture.width) {
                        if (!TryPixel(lTestIndex, lCurrentCandidateTemp))
                            break;
                    }
                }

                // Score good enough, stop here
                if (mHumanCurrentPose.Count > int.Parse(mScoreThresh.text)) {
                    Debug.LogWarning("Got a good score: " + mHumanCurrentPose.Count);
                    return lIndexCurrentCandidate;
                } else {
                    // Try another point
                    Debug.LogWarning("Got a bad score: " + mHumanCurrentPose.Count);
                    mRemovedCandidates.Add(lIndexCurrentCandidate);
                    return ComputeScore(iMiddleCandidate);

                }
            }
        }

        private int NextMaxCandidate()
        {
            float lMax = 0F;
            int lIndexMax = -1;
            for (int i = 0; i < mThermalSensorDataArrayCandidate.Length; ++i) {
                //if removed candidate, go to next one
                if (mRemovedCandidates.Contains(i))
                    continue;
                else {
                    if (lMax < mThermalSensorDataArrayCandidate[i]) {
                        lMax = mThermalSensorDataArrayCandidate[i];
                        lIndexMax = i;
                    }
                }
            }

            return lIndexMax;

        }

        private int GetNextMiddleCandidate()
        {
            for (int i = 0; i < mThermalSensorDataArrayCandidate.Length; i++) {
                //if removed candidate, go to next one
                if (mRemovedCandidates.Contains(i))
                    continue;

                // if we are on the side, go to center
                float lCentered = ((float)(i % mTexture.width)) / (mTexture.width - 1);

                if (lCentered < 0.65F && lCentered > 0.35F) {

                    // TODO: why is it going through this???
                    if (i < mThermalSensorDataArrayCandidate.Length)
                        if (mThermalSensorDataArrayCandidate[i] - mThermalSensorDataArrayCandidate.Min() > float.Parse(mTempMax.text)) {
                            // if temp high enough, return this candidate
                            Debug.LogWarning("Trying position " + i + " with temp " + mThermalSensorDataArrayCandidate[i] + " and temp diff " + (mThermalSensorDataArrayCandidate[i] - mThermalSensorDataArrayCandidate.Min()));
                            return i;
                        }
                } else
                    continue;
            }
            return -1;
        }

        private bool TryPixel(int iTestIndex, float iCurrentCandidateTemp)
        {
            if (mThermalSensorDataArrayCandidate[iTestIndex] - mThermalSensorDataArrayCandidate.Min() > iCurrentCandidateTemp * 0.85) {
                mHumanCurrentPose.Add(iTestIndex);

                // this pixel cannot be a candidate
                mRemovedCandidates.Add(iTestIndex);

                // Try neighbours
                // previous neighbour is not in previous colon?
                if (iTestIndex % mTexture.width > 0)
                    if (mThermalSensorDataArrayCandidate[iTestIndex - 1] - mThermalSensorDataArrayCandidate.Min() > iCurrentCandidateTemp * 0.85) {

                        //Update texture
                        mHumanCurrentPose.Add(iTestIndex - 1);
                    }

                // next neighbour is not in next colon?
                if (iTestIndex % mTexture.width < (mTexture.width - 1))
                    if (mThermalSensorDataArrayCandidate[iTestIndex + 1] - mThermalSensorDataArrayCandidate.Min() > iCurrentCandidateTemp * 0.85) {

                        //Update texture
                        mHumanCurrentPose.Add(iTestIndex + 1);
                    }

                return true;
            } else
                return false;
        }

        private void PrintArray(float[] iThermalSensorDataMinArray, string iMessage)
        {
            for (int i = 0; i < iThermalSensorDataMinArray.Length;) {
                Debug.LogWarning(iMessage + " " + iThermalSensorDataMinArray[i] + " " + iThermalSensorDataMinArray[i + 1] +
                    " " + iThermalSensorDataMinArray[i + 2] + " " + iThermalSensorDataMinArray[i + 3] +
                    " " + iThermalSensorDataMinArray[i + 4] + " " + iThermalSensorDataMinArray[i + 5] +
                    " " + iThermalSensorDataMinArray[i + 6] + " " + iThermalSensorDataMinArray[i + 7]);

                i += 8;
            }
        }

        // Update Barycenter
        private void ComputeBarycenter()
        {
            for (int i = 0; i < mHumanCurrentPose.Count; ++i) {
                mMeanX += (mHumanCurrentPose[i] % mTexture.width);
                mMeanY += (mHumanCurrentPose[i] / mTexture.width);
            }
        }

    }
}