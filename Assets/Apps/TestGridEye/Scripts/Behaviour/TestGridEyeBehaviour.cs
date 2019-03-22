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
        private TestGridEyeData mAppData;

        private Sprite mSprite;
        private float[] mThermalSensorDataArray;
        private Texture2D mTexture;

        [SerializeField]
        private Text Score;

        [SerializeField]
        private Text Position;

        [SerializeField]
        private InputField mCoeffCandidate;

        [SerializeField]
        private InputField mCoeffNeighbours;

        [SerializeField]
        private InputField mScoreThresh;

        private bool mDisplay;
        private bool mMotion;
        private bool mFollow;
        private bool mPreviousLeft;
        private bool mStopped;
        private bool mLyingHuman;
        private float mMin;
        private int mMeanX;
        private int mMeanY;
        private int mPreviousIndexMax;
        private List<int> mHumanPosition;
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

        public void ToggleLyingHuman()
        {
            mLyingHuman = !mLyingHuman;
        }

        void Start()
        {
            TestGridEyeActivity.Init(null);
            mLyingHuman = false;
            mDisplay = true;
            mMotion = false;
            mPreviousIndexMax = -1;

            mHumanPosition = new List<int>();
            mRemovedCandidates = new List<int>();

            mTexture = new Texture2D(8, 8) {
                filterMode = FilterMode.Point,
                anisoLevel = 1,
                mipMapBias = -0.2F
            };

            mThermalSensorDataArray = new float[64];

            Buddy.Sensors.ThermalCamera.OnNewFrame.Add(NewThermalFrame);
        }


        private void NewThermalFrame(ThermalCameraFrame iThermalFrame)
        {
            mRemovedCandidates.Clear();
            iThermalFrame.Mat.get(0, 0, mThermalSensorDataArray);

            float lMax = mThermalSensorDataArray.Max();
            mMin = mThermalSensorDataArray.Min();

            Debug.LogWarning("min max: " + mMin + " " + lMax);

            if (mDisplay) {
                // Draw the current texture
                for (int i = 0; i < mThermalSensorDataArray.Length; i++) {
                    // Use full range
                    float lNormalizedTemp = (mThermalSensorDataArray[i] - mMin) / (lMax - mMin);

                    // "7 -" to turn the image upside down
                    mTexture.SetPixel(i % mTexture.width, (mTexture.height - 1) - i / mTexture.width, new Color(lNormalizedTemp, 0.0F, 1F - lNormalizedTemp, 0.85F));
                }
            }

            int lIndexMax = FindHuman(true);

            // if human
            if (lIndexMax > -1) {
                mPreviousIndexMax = lIndexMax;

                if (mDisplay)
                    DrawHuman(lIndexMax);

                Score.text = "Score: " + mHumanPosition.Count;
                Position.text = "Position: " + lIndexMax % mTexture.width + ":" + lIndexMax / mTexture.width;

                if (mMotion || mFollow)
                    OnHumanDetect(((float)mMeanX / mHumanPosition.Count) / (mTexture.width - 1));
            }


            // Display on toaster if needed
            if (mDisplay) {
                mTexture.Apply();
                mSprite = Sprite.Create(mTexture, new UnityEngine.Rect(0.0f, 0.0f, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f), 1.0F);

                if (!Buddy.GUI.Toaster.IsBusy)
                    Buddy.GUI.Toaster.Display<PictureToast>().With(mSprite,
                        () => {
                            Buddy.GUI.Toaster.Hide();
                            mDisplay = false;
                        });
            }
        }


        /// <summary>
        /// Once we find a human, do something with its position...
        /// </summary>
        /// <param name="iCentered"></param>
        /// <returns></returns>
        private bool OnHumanDetect(float iCentered)
        {
            Debug.LogWarning("centered: " + iCentered);
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
                Buddy.Actuators.Wheels.SetVelocities(0F, -40F);
                mStopped = false;
                mPreviousLeft = true;
            } else if (iCentered > 0.4F && (mPreviousLeft || mStopped /*|| !Buddy.Actuators.Wheels.IsBusy*/)) {
                Debug.LogWarning("Go to right " + mStopped);
                Buddy.Actuators.Wheels.SetVelocities(0F, 40F);
                mStopped = false;
                mPreviousLeft = false;
            }
            return true;
        }


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

                if (lCurrentCandidateTemp < float.Parse(mCoeffCandidate.text) * mMin)
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
            if (lCurrentCandidateTemp < float.Parse(mCoeffCandidate.text) * mMin) {

                //Debug.LogWarning("Current candidate at " + lIndexCurrentCandidate + " with temp " + mThermalSensorDataArray[lIndexCurrentCandidate] + " is too low compared to  " + float.Parse(mCoeffCandidate.text) * mMin);
                Score.text = "No human";
                return -1;

            } else {
                ////// WE HAVE A CANDIDATE!! //////////
                // Compute the score of the current candidate

                //mHumanPosition.Add(lIndexCurrentCandidate);

                // give more importance to verticality
                for (int i = 0; i < mTexture.height; i++) {
                    //Try pixel below
                    int lTestIndex = lIndexCurrentCandidate - i * mTexture.width;
                    if (lTestIndex >= 0) {
                        int PixelScore = TryPixel(lTestIndex, lCurrentCandidateTemp * float.Parse(mCoeffNeighbours.text));
                        if (PixelScore == 0)
                            break;
                    }
                }

                // Remove i = 0 to avoid taking into account twice the max pixel
                for (int i = 1; i < mTexture.height; i++) {
                    // Try pixel above
                    int lTestIndex = lIndexCurrentCandidate + i * mTexture.width;
                    if (lTestIndex < mTexture.height * mTexture.width) {
                        int PixelScore = TryPixel(lTestIndex, lCurrentCandidateTemp * float.Parse(mCoeffNeighbours.text));
                        if (PixelScore == 0)
                            break;
                    }
                }

                // Score good enough, stop here
                if (mHumanPosition.Count > int.Parse(mScoreThresh.text)) {
                    return lIndexCurrentCandidate;

                } else {
                    Debug.LogWarning("Score not enough:" + mHumanPosition.Count);

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
                float lCentered = ((float)(i % mTexture.width)) / (mTexture.width - 1);

                if (lCentered < 0.65F && lCentered > 0.35F) {

                    if (i < mThermalSensorDataArray.Length)
                        if (mThermalSensorDataArray[i] > float.Parse(mCoeffCandidate.text) * mMin) {
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
            if (iTestIndex % mTexture.width > 0)
                if (mThermalSensorDataArray[iTestIndex - 1] > iTempMin) {

                    //Update texture
                    //mTexture.SetPixel((iTestIndex - 1) % mTexture.width, (mTexture.height - 1) - iTestIndex / mTexture.width, new Color(0F, 1F, 0F, 1F));
                    mHumanPosition.Add(iTestIndex - 1);

                    lScore++;
                    if (mLyingHuman)
                        lScore += TryPreviousPixel(iTestIndex - 1, iTempMin);
                }
            return lScore;
        }


        private int TryNextPixel(int iTestIndex, float iTempMin)
        {
            int lScore = 0;
            // next neighbour is not in next colon?
            if (iTestIndex % mTexture.width < (mTexture.width - 1))
                if (mThermalSensorDataArray[iTestIndex + 1] > iTempMin) {

                    //Update texture
                    mHumanPosition.Add(iTestIndex + 1);

                    lScore++;
                    if (mLyingHuman)
                        lScore += TryPreviousPixel(iTestIndex + 1, iTempMin);
                }
            return lScore;
        }


        /// <summary>
        /// Draw human position in green with white as hot point and black as barycenter
        /// </summary>
        /// <param name="iIndexMax"></param>
        private void DrawHuman(int iIndexMax)
        {
            for (int i = 0; i < mHumanPosition.Count(); ++i)
                mTexture.SetPixel(mHumanPosition[i] % mTexture.width, (mTexture.height - 1) - mHumanPosition[i] / mTexture.width, new Color(0F, 1F, 0F, 1F));


            // Set max temp in white
            mTexture.SetPixel(iIndexMax % mTexture.width, (mTexture.height - 1) - (iIndexMax / mTexture.width), new Color(1F, 1F, 1F, 1F));
            mHumanPosition.Add(iIndexMax);

            // Set Barycenter in black
            ComputeBarycenter();
            mTexture.SetPixel(mMeanX / mHumanPosition.Count, (mTexture.height - 1) - (mMeanY / mHumanPosition.Count), new Color(0F, 0F, 0F, 1F));

            Debug.LogWarning("Barycenter value x : " + mMeanX + " y " + mMeanY + " score " + mHumanPosition.Count + " resulty 7 - " + ((mMeanY + 1) / mHumanPosition.Count));

        }

        // Update Barycenter
        private void ComputeBarycenter()
        {
            mMeanX = 0;
            mMeanY = 0;
            for (int i = 0; i < mHumanPosition.Count; ++i) {
                mMeanX += (mHumanPosition[i] % mTexture.width);
                mMeanY += (mHumanPosition[i] / mTexture.width);
                Debug.LogWarning("MeanY + " + i + " /8 = " + (i / mTexture.width) + " MeanY: " + mMeanY);
            }
        }

    }
}