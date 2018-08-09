using UnityEngine;
using System.Collections;
using BlueQuark;

namespace BuddyApp.Guardian
{
    public class DebugThermicState : AStateMachineBehaviour
    {
        private Animator mDebugTempAnimator;
        private ShowTemperature mShowTemperature;
        private bool mGoBack = false;
        private bool mHasDetectedFire = false;
        private bool mHasOpenedWindow = false;
        private float mTimer;
        private ThermalDetector mThermalDetection;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mThermalDetection.OnDetect.AddP(OnFireDetected, 50);
           // Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            
            mShowTemperature = GetGameObject(StateObject.DEBUG_FIRE).GetComponent<ShowTemperature>();
            mDebugTempAnimator = mShowTemperature.gameObject.GetComponent<Animator>();
            mShowTemperature.ButtonBack.onClick.AddListener(GoBack);
            mGoBack = false;
            mHasDetectedFire = false;
            mHasOpenedWindow = false;
            mTimer = 0.0f;

			Buddy.Vocal.SayKey("thermaldebug");

		}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mHasOpenedWindow && mTimer > 1.3f)
            {
                mTimer = 0.0f;
                mHasOpenedWindow = true;
                mDebugTempAnimator.SetTrigger("Open_WDebugs");
            }
            else if (mHasOpenedWindow)
            {
                int[] lMatArrayFromFrame;
                int[] lThermicMatrix;
                for (int i = 0; i < Buddy.Sensors.ThermalCamera.Frame.width() * Buddy.Sensors.ThermalCamera.Frame.cols(); ++i)
                {
                    //lThermicMatrix[i] = 
                }
                //int[] lThermicMatrix = Buddy.Sensors.ThermalCamera.Frame;
                mShowTemperature.FillTemperature(lThermicMatrix);
                mShowTemperature.UpdateTexture();
                if (mDebugTempAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
                {
                    animator.SetInteger("DebugMode", -1);
                    mGoBack = false;
                    mShowTemperature.IcoFire.enabled = false;
                }
                CheckFireDetection();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mShowTemperature.IcoFire.enabled = false;
            GuardianData.Instance.FirstRunParam = false;
            mShowTemperature.ButtonBack.onClick.RemoveAllListeners();
            mThermalDetection.OnDetect.RemoveP(OnFireDetected);
            Buddy.Vocal.Stop();
            //Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
        }

        private void GoBack()
        {
            mDebugTempAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private bool OnFireDetected(ObjectEntity[] iObject)
        {
            mHasDetectedFire = true;
            return mHasDetectedFire;
        }

        private void CheckFireDetection()
        {
            if (mHasDetectedFire)
            {
                Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
                mHasDetectedFire = false;
                mShowTemperature.IcoFire.enabled = true;
            }
            else
                mShowTemperature.IcoFire.enabled = false;
        }

    }
}