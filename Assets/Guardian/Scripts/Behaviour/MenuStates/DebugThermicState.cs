using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.Guardian
{
    public class DebugThermicState : AStateMachineBehaviour
    {
        private Animator mDebugTempAnimator;
        private ShowTemperature mShowTemperature;
        private Animator mAnimator;
        private FireDetector mFireDetector;
        private bool mGoBack = false;
        private bool mHasDetectedFire = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //SetWindowAppOverBuddyColor(1);
            mFireDetector = BYOS.Instance.Perception.FireDetector;
            mFireDetector.OnDetection += OnFireDetected;
            mShowTemperature = GetGameObject(StateObject.DEBUG_FIRE).GetComponent<ShowTemperature>();
            mDebugTempAnimator = mShowTemperature.gameObject.GetComponent<Animator>();
            mDebugTempAnimator.SetTrigger("Open_WDebugs");
            //mShowTemperature.gameObject.SetActive(true);
            mAnimator = animator;
            mShowTemperature.ButtonBack.onClick.AddListener(GoBack);
            mGoBack = false;
            mHasDetectedFire = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //mShowTemperature.FillTemperature(null);
            Debug.Log("hum");
            mShowTemperature.UpdateTexture();
            Debug.Log("hum2");
            if (mDebugTempAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
            {
                Debug.Log("fin debug temp state");
                mAnimator.SetInteger("DebugMode", -1);
                mGoBack = false;
                mShowTemperature.IcoFire.enabled = false;
            }
            if (mHasDetectedFire)
            {
                //StateManager.PlayBeep();
                BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                mHasDetectedFire = false;
                mShowTemperature.IcoFire.enabled = true;
            }
            else
                mShowTemperature.IcoFire.enabled = false;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mShowTemperature.IcoFire.enabled = false;
            mShowTemperature.ButtonBack.onClick.RemoveAllListeners();
            mFireDetector.OnDetection -= OnFireDetected;
            //mShowTemperature.gameObject.SetActive(false);
        }

        private void GoBack()
        {
            mDebugTempAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private void OnFireDetected()
        {
            mHasDetectedFire = true;
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