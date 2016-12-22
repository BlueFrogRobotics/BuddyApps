using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class ParameterPatrolState : AStateGuardian
    {

        private ParametersGuardian mParameters;
        private DetectionManager mDetectionManager;
        private Animator mAnimator;
        private bool mHasInitSlider = false;
        private bool mHasSwitchedState = false;
        private Animator mAnimatorParameter;

        private enum NextState : int
        {
            NONE,
            DEBUG_TEMP,
            DEBUG_MOV,
            DEBUG_SOUND,
            HEAD_CONTROL,
            BACK,
            VALIDATE
        }

        private NextState mNextState;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            mAnimator = animator;
            mParameters = StateManager.Parameters;
            mDetectionManager = StateManager.DetectorManager;
            mAnimatorParameter = mParameters.gameObject.GetComponent<Animator>();

            mAnimatorParameter.SetTrigger("Open_WParameters");
            mParameters.gameObject.SetActive(true);
            animator.SetBool("ChangeState", false);

            mParameters.ButtonDebugSound.onClick.AddListener(ShowDebugSoundWindow);
            mParameters.ButtonDebugMovement.onClick.AddListener(ShowDebugMovementWindow);
            mParameters.ButtonDebugTemperature.onClick.AddListener(ShowDebugTemperatureWindow);
            mParameters.ButtonHeadControl.onClick.AddListener(ShowHeadControllerWindow);
            mParameters.ButtonValidate.onClick.AddListener(Validate);
            mParameters.ButtonBack.onClick.AddListener(Back);
            mHasInitSlider = false;
            mHasSwitchedState = false;
            mNextState = NextState.NONE;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mParameters.SliderMovement)
            {
                mParameters.SliderMovement.value = (1.0f - (mDetectionManager.MovementDetector.GetThreshold() / mDetectionManager.MovementDetector.GetMaxThreshold())) * mParameters.SliderMovement.maxValue;
                mHasInitSlider = true;

                mParameters.SliderSound.value = (1.0f - (mDetectionManager.SoundDetector.GetThreshold() / mDetectionManager.SoundDetector.GetMaxThreshold())) * mParameters.SliderSound.maxValue;
            }

            else if (mHasInitSlider && !mHasSwitchedState && mAnimatorParameter.GetCurrentAnimatorStateInfo(0).IsName("Window_Parameters_Off") && mNextState!=NextState.NONE)
            {
                Debug.Log("fin param");
                mHasSwitchedState = true;
                ChangeState();
            }

            SetDetectorsThreshold();

            

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //ParametersGuardian  mParameters = mParameterObject.GetComponent<ParametersGuardian>();
            //DetectionPatrolState lDetectionState = animator.GetBehaviour<DetectionPatrolState>();
            //lDetectionState.CanDetectFire = mParameters.ToggleFire.isOn;
            //lDetectionState.CanDetectMovement = mParameters.ToggleMovement.isOn;
            //lDetectionState.CanDetectSound = mParameters.ToggleSound.isOn;
            //lDetectionState.CanDetectKidnapping = mParameters.ToggleKidnap.isOn;

            SetDetectorsThreshold();

            //mParameters.gameObject.SetActive(false);
            
            animator.SetBool("ChangeState", false);
            mAnimator.SetBool("Back", false);
            mParameters.ButtonDebugSound.onClick.RemoveAllListeners();
            mParameters.ButtonDebugMovement.onClick.RemoveAllListeners();
            mParameters.ButtonDebugTemperature.onClick.RemoveAllListeners();
            mParameters.ButtonHeadControl.onClick.RemoveAllListeners();
            mParameters.ButtonValidate.onClick.RemoveAllListeners();
            mParameters.ButtonBack.onClick.RemoveAllListeners();
            Debug.Log("fin param");
        }

        private void ChangeState()
        {
            switch(mNextState)
            {
                case NextState.BACK:
                    mAnimator.SetBool("Back", true);
                    break;
                case NextState.DEBUG_MOV:
                    mAnimator.SetInteger("DebugMode", 1);
                    break;
                case NextState.DEBUG_SOUND:
                    mAnimator.SetInteger("DebugMode", 0);
                    break;
                case NextState.DEBUG_TEMP:
                    mAnimator.SetInteger("DebugMode", 2);
                    break;
                case NextState.HEAD_CONTROL:
                    mAnimator.SetInteger("DebugMode", 3);
                    break;
                case NextState.VALIDATE:
                    mAnimator.SetBool("ChangeState", true);
                    break;
                default:
                    break;
            }
        }

        private void ShowDebugSoundWindow()
        {
            Debug.Log("show sound window");
            mAnimatorParameter.SetTrigger("Close_WParameters");
            mNextState = NextState.DEBUG_SOUND;
            //mAnimator.SetInteger("DebugMode", 0);
            
        }

        private void ShowDebugMovementWindow()
        {
            Debug.Log("show mouv window");
            mAnimatorParameter.SetTrigger("Close_WParameters");
            mNextState = NextState.DEBUG_MOV;
            //mAnimator.SetInteger("DebugMode", 1);

        }

        private void ShowDebugTemperatureWindow()
        {
            Debug.Log("show temperature window");
            mAnimatorParameter.SetTrigger("Close_WParameters");
            mNextState = NextState.DEBUG_TEMP;
            //mAnimator.SetInteger("DebugMode", 2);

        }

        private void ShowHeadControllerWindow()
        {
            mAnimatorParameter.SetTrigger("Close_WParameters");
            StateManager.BackgroundAnimator.SetTrigger("Close_BG");
            mNextState = NextState.HEAD_CONTROL;
        }

        private void Validate()
        {
            //mAnimator.SetBool("ChangeState", true);
            StateManager.BackgroundAnimator.SetTrigger("Close_BG");
            mAnimatorParameter.SetTrigger("Close_WParameters");
            mNextState = NextState.VALIDATE;
        }

        private void Back()
        {
            //mAnimator.SetBool("Back", true);
            StateManager.BackgroundAnimator.SetTrigger("Close_BG");
            mAnimatorParameter.SetTrigger("Close_WParameters");
            mNextState = NextState.BACK;
        }

        private void SetDetectorsThreshold()
        {
            float lValue = 1.0f - (mParameters.SliderFire.value / mParameters.SliderFire.maxValue);
            lValue = 1.0f - (mParameters.SliderMovement.value / mParameters.SliderMovement.maxValue);
            mDetectionManager.MovementDetector.SetThreshold(lValue * mDetectionManager.MovementDetector.GetMaxThreshold());

            lValue = 1.0f - (mParameters.SliderSound.value / mParameters.SliderSound.maxValue);
            mDetectionManager.SoundDetector.SetThreshold(lValue * mDetectionManager.SoundDetector.GetMaxThreshold());

            lValue = 1.0f - (mParameters.SliderKidnap.value / mParameters.SliderKidnap.maxValue);
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