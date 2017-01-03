using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using BuddyFeature.Web;
using System;

namespace BuddyApp.BabyPhone
{
    [RequireComponent(typeof(InputMicro))]
    [RequireComponent(typeof(AudioSource))]
    public class SoundDetector : MonoBehaviour
    {
        private const int DETECTION_TIME = 2;
        private InputMicro mInputMicro;
        private float mSound;
        private float mMean;
        private float mCount;
        private float mElapsedTime;
        private float mMicroSensitivity;

        private bool mIsNoisy;
        public bool isNoisy { get { return mIsNoisy; } } 

        void OnEnable()
        {
            mMicroSensitivity = (BabyPhoneData.Instance.MicrophoneSensitivity / 100F);
            Debug.Log("Micro Sensitivity" + mMicroSensitivity);
            mInputMicro = GetComponent<InputMicro>();
            mSound = 0F;
            mMean = 0F;
            mCount = 0F;
            mElapsedTime = 0F;
            mIsNoisy = false;
        }

        void Update()
        {
            mElapsedTime += Time.deltaTime;

            if (mElapsedTime <= DETECTION_TIME)
            {
                mSound = mInputMicro.Loudness;
                mMean += mSound;
                mCount = mCount + 1;
            }
            else
            {
                mMean = mMean / mCount;
                
                if (mMean >= mMicroSensitivity)
                {
                    mIsNoisy = true;
                    Debug.Log("Do detect sound ? : " + mIsNoisy);
                }              
                else
                    mIsNoisy = false;

                mMean = 0;
                mCount = 0;
                mElapsedTime = 0;
            }
        }
    }
}
