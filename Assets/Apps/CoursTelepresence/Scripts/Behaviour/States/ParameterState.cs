using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.CoursTelepresence
{
    public class ParameterState : AStateMachineBehaviour
    {

        private TSlider mSliderVolume; 
        private TToggle mToggleNavigation;
        private TButton mButtonVerify;

        private RTMManager mRTMManager;

        override public void Start()
        {
            mRTMManager = GetComponent<RTMManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(true);
            Buddy.GUI.Header.OnClickParameters.Add(() => { TriggerState("IDLE"); });

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                mSliderVolume = iBuilder.CreateWidget<TSlider>();
                mSliderVolume.SlidingValue = Buddy.Actuators.Speakers.Volume*100F;
                mSliderVolume.OnSlide.Add(UpdateVolume);
                
                mToggleNavigation = iBuilder.CreateWidget<TToggle>();
                mToggleNavigation.SetLabel("Navigation Statique/Dynamique");
                mToggleNavigation.ToggleValue = mRTMManager.mStaticSteering;
                mToggleNavigation.OnToggle.Add(mRTMManager.SwapSteering);

                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetLabel("Utilisateur(s) associé(s)");
                mButtonVerify = iBuilder.CreateWidget<TButton>();
                mButtonVerify.SetLabel("Vérifier");
                mButtonVerify.SetIcon(Buddy.Resources.Get<Sprite>("Atlas_Education_IconRefresh", Context.APP));
                

                mButtonVerify.OnClick.Add(() => { Trigger("CONNECTING"); });
            }
           );
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.OnClickParameters.Clear();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
        }

        public void UpdateVolume(float iValue)
        {
            float lValue = iValue / 100F;
            if (Mathf.Abs(Buddy.Actuators.Speakers.Volume - lValue) > 0.05)
            {
                Debug.Log("PRE Volume set to " + Buddy.Actuators.Speakers.Volume);
                Debug.Log("PRE slider set to " + lValue);
                Buddy.Actuators.Speakers.Volume = lValue;
                //if (!Buddy.Actuators.Speakers.IsBusy)
                //    Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
                Debug.Log("POST Volume set to " + Buddy.Actuators.Speakers.Volume);
            }
        }
    }
}