﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public class BLPlay : AStateMachineBehaviour
    {
        private LabUIEditorManager mUIManager;
        private ItemControlUnit mItemControl;
        private bool mIsPlaying;
        private IEnumerator mSequence;
        private IEnumerator mPlay;
        private ThermalDetection mFireDetection;
        private MotionDetection mMotionDetection;
        private QRCodeDetection mQRcodeDetection;
        private TimelineDisplayer mTimelineDisplayer;
        private LoopManager mLoopManager;
        private ConditionManager mConditionManager;
        private BuddyLabBehaviour mBLBehaviour;
        private GameObject mButtonHideUI;
        private GameObject mUIToHide;
        private GameObject mSequenceToHide;

        public override void Start()
        {
            mSequenceToHide = GetGameObject(10);
            mButtonHideUI = GetGameObject(8);
            mUIToHide = GetGameObject(9);
            mConditionManager = GetGameObject(3).GetComponent<ConditionManager>();
            mLoopManager = GetGameObject(3).GetComponent<LoopManager>();
            mUIManager = GetComponent<LabUIEditorManager>();
            mItemControl = GetComponentInGameObject<ItemControlUnit>(4);
            mTimelineDisplayer = GetComponentInGameObject<TimelineDisplayer>(7);
            mBLBehaviour = GetComponentInGameObject<BuddyLabBehaviour>(3);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMotionDetection = BYOS.Instance.Perception.Motion;
            mFireDetection = BYOS.Instance.Perception.Thermal;
            mQRcodeDetection = BYOS.Instance.Perception.QRCode;
            mUIManager.OpenPlayUI();
            mIsPlaying = false;
            mUIManager.StopButton.onClick.AddListener(Stop);
            mUIManager.ReplayButton.onClick.AddListener(Replay);
            mTimelineDisplayer.DisplaySequence(mBLBehaviour.NameOpenProject + ".xml");
            ItemControlUnit.OnNextAction += ChangeItemHighlight;
            StartCoroutine(Play());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ItemControlUnit.OnNextAction -= ChangeItemHighlight;
            mTimelineDisplayer.HideSequence();
            mUIManager.StopButton.onClick.RemoveListener(Stop);
            mButtonHideUI.GetComponent<Button>().onClick.RemoveListener(HideUi);
            mUIManager.ReplayButton.onClick.RemoveListener(Replay);
            mUIManager.ClosePlayUI();
        }

        private void Stop()
        {
            mButtonHideUI.SetActive(false);
            ResetPosition();
            mItemControl.IsRunning = false;
            mLoopManager.ResetParam();
            mLoopManager.NeedChangeIndex();
            mConditionManager.ConditionType = "";
            mIsPlaying = false;
            mLoopManager.ChangeIndex = false;
            if (Primitive.RGBCam.IsOpen)
            {
                Debug.Log("CAMERA OPEN");
                //mMotionDetection.StopAllOnDetect();
                //mQRcodeDetection.StopAllOnDetect();
                //mFireDetection.StopAllOnDetect();
                Primitive.RGBCam.Close();
            }
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void Replay()
        {
            Debug.Log("REPLAY ");
            ResetPosition();
            mItemControl.IsRunning = false;
            mLoopManager.ResetParam();
            mLoopManager.NeedChangeIndex();
            mConditionManager.ConditionType = "";
            mIsPlaying = false;
            mLoopManager.ChangeIndex = false;
            if(!mIsPlaying)
            {
                Debug.Log("STARTCOROUTINE REPLAY ");
                //mItemControl.IsRunning = true;
                //mIsPlaying = true;
                StartCoroutine(Play());
            } 
        }

        private IEnumerator Play()
        {
            if (!mUIToHide.activeSelf)
                mUIToHide.SetActive(true);
            mButtonHideUI.SetActive(true);
            mButtonHideUI.GetComponent<Button>().onClick.AddListener(HideUi);
            yield return new WaitForSeconds(0.5F);
            mItemControl.IsRunning = true;
            mIsPlaying = true;
            Debug.Log("PLAY : AVANT YIELD RETURN : ISRUNNING " + mItemControl.IsRunning + " MISPLAYING : " + mIsPlaying);
            yield return mItemControl.PlaySequence();
            Debug.Log("PLAY : APRES YIELD RETURN : ISRUNNING " + mItemControl.IsRunning + " MISPLAYING : " + mIsPlaying);
            mIsPlaying = false;
        }

        private void ResetPosition()
        {
            //Primitive.Motors.Wheels.Locked = false;
            //Primitive.Motors.Wheels.Stop();
            Primitive.Motors.YesHinge.Locked = false;
            Primitive.Motors.YesHinge.SetPosition(0F, 100F);
            Primitive.Motors.NoHinge.Locked = false;
            Primitive.Motors.NoHinge.SetPosition(0F, 100F);
        }

        private void ChangeItemHighlight(int iNum)
        {
            mTimelineDisplayer.HighlightElement(iNum);
        }

        private void HideUi()
        {
           
            if (mUIToHide.activeSelf)
            {
                mUIToHide.SetActive(false);
                mSequenceToHide.SetActive(false);
                //Header.DisplayParametersButton = false;
            }
            else
            {
                mUIToHide.SetActive(true);
                mSequenceToHide.SetActive(true);
                //Header.DisplayParametersButton = true;
                
            }
        }
    }
}

