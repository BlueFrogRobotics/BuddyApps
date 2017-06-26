using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.FreezeDance
{
    public class GameState : AStateMachineBehaviour
    {
        override public void Start()
        {

        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //mIsMoving = motionGame.GetComponent<MotionGame>().IsMoving();
            //if (mIsOnGame)
            //{

            //    if (speaker.isPlaying)
            //    {
            //        mElapsedTime += Time.deltaTime;
            //        float valueX = mElapsedTime / mAudioClipLength;
            //        progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(valueX, 0);
            //    }
            //    float lTime = Time.time;
            //    if (!mIsSetRandomStop)
            //        mRandomStopDelay = Random.Range(10, 30);
            //    if (lTime - mTime > mRandomStopDelay)
            //        RandomStop();
            //    if (!mStartMusic)
            //    {

            //        if (mIsMoving && !mIsOccupied && mPauseMusic)
            //            StartCoroutine(SetAngry());
            //        if (!mIsMoving && !mIsOccupied && mPauseMusic)
            //        {
            //            StartCoroutine(SetNeutral());
            //            if (mChrono)
            //                StartCoroutine(chrono());
            //        }
            //    }
            //}
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

    }
}