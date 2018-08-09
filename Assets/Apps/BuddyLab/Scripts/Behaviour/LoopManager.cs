using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BlueQuark;

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

        [SerializeField]
        private ConditionManager ConditionManager;

        /// <summary>
        /// Variables for loop_X
        /// </summary>
        private int mCounterLoopX;
        private bool mIsLoopX;
        private int mLoopCounter;
        public int LoopCounter { get { return mLoopCounter; } set { mLoopCounter = value; } }
        private int mConvert;
        private int mIndexLoop;
        public int IndexLoop { get { return mIndexLoop; } set { mIndexLoop = value; } }

        private bool mChangeIndex;
        public bool ChangeIndex { get { return mChangeIndex; } set { mChangeIndex = value; } }

        /// <summary>
        /// Variables for Infinite loop
        /// </summary>
        private bool mIsInfiniteLoop;

        /// <summary>
        /// Variables for loop_Sensor
        /// </summary>
        private bool mIsSensorLoop;
        private bool mIsSensorLoopWithParam;
        public bool IsSensorLoopWithParam { get { return mIsSensorLoopWithParam; } set { mIsSensorLoopWithParam = value; } }

        /// <summary>
        /// Variables for Vision loop
        /// </summary>
        private bool mIsVisionLoop;
        


        // Use this for initialization
        void Start()
        {
            mIsInfiniteLoop = false;
            mIsLoopX = false;
            mTimer = 0F;
            mIndexLoop = 0;
            mChangeIndex = false;
            mIsInLoop = false;
            mLoopType = LoopType.NONE;
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
                {
                    LoopX();
                }
                if(mIsInfiniteLoop)
                {
                    LoopInfinite();
                }       
                if(mIsSensorLoop)
                {
                    LoopSensor();
                }
            }
        }

        // We could only do this in the update but we might need this, at the end of the development of Buddylab we will know if we delete this
        private void LoadLoop()
        {
            if(!mIsInLoop)
            {
                switch (mLoopType)
                {
                    case LoopType.INFINITE:
                        Debug.Log("Loop Infinite");
                        mIsInfiniteLoop = true;
                        mIsInLoop = true;
                        break;
                    case LoopType.LOOP_X:
                        Debug.Log("Loop X");
                        mIsLoopX = true;
                        mIsInLoop = true;
                        break;
                    case LoopType.SENSOR:
                        Debug.Log("Loop Sensor");
                        mIsSensorLoop = true;
                        mIsInLoop = true;
                        break;
                    case LoopType.VISION:
                        Debug.Log("Loop Vision");
                        mIsVisionLoop = true;
                        mIsInLoop = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoopVision()
        {

        }

        private void LoopSensor()
        {
            mIsSensorLoopWithParam = true;
        }

        private void LoopX()
        {
            if(mParamLoop != null)
            {
                try
                {
                    mConvert = Int32.Parse(mParamLoop);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                //Debug.Log("MCONVERT LOOP X : " + mConvert + " LOOPCOUNTER : " + mLoopCounter);
                //Debug.Log(" LOOP X MCHANGEINDEX: " + mChangeIndex);

                if (mLoopCounter < mConvert-1 )
                    return;
                else
                {
                    //Debug.Log("LOOPX RESETPARAM");
                    mLoopCounter = 1;
                    ResetParam();
                }
            }

        }

        private void LoopInfinite()
        {
            Debug.Log("INFINITE LOOP");
        }

        public void ResetParam()
        {
            mIsSensorLoop = false;
            mIsLoopX = false;
            mIsInfiniteLoop = false;
            mChangeIndex = true;
            mLoopType = LoopType.NONE;
            mIsInLoop = false;
        }

        public void NeedChangeIndex()
        {
            mChangeIndex = false;
        }

    }

}
