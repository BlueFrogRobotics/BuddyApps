using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class ParameterPatrolState : AStateGuardian
    {

        private ParametersGuardian mParameters;
        private TextToSpeech mTTS;
        private DetectionManager mDetectionManager;
        private Animator mAnimator;
        private bool mHasInitSlider = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            mAnimator = animator;
            mParameters = StateManager.Parameters;
            mDetectionManager = StateManager.DetectorManager;

            mParameters.gameObject.SetActive(true);
            animator.SetBool("ChangeState", false);
            mTTS = BYOS.Instance.TextToSpeech;
            mParameters.ButtonDebugSound.onClick.AddListener(ShowDebugSoundWindow);
            mParameters.ButtonDebugMovement.onClick.AddListener(ShowDebugMovementWindow);
            mParameters.ButtonDebugTemperature.onClick.AddListener(ShowDebugTemperatureWindow);
            mParameters.ButtonValidate.onClick.AddListener(Validate);
            mHasInitSlider = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mParameters.GaugeMovement.Slider)
            {
                mParameters.GaugeMovement.Slider.value = (1.0f - (mDetectionManager.MovementDetector.GetThreshold() / mDetectionManager.MovementDetector.GetMaxThreshold())) * mParameters.GaugeMovement.Slider.maxValue;
                mHasInitSlider = true;

                mParameters.GaugeSound.Slider.value = (1.0f - (mDetectionManager.SoundDetector.GetThreshold() / mDetectionManager.SoundDetector.GetMaxThreshold())) * mParameters.GaugeSound.Slider.maxValue;
            }

            SetDetectorsThreshold();

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //ParametersGuardian  mParameters = mParameterObject.GetComponent<ParametersGuardian>();
            DetectionPatrolState lDetectionState = animator.GetBehaviour<DetectionPatrolState>();
            lDetectionState.CanDetectFire = mParameters.ToggleFire.isOn;
            lDetectionState.CanDetectMovement = mParameters.ToggleMovement.isOn;
            lDetectionState.CanDetectSound = mParameters.ToggleSound.isOn;
            lDetectionState.CanDetectKidnapping = mParameters.ToggleKidnap.isOn;

            SetDetectorsThreshold();

            mParameters.gameObject.SetActive(false);
            animator.SetBool("ChangeState", false);
            mParameters.ButtonDebugSound.onClick.RemoveAllListeners();
            mParameters.ButtonDebugMovement.onClick.RemoveAllListeners();
            mParameters.ButtonDebugTemperature.onClick.RemoveAllListeners();
            mParameters.ButtonValidate.onClick.RemoveAllListeners();
            Debug.Log("fin param");
        }

        private void ShowDebugSoundWindow()
        {
            Debug.Log("show sound window");
            mAnimator.SetInteger("DebugMode", 0);
        }

        private void ShowDebugMovementWindow()
        {
            Debug.Log("show mouv window");
            mAnimator.SetInteger("DebugMode", 1);
        }

        private void ShowDebugTemperatureWindow()
        {
            Debug.Log("show temperature window");
            mAnimator.SetInteger("DebugMode", 2);
        }

        private void Validate()
        {
            mAnimator.SetBool("ChangeState", true);
        }

        private void SetDetectorsThreshold()
        {
            float lValue = 1.0f - (mParameters.GaugeFire.Slider.value / mParameters.GaugeFire.Slider.maxValue);
            lValue = 1.0f - (mParameters.GaugeMovement.Slider.value / mParameters.GaugeMovement.Slider.maxValue);
            mDetectionManager.MovementDetector.SetThreshold(lValue * mDetectionManager.MovementDetector.GetMaxThreshold());

            lValue = 1.0f - (mParameters.GaugeSound.Slider.value / mParameters.GaugeSound.Slider.maxValue);
            mDetectionManager.SoundDetector.SetThreshold(lValue * mDetectionManager.SoundDetector.GetMaxThreshold());

            lValue = 1.0f - (mParameters.GaugeKidnap.Slider.value / mParameters.GaugeKidnap.Slider.maxValue);
            mDetectionManager.KidnappingDetector.SetThreshold(lValue * mDetectionManager.KidnappingDetector.GetMaxThreshold());
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}