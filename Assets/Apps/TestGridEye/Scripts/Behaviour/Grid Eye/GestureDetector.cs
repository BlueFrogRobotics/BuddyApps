using UnityEngine;
using System.Collections;



namespace BuddyApp.TestGridEye
{

    public class GestureDetector : MonoBehaviour
    {

        int mColumnSumGradientIdxH = 1;
        int mColumnSumGradientIdxV = 1;
        int[] mFirFilterCoeffs = { -1, 0, 1 };
        int mIdxMedH;
        int mIdxMedV;
        int mCountLeft;
        int mCountRight;
        int mCountUp;
        int mCountDown;

        public enum DirectionH : int { NONE, LEFT, RIGHT }
        public enum DirectionV : int { NONE, UP, DOWN }

        // Use this for initialization
        void Start()
        {
            mIdxMedH = 0;
            mIdxMedV = 0;
            mCountLeft = 0;
            mCountRight = 0;
            mCountUp = 0;
            mCountDown = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public DirectionH CheckHorizontalSwipe(GE_Image_Data imageData)
        {
            DirectionH lDirectionH = DirectionH.NONE;
            int lMaxGDerivation = 0;
            int lMinGDerivation = int.MaxValue;
            int lIdxMin = 0;
            int lIdxMax = 0;
            int lAbsMaxSlope = 0;
            long[] lSumOfcolumn = new long[8];
            int[] lColumnSumGradientDerivation = new int[8];
            int[,] lColumnSumGradient = new int[8, 8];

            for (int i = 0; i < 8; i++) {
                //Calculate the sum within a column
                long sum = 0;
                for (int n = 0; n < 8; n++) {
                    //sum += imageData.g_ashSnrAveTemp[i + n * 8];
                    sum += imageData.g_aucRaw[i + n * 8];
                    //Debug.Log("image " + imageData.g_ashSnrAveTemp[i + n * 8]);
                }
                lSumOfcolumn[i] = sum;

                //Calculate the gradient over the column sums and the derivation over time.
                if (i > 0) {
                    lColumnSumGradient[mColumnSumGradientIdxH, i - 1] = (int)(lSumOfcolumn[i] - lSumOfcolumn[i - 1]);
                    //Debug.Log("column: " + columnSumGradient[columnSumGradientIdx, i - 1]);
                    // Use a derivation filter with coeffs: -1, 0, 1 to get a higher maximum and minimum.
                    int lFirFilterSum = 0;
                    for (int n = 0; n < 3; n++) {
                        lFirFilterSum = lFirFilterSum + lColumnSumGradient[mColumnSumGradientIdxH, i - 1] * mFirFilterCoeffs[n];
                        mColumnSumGradientIdxH = (mColumnSumGradientIdxH + 1) % 3;
                    }
                    lColumnSumGradientDerivation[i - 1] = lFirFilterSum;
                    // Can be used for normalization
                    if (Mathf.Abs(lColumnSumGradient[mColumnSumGradientIdxH, i - 1]) > lAbsMaxSlope) {
                        lAbsMaxSlope = Mathf.Abs(lColumnSumGradient[mColumnSumGradientIdxH, i - 1]);
                    }
                }
            }
            mColumnSumGradientIdxH = (mColumnSumGradientIdxH + 1) % 3;
            string lDerivation = "";
            for (int i = 0; i < 7; i++) {
                lDerivation += lColumnSumGradientDerivation[i];
                lDerivation += ", ";
                if (lMaxGDerivation < lColumnSumGradientDerivation[i]) {
                    lMaxGDerivation = lColumnSumGradientDerivation[i];
                    lIdxMax = i;
                }
                if (lMinGDerivation > lColumnSumGradientDerivation[i]) {
                    lMinGDerivation = lColumnSumGradientDerivation[i];
                    lIdxMin = i;
                }

            }
            int lDiffDerivation = lMaxGDerivation - lMinGDerivation;
            //Debug.Log("diff max-min: " + (mMaxGDerivation - mMinGDerivation));
            lMaxGDerivation = 0;
            lMinGDerivation = int.MaxValue;
            int lIdxMed = (lIdxMin + lIdxMax) / 2;
            if (lIdxMed < mIdxMedH && lDiffDerivation > 20) {
                mCountLeft++;
                mCountRight = 0;
                Debug.Log("idx: " + lIdxMed);
            } else if (lIdxMed > mIdxMedH && lDiffDerivation > 20) {
                mCountLeft = 0;
                mCountRight++;
                Debug.Log("idx: " + lIdxMed);
            }
            mIdxMedH = lIdxMed;
            //Debug.Log("idx: " + mIdxMed);
            if (mCountRight > 1) {
                lDirectionH = DirectionH.RIGHT;
                Debug.Log("right");
                mCountRight = 0;
            } else if (mCountLeft > 1) {
                lDirectionH = DirectionH.LEFT;
                Debug.Log("left");
                mCountLeft = 0;
            }

            return lDirectionH;
            //Debug.Log(lDerivation);
            //for (int j = 0; j < 64; j++)
            //    Debug.Log("image " + imageData.g_ashSnrAveTemp[j]);
            // Create the curve graph
            //Point[] derivationPoints = new Point[7];
            //for (int i = 0; i < 7; i++)
            //{
            //    int x = pictureBox1.Width * i / 6;
            //    int y = columnSumGradientDerivation[i] * (pictureBox1.Height / 2) / 6000/*absMaxSlope*/ + (pictureBox1.Height / 2);
            //    derivationPoints[i] = new Point(x, y);
            //}
            //Bitmap curveImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //System.Drawing.Graphics curveImageGrafic = Graphics.FromImage(curveImage);
            //curveImageGrafic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //curveImageGrafic.DrawLine(new Pen(Color.LightGray, 1), new Point(0, pictureBox1.Height / 2), new Point(pictureBox1.Width, pictureBox1.Height / 2));
            //curveImageGrafic.DrawCurve(new Pen(Color.Black, 2), derivationPoints, 0.1f);
            //pictureBox1.Image = curveImage;
        }

        public DirectionV CheckVerticalSwipe(GE_Image_Data imageData)
        {
            DirectionV lDirectionV = DirectionV.NONE;
            int lMaxGDerivation = 0;
            int lMinGDerivation = int.MaxValue;
            int lIdxMin = 0;
            int lIdxMax = 0;
            int lAbsMaxSlope = 0;
            long[] lSumOfcolumn = new long[8];
            int[] lColumnSumGradientDerivation = new int[8];
            int[,] lColumnSumGradient = new int[8, 8];

            for (int i = 0; i < 8; i++) {
                //Calculate the sum within a column
                long sum = 0;
                for (int n = 0; n < 8; n++) {
                    //sum += imageData.g_ashSnrAveTemp[i + n * 8];
                    sum += imageData.g_aucRaw[i * 8 + n];
                    //Debug.Log("image " + imageData.g_ashSnrAveTemp[i + n * 8]);
                }
                lSumOfcolumn[i] = sum;

                //Calculate the gradient over the column sums and the derivation over time.
                if (i > 0) {
                    lColumnSumGradient[mColumnSumGradientIdxV, i - 1] = (int)(lSumOfcolumn[i] - lSumOfcolumn[i - 1]);
                    //Debug.Log("column: " + columnSumGradient[columnSumGradientIdx, i - 1]);
                    // Use a derivation filter with coeffs: -1, 0, 1 to get a higher maximum and minimum.
                    int firFilterSum = 0;
                    for (int n = 0; n < 3; n++) {
                        firFilterSum = firFilterSum + lColumnSumGradient[mColumnSumGradientIdxV, i - 1] * mFirFilterCoeffs[n];
                        mColumnSumGradientIdxV = (mColumnSumGradientIdxV + 1) % 3;
                    }
                    lColumnSumGradientDerivation[i - 1] = firFilterSum;
                    // Can be used for normalization
                    if (Mathf.Abs(lColumnSumGradient[mColumnSumGradientIdxV, i - 1]) > lAbsMaxSlope) {
                        lAbsMaxSlope = Mathf.Abs(lColumnSumGradient[mColumnSumGradientIdxV, i - 1]);
                    }
                }
            }
            mColumnSumGradientIdxV = (mColumnSumGradientIdxV + 1) % 3;
            string lDerivation = "";
            for (int i = 0; i < 7; i++) {
                lDerivation += lColumnSumGradientDerivation[i];
                lDerivation += ", ";
                if (lMaxGDerivation < lColumnSumGradientDerivation[i]) {
                    lMaxGDerivation = lColumnSumGradientDerivation[i];
                    lIdxMax = i;
                }
                if (lMinGDerivation > lColumnSumGradientDerivation[i]) {
                    lMinGDerivation = lColumnSumGradientDerivation[i];
                    lIdxMin = i;
                }

            }
            int lDiffDerivation = lMaxGDerivation - lMinGDerivation;
            //Debug.Log("diff max-min: " + (mMaxGDerivation - mMinGDerivation));
            lMaxGDerivation = 0;
            lMinGDerivation = int.MaxValue;
            int lIdxMed = (lIdxMin + lIdxMax) / 2;
            if (lIdxMed < mIdxMedV && lDiffDerivation > 20) {
                mCountDown++;
                mCountUp = 0;
                Debug.Log("idx: " + lIdxMed);
            } else if (lIdxMed > mIdxMedV && lDiffDerivation > 20) {
                mCountDown = 0;
                mCountUp++;
                Debug.Log("idx: " + lIdxMed);
            }
            mIdxMedV = lIdxMed;
            //Debug.Log("idx: " + mIdxMed);
            if (mCountUp > 1) {
                lDirectionV = DirectionV.UP;
                Debug.Log("up");
                mCountUp = 0;
            } else if (mCountDown > 1) {
                lDirectionV = DirectionV.DOWN;
                Debug.Log("down");
                mCountDown = 0;
            }

            return lDirectionV;

        }
    }
}
