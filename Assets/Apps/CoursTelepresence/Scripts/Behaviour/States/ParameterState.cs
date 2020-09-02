using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.CoursTelepresence
{
    public class ParameterState : AStateMachineBehaviour
    {

        private TSlider mSliderVolume; 
        private TToggle mToggleNavigationStatic;
        private TToggle mToggleNavigationDynamic;
        private TButton mButtonVerify;

        private RTMManager mRTMManager;

        override public void Start()
        {
            mRTMManager = GetComponent<RTMManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CoursTelepresenceData.Instance.CurrentState = CoursTelepresenceData.States.PARAMETER_STATE;
            Buddy.GUI.Header.DisplayParametersButton(true);
            Buddy.GUI.Header.OnClickParameters.Add(() => { TriggerState("IDLE"); });

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TText lTextVolume = iBuilder.CreateWidget<TText>();
                lTextVolume.SetLabel("Réglage du volume");
                mSliderVolume = iBuilder.CreateWidget<TSlider>();
                mSliderVolume.SlidingValue = Buddy.Actuators.Speakers.Volume*100F;
                mSliderVolume.OnSlide.Add(UpdateVolume);

                mToggleNavigationStatic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationStatic.SetLabel("Navigation Statique");
                mToggleNavigationStatic.ToggleValue = mRTMManager.mStaticSteering;
                mToggleNavigationStatic.OnToggle.Add(SetNavigationStatic);

                mToggleNavigationDynamic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationDynamic.SetLabel("Navigation Dynamique");
                mToggleNavigationDynamic.ToggleValue = !mRTMManager.mStaticSteering;
                mToggleNavigationDynamic.OnToggle.Add(SetNavigationDynamic);

                TText lText = iBuilder.CreateWidget<TText>();
                lText.SetLabel("Utilisateur(s) associé(s)");
                mButtonVerify = iBuilder.CreateWidget<TButton>();
                mButtonVerify.SetLabel("Vérifier");
                mButtonVerify.SetIcon(Buddy.Resources.Get<Sprite>("Atlas_Education_IconRefresh", Context.APP));
                

                mButtonVerify.OnClick.Add(() => {
                    DBManager.Instance.StartDBManager();
                    StartCoroutine(DBManager.Instance.RefreshPlanning());
                    //Debug.LogError("<color=blue>VERIF : " + DBManager.Instance.ListUserStudent[0].Nom + "</color>");
                    
                    //Debug.LogError("<color=blue>VERIF 2 : " + DBManager.Instance.ListUIDTablet[0] + "</color>");
                    
                    Trigger("IDLE");/*Debug.LogError("<color=blue>PARAM STATE CLICK VERIF </color>"); Trigger("CONNECTING");*/
                });
            },
            () => { Trigger("IDLE"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.Get<Sprite>("os_icon_close", Context.OS),
            () => {  SaveParam(); Trigger("IDLE"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.Get<Sprite>("os_icon_check", Context.OS)
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

        private void SaveParam()
        {
            
        }

        private void SetNavigationStatic(bool iValue)
        {
            if (iValue)
            {
                mRTMManager.SwapSteering(true);
                mToggleNavigationDynamic.ToggleValue = false;
            }
            else
            {
                mRTMManager.SwapSteering(false);
                mToggleNavigationDynamic.ToggleValue = true;
            }
        }

        private void SetNavigationDynamic(bool iValue)
        {
            if (iValue)
            {
                mRTMManager.SwapSteering(false);
                mToggleNavigationStatic.ToggleValue = false;
            }
            else
            {
                mRTMManager.SwapSteering(true);
                mToggleNavigationStatic.ToggleValue = true;
            }
        }
    }
}