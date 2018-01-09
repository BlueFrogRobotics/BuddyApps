using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

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

        public override void Start()
        {
            mUIManager = GetComponent<LabUIEditorManager>();
            mItemControl = GetComponentInGameObject<ItemControlUnit>(4);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMotionDetection = BYOS.Instance.Perception.Motion;
            mFireDetection = BYOS.Instance.Perception.Thermal;
            mQRcodeDetection = BYOS.Instance.Perception.QRCode;
            mUIManager.OpenPlayUI();
            mPlay = Play();
            mIsPlaying = false;
            mUIManager.StopButton.onClick.AddListener(Stop);
            mUIManager.ReplayButton.onClick.AddListener(Replay);
            StartCoroutine(Play());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mUIManager.StopButton.onClick.RemoveListener(Stop);
            mUIManager.ReplayButton.onClick.RemoveListener(Replay);
            mUIManager.ClosePlayUI();
        }

        private void Stop()
        {
            Debug.Log("STOP BUTTON FDP");
            mItemControl.IsRunning = false;
            if (Primitive.RGBCam.IsOpen)
            {
                Debug.Log("CAMERA OPEN");
                mMotionDetection.StopAllOnDetect();
                mQRcodeDetection.StopAllOnDetect();
                mFireDetection.StopAllOnDetect();
                Primitive.RGBCam.Close();
            }
            GetGameObject(6).GetComponent<Animator>().SetTrigger("open");
            Trigger("Scene");
        }

        private void Replay()
        {
            mIsPlaying = false;
            if(!mIsPlaying)
                StartCoroutine(mPlay);
        }

        private IEnumerator Play()
        {
            mItemControl.IsRunning = true;
            mIsPlaying = true;
            yield return mItemControl.PlaySequence();
            mIsPlaying = false;
        }

    }
}

