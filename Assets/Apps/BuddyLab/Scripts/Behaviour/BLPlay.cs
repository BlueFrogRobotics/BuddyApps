using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using UnityEngine.UI;

namespace BuddyApp.BuddyLab
{
    public sealed class BLPlay : AStateMachineBehaviour
    {
        private LabUIEditorManager mUIManager;
        private ItemControlUnit mItemControl;
        private bool mIsPlaying;
        private IEnumerator mSequence;
        private IEnumerator mPlay;
        private ThermalDetector mFireDetection;
        private MotionDetector mMotionDetection;
        private QRCodeDetector mQRcodeDetection;
        private TimelineDisplayer mTimelineDisplayer;
        private BuddyLabBehaviour mBLBehaviour;
        private GameObject mButtonHideUI;
        private GameObject mUIToHide;
        private GameObject mSequenceToHide;
        private HDCamera mHDCamera;

        private FButton mStopButton;
        private FButton mReplayButton;

        private bool mButtonsVisible;

        private float mTime;

        public override void Start()
        {
            mSequenceToHide = GetGameObject(10);
            mButtonHideUI = GetGameObject(8);
            mUIToHide = GetGameObject(9);
            mUIManager = GetComponent<LabUIEditorManager>();
            mItemControl = GetComponentInGameObject<ItemControlUnit>(4);
            mTimelineDisplayer = GetComponentInGameObject<TimelineDisplayer>(7);
            mBLBehaviour = GetComponentInGameObject<BuddyLabBehaviour>(3);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimelineDisplayer.DisplayAlgo();
            StartCoroutine(PlayTestInterpreter());
            mTime = 0.0F;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimelineDisplayer.HideSequence();
            mUIManager.StopButton.onClick.RemoveListener(Stop);
            mButtonHideUI.GetComponent<Button>().onClick.RemoveListener(HideUi);
            mUIManager.ClosePlayUI();
        }

        private void Stop()
        {
            mButtonHideUI.SetActive(false);
            ResetPosition();
            mIsPlaying = false;

            if (Buddy.Sensors.HDCamera.IsOpen)
            {
               Buddy.Sensors.HDCamera.Close();
            }
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void StopAlgo()
        {
            Buddy.Behaviour.Interpreter.Stop();
            Buddy.Actuators.Head.Yes.SetPosition(20);
            Buddy.Actuators.Head.No.SetPosition(0);
            CloseFooter();
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void ReplayAlgo()
        {
            Buddy.Behaviour.Interpreter.Stop();
            Buddy.Behaviour.Interpreter.Run(mItemControl.BehaviourAlgorithm, mTimelineDisplayer.OnExecuteInstruction);
        }

        private void ResetPosition()
        {
            Buddy.Actuators.Head.Yes.Locked = false;
            Buddy.Actuators.Head.Yes.SetPosition(20F, 50F);
            Buddy.Actuators.Head.No.Locked = false;
            Buddy.Actuators.Head.No.SetPosition(0F, 50F);
        }

        private void ChangeItemHighlight(int iNum)
        {
            StartCoroutine(DelayForTimeline(iNum , 0.8F)); 
        }

        private void HideUi()
        {
            if (mTime > 2.0F) {
                mTime = 0.0F;
                if (mButtonsVisible) {
                    mTimelineDisplayer.EnableTimeline(false);
                    CloseFooter();
                    mButtonsVisible = false;
                } else {
                    mTimelineDisplayer.EnableTimeline(true);
                    ShowFooter();
                    mButtonsVisible = true;

                }
            }
        }
        IEnumerator DelayForTimeline (int iNum, float iDelay)
        {
            yield return new WaitForSeconds(iDelay);
        }

        IEnumerator PlayTestInterpreter()
        {
            mButtonsVisible = true;
            mButtonHideUI.SetActive(true);
            mButtonHideUI.GetComponent<Button>().onClick.AddListener(HideUi);
            ShowFooter();
            Buddy.Behaviour.Interpreter.Run(mItemControl.BehaviourAlgorithm, mTimelineDisplayer.OnExecuteInstruction);
            yield return null;
        }

        private void ShowFooter()
        {
            mStopButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mStopButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_stop"));

            mStopButton.SetBackgroundColor(Color.white);
            mStopButton.SetIconColor(Color.black);
            mStopButton.OnClick.Add(() => { StopAlgo(); });

            mReplayButton = Buddy.GUI.Footer.CreateOnRight<FButton>();

            mReplayButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_redo"));

            mReplayButton.SetBackgroundColor(Color.white);
            mReplayButton.SetIconColor(Color.black);
            mReplayButton.OnClick.Add(() => { ReplayAlgo(); });
        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mStopButton);
            Buddy.GUI.Footer.Remove(mReplayButton);
        }
    }
}

