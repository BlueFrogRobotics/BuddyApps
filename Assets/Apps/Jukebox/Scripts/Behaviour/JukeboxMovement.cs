using UnityEngine;
using System.Collections;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Jukebox
{
    public class JukeboxMovement : MonoBehaviour
    {

        private int[] mMatrixThermal;
        private int mSumMatrixValue;
        private bool mIsThermalFollowActivated;
        private int mMaxThermalMatrix;
        private int[] mMatrixThermalCopy;
        private float mTimer;
        private bool mIsThermalFollow;

        public void Start()
        {
            mTimer = 0F;
            mSumMatrixValue = 0;
            mMatrixThermal = new int[16];
            mMatrixThermal = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
            mMatrixThermalCopy = mMatrixThermal;
            mIsThermalFollowActivated = false;
            mIsThermalFollow = false;
        }

        public void Update()
        {
            if (BYOS.Instance.Primitive.IRSensors.Middle.Distance < 0.4F && mIsThermalFollowActivated && mIsThermalFollow)
            {
                Debug.Log("UPDATE JUKEBOX STOP OBSTACLE");
                BYOS.Instance.Navigation.Stop();
                mIsThermalFollow = false;
            }
                
            if(mIsThermalFollowActivated && BYOS.Instance.Primitive.IRSensors.Middle.Distance > 0.4F && !mIsThermalFollow)
            {
                Debug.Log("UPDATE JUKEBOX GO");
                BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();
                mIsThermalFollow = true;
            }

            //mTimer += Time.deltaTime;
            //if ( mIsThermalFollowActivated && mTimer > 0.4F)
            //{
            //    mMatrixThermal = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
            //    mMatrixThermalCopy = mMatrixThermal;
            //    if (  mMatrixThermalCopy.Length != 0 && mMatrixThermalCopy != null)
            //    {
            //        for (int i = 0; i < mMatrixThermalCopy.Length; ++i)
            //        {
            //            Debug.Log("MATRIX THERMAL : " + mMatrixThermalCopy[i] + " id : " + i);
            //        }
            //        mMaxThermalMatrix = MaxValue(mMatrixThermalCopy);
            //        Debug.Log("mMaxThermalMatrixL : " + mMaxThermalMatrix);
            //        for (int i = 0; i < 16; ++i)
            //        {
            //            mSumMatrixValue += mMatrixThermalCopy[i];
            //        }

            //        mSumMatrixValue/=16;
            //        if (mSumMatrixValue < 10 && mMaxThermalMatrix < 12)
            //            return;
            //        Debug.Log("moyenne : " + mSumMatrixValue);
                    
            //        Debug.Log("moyenne : " + mSumMatrixValue + " mMaxThermalMatrix : " + mMaxThermalMatrix);
            //        if (mSumMatrixValue < 22 && mMaxThermalMatrix < 25)
            //        {
            //            Debug.Log("STOP THERMAL JUKEBOX --------------------------------------------------");
            //            BYOS.Instance.Navigation.Stop();
            //            mIsThermalFollowActivated = false;

            //        }
            //    }
            //   mTimer = 0F;
            //}
            
            
            //Debug.Log(thermalMovement.GetComponent<Companion.FollowPersonReaction>().enabled + " " + balladeMovement.GetComponent<CompanionWalk>().enabled);
        }

        public void Walk()
        {
            Debug.Log("start mouv");
            BYOS.Instance.Navigation.Stop();
            BYOS.Instance.Navigation.Roomba.enabled = true;
        }

        public void ThermalFollow()
        {
            mIsThermalFollowActivated = !mIsThermalFollowActivated;
            Debug.Log(mIsThermalFollowActivated + " : mIsThermalFollowActivated");
            if (BYOS.Instance.Navigation.Roomba.enabled == true)
                BYOS.Instance.Navigation.Roomba.enabled = false;

            if(mIsThermalFollowActivated)
            {
                BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();
                mIsThermalFollow = true;
            } 
            else if(!mIsThermalFollowActivated)
                BYOS.Instance.Navigation.Stop();
        }

        public void stopMovement()
        {
            Debug.Log("stop mouv");
            BYOS.Instance.Navigation.Roomba.enabled = false;
            BYOS.Instance.Navigation.Stop();
            BYOS.Instance.Primitive.Motors.Wheels.Stop();
            mIsThermalFollowActivated = false;
        }

        private int MaxValue(int[]  intArray )
        {
            int max = intArray[0];
            for (int i = 1; i<intArray.Length; i++) {
                 if (intArray[i] > max) {
                     max = intArray[i];
                 }
            }
            return max;
        }
    }
}
