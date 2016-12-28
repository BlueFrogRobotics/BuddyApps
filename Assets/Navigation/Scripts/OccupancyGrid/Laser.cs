using UnityEngine;
using BuddyAPI;

namespace BasicSLAM
{
    public class Laser
    {
        public Laser()
        {
            mScan = new float[1];
        }
        /// <summary>
        /// Maximum distance detected by the scan, in meters
        /// </summary>
        public float mMaxRange = 4f;

        /// <summary>
        /// Minimum distance detected by the scan, in meters
        /// </summary>
        public float mMinRange = 0.45f;

        /// <summary>
        /// Maximum angle detected by the scan, in radian
        /// </summary>
        public float mMaxAngle = Mathf.PI / 6f;

        /// <summary>
        /// Minimum angle detected by the scan, in radian
        /// </summary>
        public float mMinAngle = -Mathf.PI / 6f;

        /// <summary>
        /// Field of view of the scan, in degrees
        /// </summary>
        public float mFieldOfView = 60f;

        /// <summary>
        /// Angle step between successif points, in radian
        /// </summary>
        public float mIncrementAngle = Mathf.PI / 360f;

        /// <summary>
        /// Maximum distance detected by the scan, in meters
        /// </summary>
        public float mMaxHeight = 0.1f;

        /// <summary>
        /// Minimum height of laser dectection
        /// </summary>
        public float mMinHeight = -0.1f;

        /// <summary>
        /// Laser scan in polar coordinates (range, bearing). 
        /// </summary>
        public float[] mScan;

        public void PointCloudToLaserScan(DepthPoint[] iPointCloud)
        {
            if (iPointCloud != null)
            {

                for (int i = 0; i < mScan.Length; i++)
                {
                    mScan[i] = 1000f;
                }

                for (int i = 0; i < iPointCloud.Length; i++)
                {
                    float lX = iPointCloud[i].x;
                    float lY = iPointCloud[i].y;
                    float lZ = iPointCloud[i].z;
                    float lRange = 0;
                    float lAngle = 0;

                    //on ne regarde que les points compris entre Hmin et Hmax
                    if ((lY > mMinHeight) && (lY < mMaxHeight))
                    {
                        lRange = Range(lX, lZ);
                        if (lRange < mMinRange)
                            lRange = mMinRange;
                        lAngle = Bearing(lZ, lX);

                        //on ne garde que les points entre -30 et +30 degrès 
                        if ((lAngle > mMinAngle) && (lAngle < mMaxAngle) && (lRange < mMaxRange))
                        {
                            int index = (int)((lAngle - mMinAngle) / mIncrementAngle);

                            if (lRange < mScan[index])
                            {
                                mScan[index] = lRange;
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Computes euclidien distance 
        /// </summary>
        /// <param name="iX"></param>
        /// <param name="iY"></param>
        /// <returns></returns>
        public float Range(float iX, float iY)
        {
            return Mathf.Sqrt((iX * iX) + (iY * iY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iX"></param>
        /// <param name="iY"></param>
        /// <returns></returns>
        public float Bearing(float iX, float iY)
        {
            return Mathf.Atan2(iY, iX);
        }

    }
}