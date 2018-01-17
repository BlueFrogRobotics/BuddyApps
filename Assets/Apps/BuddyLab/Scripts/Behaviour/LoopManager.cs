using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Buddy;

namespace BuddyApp.BuddyLab
{
    public class LoopManager : MonoBehaviour
    {
        /// <summary>
        /// Variables for LoopManager
        /// </summary>
        private string mParamLoop;
        public string ParamLoop { get { return mParamLoop; } set { mParamLoop = value; } }

        private LoopType mLoopType;
        public LoopType LoopType
        {
            get { return mLoopType; }
            set
            {
                mLoopType = value;
            }
        }
        private float mTimer;
        private bool mIsInLoop;

        /// <summary>
        /// Variables for loop_X
        /// </summary>
        private int mCounterLoopX;
        private bool mIsLoopX;
        private int mLoopCounter;
        public int LoopCounter { get { return mLoopCounter; } set { mLoopCounter = value; } }
        private int mConvert;
        private int mIndexLoop;
        public int IndexLoop { set { mIndexLoop = value; } }

        // Use this for initialization
        void Start()
        {
            mTimer = 0F;
            mIndexLoop = 0;
        }

        // Update is called once per frame
        void Update()
        {
            mTimer += Time.deltaTime;
            if (mTimer > 1.0F)
            {
                mTimer = 0.0F;
                LoadLoop();
                if (mIsLoopX)
                    LoopX();
            }
        }

        private void LoadLoop()
        {
            if(!mIsInLoop)
            {
                switch (mLoopType)
                {
                    case LoopType.INFINITE:
                        Debug.Log("Loop Infinite");
                        mIsInLoop = true;
                        break;
                    case LoopType.LOOP_X:
                        Debug.Log("Loop X");
                        mIsLoopX = true;
                        mIsInLoop = true;
                        break;
                    case LoopType.SENSOR:
                        Debug.Log("Loop Sensor");
                        mIsInLoop = true;
                        break;
                    case LoopType.VISION:
                        Debug.Log("Loop Vision");
                        mIsInLoop = true;
                        break;
                    default:
                        break;
                }
            }

        }

        private void LoopX()
        {
            
            try
            {
                mConvert = Int32.Parse(mParamLoop);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            if (mLoopCounter < mConvert)
                return;
            else
            {

                ResetParam();
            }
        }

        private void ResetParam()
        {
            mLoopType = LoopType.NONE;
            mIsInLoop = false;
            mIndexLoop = 0;
        }
    }

}
