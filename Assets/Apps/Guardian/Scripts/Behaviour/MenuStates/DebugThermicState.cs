using UnityEngine;
using System.Collections;
using Buddy;

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
        private ThermalDetection mThermalDetection;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mThermalDetection = BYOS.Instance.Perception.Thermal;
            mThermalDetection.OnDetect(OnFireDetected, 50);
           // Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            
            mShowTemperature = GetGameObject(StateObject.DEBUG_FIRE).GetComponent<ShowTemperature>();
            mDebugTempAnimator = mShowTemperature.gameObject.GetComponent<Animator>();
            mShowTemperature.ButtonBack.onClick.AddListener(GoBack);
            mGoBack = false;
            mHasDetectedFire = false;
            mHasOpenedWindow = false;
            mTimer = 0.0f;

			Interaction.TextToSpeech.SayKey("thermaldebug");

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
                int[] lThermicMatrix = Primitive.ThermalSensor.MatrixArray;
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
            mThermalDetection.StopOnDetect(OnFireDetected);
            Interaction.TextToSpeech.Stop();
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
                BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                mHasDetectedFire = false;
                mShowTemperature.IcoFire.enabled = true;
            }
            else
                mShowTemperature.IcoFire.enabled = false;
        }

    }
}