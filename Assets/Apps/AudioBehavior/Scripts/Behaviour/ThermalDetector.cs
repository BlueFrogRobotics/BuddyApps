using BlueQuark;

using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System;

namespace BuddyApp.AudioBehavior
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ThermalDetector
    {
        private float[] mThermalSensorDataArray;

        private const float mCoeffCandidate = 1.5F;
        private const float mCoeffNeighbours = 0.88F;
        private const int mScoreThresh = 7;
        public Action<HumanEntity[]> mCallback;

        private bool mStopped;
        private float mMin;
        private int mMeanX;
        private int mMeanY;
        private int mPreviousIndexMax;
        private List<int> mHumanPosition;
        private List<int> mRemovedCandidates;


        internal void Start()
        {
            mPreviousIndexMax = -1;

            mHumanPosition = new List<int>();
            mRemovedCandidates = new List<int>();

            mThermalSensorDataArray = new float[64];

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            mRemovedCandidates.Clear();
            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);

            float lMax = mThermalSensorDataArray.Max();
            mMin = mThermalSensorDataArray.Min();

            int lIndexMax = FindHuman(true);

            // if human
            if (lIndexMax > -1) {
                ComputeBarycenter();
                mPreviousIndexMax = lIndexMax;
                HumanEntity[] lHumanEntities = new HumanEntity[1];
                HumanEntity lHumanEntity = new HumanEntity();

                //Debug.LogWarning("MeanX " + mMeanX + " (mMeanX / mHumanPosition.Count) " + ((double)mMeanX / mHumanPosition.Count) + " /7F " + ((double)mMeanX / mHumanPosition.Count) / (7F));
                lHumanEntity.Center = new OpenCVUnity.Point(((double)mMeanX / mHumanPosition.Count) / (7F), ((double)mMeanY / mHumanPosition.Count));
                lHumanEntities[0] = lHumanEntity;

                mCallback(lHumanEntities);
            }
        }

        internal void Stop()
        {
            Buddy.Sensors.ThermalCamera.OnNewFrame.Clear();
        }


        /// <summary>
        /// Once we find a human, do something with its position...
        /// </summary>
        /// <param name="iCentered"></param>
        /// <returns></returns>
        //private bool OnHumanDetect(float iCentered)
        //{
        //   mTimeHumanDetected = Time.time;
        //    //float lCentered = ((float) iRow) / (mTexture.width - 1);
        //    if (iCentered < 0.6F && iCentered > 0.4F) {
        //        Debug.LogWarning("Stop " + mStopped);
        //        mStopped = true;
        //        Buddy.Actuators.Wheels.Stop();

        //        return false;

        //        // otherwise, try to put the human in the center
        //    } else if (iCentered < 0.6F && (!mPreviousLeft || mStopped /*!Buddy.Actuators.Wheels.IsBusy*/)) {
        //        Debug.LogWarning("Go to left " + mStopped);
        //        Buddy.Actuators.Wheels.SetVelocities(0F, -40F);
        //        mStopped = false;
        //        mPreviousLeft = true;
        //    } else if (iCentered > 0.4F && (mPreviousLeft || mStopped /*|| !Buddy.Actuators.Wheels.IsBusy*/)) {
        //        Debug.LogWarning("Go to right " + mStopped);
        //        Buddy.Actuators.Wheels.SetVelocities(0F, 40F);
        //        mStopped = false;
        //        mPreviousLeft = false;
        //    }
        //    return true;
        //}


        /// <summary>
        /// Find a human following a hot vertical form strategy
        /// </summary>
        /// <param name="iMiddleCandidate"></param>
        /// <returns></returns>
        private int FindHuman(bool iMiddleCandidate)
        {

            float lCurrentCandidateTemp = 0F;
            int lIndexCurrentCandidate = -1;

            // We try a new candidate, reset previous position
            mHumanPosition.Clear();


            ////// LOOKING FOR A CANDIDATE!! //////////

            if (mRemovedCandidates.Count == 0 && mPreviousIndexMax != -1) {
                // This is first try, test the previous human hotter position (previous winning candidate)
                lIndexCurrentCandidate = mPreviousIndexMax;
                lCurrentCandidateTemp = mThermalSensorDataArray[lIndexCurrentCandidate];
                mRemovedCandidates.Add(mPreviousIndexMax);

                if (lCurrentCandidateTemp < mCoeffCandidate * mMin)
                    lIndexCurrentCandidate = -1;
            }

            // Not first time and still some middle candidate? Try to find one!
            if (lIndexCurrentCandidate == -1)
                if (iMiddleCandidate) {
                    lIndexCurrentCandidate = GetNextMiddleCandidate();
                }


            if (lIndexCurrentCandidate != -1)
                lCurrentCandidateTemp = mThermalSensorDataArray[lIndexCurrentCandidate];
            // If no candidate in center and previous not good, Just get a hot pixel
            else {
                //Debug.LogWarning("No middle candidate, try somewhere else");
                iMiddleCandidate = false;
                lIndexCurrentCandidate = NextMaxCandidate();

                //No more candidate
                if (lIndexCurrentCandidate == -1)
                    return -1;


                //Debug.LogWarning("We have a NOT middle candidate at " + lIndexCurrentCandidate + " with temp " + mThermalSensorDataArray[lIndexCurrentCandidate]);

                lCurrentCandidateTemp = mThermalSensorDataArray[lIndexCurrentCandidate];
            }

            // Is the current candidate ok?
            if (lCurrentCandidateTemp < mCoeffCandidate * mMin) {

                //Debug.LogWarning("Current candidate at " + lIndexCurrentCandidate + " with temp " + mThermalSensorDataArray[lIndexCurrentCandidate] + " is too low compared to  " + float.Parse(mCoeffCandidate.text) * mMin);
                return -1;

            } else {
                ////// WE HAVE A CANDIDATE!! //////////
                // Compute the score of the current candidate

                //mHumanPosition.Add(lIndexCurrentCandidate);

                // give more importance to verticality
                for (int i = 0; i < 8; i++) {
                    //Try pixel below
                    int lTestIndex = lIndexCurrentCandidate - i * 8;
                    if (lTestIndex >= 0) {
                        int PixelScore = TryPixel(lTestIndex, lCurrentCandidateTemp * mCoeffNeighbours);
                        if (PixelScore == 0)
                            break;
                    }
                }

                // Remove i = 0 to avoid taking into account twice the max pixel
                for (int i = 1; i < 8; i++) {
                    // Try pixel above
                    int lTestIndex = lIndexCurrentCandidate + i * 8;
                    if (lTestIndex < 8 * 8) {
                        int PixelScore = TryPixel(lTestIndex, lCurrentCandidateTemp * mCoeffNeighbours);
                        if (PixelScore == 0)
                            break;
                    }
                }

                // Score good enough, stop here
                if (mHumanPosition.Count > mScoreThresh) {
                    return lIndexCurrentCandidate;

                } else {
                    Debug.Log("Score not enough:" + mHumanPosition.Count);

                    // Try to find another candidate
                    mRemovedCandidates.Add(lIndexCurrentCandidate);
                    return FindHuman(iMiddleCandidate);
                }
            }
        }


        /// <summary>
        /// Find a high temperature candidate in middle of thermal matrix
        /// </summary>
        /// <returns></returns>
        private int GetNextMiddleCandidate()
        {
            for (int i = 0; i < mThermalSensorDataArray.Length; i++) {
                //if removed candidate, go to next one
                if (mRemovedCandidates.Contains(i))
                    continue;

                // if we are on the side, go to center
                float lCentered = ((float)(i % 8)) / (8 - 1);

                if (lCentered < 0.65F && lCentered > 0.35F) {

                    if (i < mThermalSensorDataArray.Length)
                        if (mThermalSensorDataArray[i] > mCoeffCandidate * mMin) {
                            // if temp high enough, return this candidate
                            //Debug.LogWarning("We have a middle candidate at " + i + " with temp " + mThermalSensorDataArray[i]);
                            return i;
                        }
                } else
                    continue;
            }
            return -1;
        }


        /// <summary>
        /// Find a candidate in the frame that is not removed and is hot enough
        /// </summary>
        /// <returns></returns>
        private int NextMaxCandidate()
        {
            float lMax = 0F;
            int lIndexMax = -1;
            for (int i = 0; i < mThermalSensorDataArray.Length; ++i) {
                //if removed candidate, go to next one
                if (mRemovedCandidates.Contains(i))
                    continue;
                else {
                    if (lMax < mThermalSensorDataArray[i]) {
                        lMax = mThermalSensorDataArray[i];
                        lIndexMax = i;
                    }
                }
            }

            //Debug.LogWarning("return next max candidate at " + lIndexMax + " and temp " + lMax);

            return lIndexMax;
        }


        /// <summary>
        /// Let's see if the current pixel is hot enough to be in the human shape
        /// </summary>
        /// <param name="iTestIndex"></param>
        /// <param name="iTempMin"></param>
        /// <returns></returns>
        private int TryPixel(int iTestIndex, float iTempMin)
        {
            int lScore = 0;

            if (mThermalSensorDataArray[iTestIndex] > iTempMin) {
                mHumanPosition.Add(iTestIndex);

                // this pixel cannot be a candidatelIndexMax
                mRemovedCandidates.Add(iTestIndex);

                lScore++;

                // Try neighbours
                lScore += TryPreviousPixel(iTestIndex, iTempMin);
                lScore += TryNextPixel(iTestIndex, iTempMin);
            }

            return lScore;
        }


        // TODO: put previous and next in same function
        private int TryPreviousPixel(int iTestIndex, float iTempMin)
        {
            int lScore = 0;
            // previous neighbour is not in previous colon?
            if (iTestIndex % 8 > 0)
                if (mThermalSensorDataArray[iTestIndex - 1] > iTempMin) {

                    //Update texture
                    //mTexture.SetPixel((iTestIndex - 1) % mTexture.width, (mTexture.height - 1) - iTestIndex / mTexture.width, new Color(0F, 1F, 0F, 1F));
                    mHumanPosition.Add(iTestIndex - 1);

                    lScore++;
                }
            return lScore;
        }


        private int TryNextPixel(int iTestIndex, float iTempMin)
        {
            int lScore = 0;
            // next neighbour is not in next colon?
            if (iTestIndex % 8 < (8 - 1))
                if (mThermalSensorDataArray[iTestIndex + 1] > iTempMin) {

                    //Update texture
                    mHumanPosition.Add(iTestIndex + 1);

                    lScore++;
                   
                }
            return lScore;
        }


       

        // Update Barycenter
        private void ComputeBarycenter()
        {
            mMeanX = 0;
            mMeanY = 0;
            for (int i = 0; i < mHumanPosition.Count; ++i) {
                mMeanX += (mHumanPosition[i] % 8);
                mMeanY += (mHumanPosition[i] / 8);
                Debug.Log("MeanY + " + i + " /8 = " + (i / 8) + " MeanY: " + mMeanY);
            }
        }

    }
}