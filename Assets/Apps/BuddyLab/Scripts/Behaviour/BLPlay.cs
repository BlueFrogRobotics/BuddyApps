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
        private LoopManager mLoopManager;
        private ConditionManager mConditionManager;
        private BuddyLabBehaviour mBLBehaviour;
        private GameObject mButtonHideUI;
        private GameObject mUIToHide;
        private GameObject mSequenceToHide;
        private HDCamera mHDCamera;

        private FButton mStopButton;
        private FButton mReplayButton;

        public override void Start()
        {
            //mMotionDetection = Buddy.Perception.MotionDetector;
            //mFireDetection = Buddy.Perception.ThermalDetector;
            //mQRcodeDetection = Buddy.Perception.QRCodeDetector;
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
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mUIManager.OpenPlayUI();
            //mIsPlaying = false;
            //mUIManager.StopButton.onClick.AddListener(Stop);
            //mUIManager.ReplayButton.onClick.AddListener(Replay);
            //mTimelineDisplayer.DisplaySequence(mBLBehaviour.NameOpenProject + ".xml");
            //ItemControlUnit.OnNextAction += ChangeItemHighlight;
            //StartCoroutine(Play());
            mTimelineDisplayer.DisplayAlgo();
            StartCoroutine(PlayTestInterpreter());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
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
            if (Buddy.Sensors.HDCamera.IsOpen)
            {
                Debug.Log("CAMERA OPEN");
                //mMotionDetection.StopAllOnDetect();
                //mQRcodeDetection.StopAllOnDetect();
                //mFireDetection.StopAllOnDetect();
               Buddy.Sensors.HDCamera.Close();
            }
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void StopAlgo()
        {
            Buddy.Behaviour.Interpreter.Stop();
            CloseFooter();
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void ReplayAlgo()
        {
            Buddy.Behaviour.Interpreter.Stop();
            Buddy.Behaviour.Interpreter.Run(mItemControl.BehaviourAlgorithm, mTimelineDisplayer.OnExecuteInstruction);
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
                //mTimelineDisplayer.HideSequence();
                //mTimelineDisplayer.DisplayAlgo();
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
            Buddy.Actuators.Head.Yes.Locked = false;
            Buddy.Actuators.Head.Yes.SetPosition(0F, 100F);
            Buddy.Actuators.Head.No.Locked = false;
            Buddy.Actuators.Head.No.SetPosition(0F, 100F);
        }

        private void ChangeItemHighlight(int iNum)
        {
            StartCoroutine(DelayForTimeline(iNum , 0.8F)); 
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
        IEnumerator DelayForTimeline (int iNum, float iDelay)
        {
            yield return new WaitForSeconds(iDelay);
            mTimelineDisplayer.HighlightElement(iNum);
        }

        IEnumerator PlayTestInterpreter()
        {
            //BehaviourAlgorithm lBeAlgo = new BehaviourAlgorithm();
            //lBeAlgo.Instructions.Add(new WaitBehaviourInstruction()
            //{
            //    Duration = 5F,
            //});
            //lBeAlgo.Instructions.Add(new SetMoodBehaviourInstruction()
            //{
            //    Duration = 4F,
            //    Mood = Mood.HAPPY
            //});
            //lBeAlgo.Instructions.Add(new SetMoodBehaviourInstruction()
            //{
            //    Duration = 4F,
            //    Mood = Mood.ANGRY
            //});
            //mItemControl.SaveAlgorithm();
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

