using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.CoursTelepresence
{

    public class ParameterManager : MonoBehaviour
    {
        private TSlider mSliderVolume;
        private TToggle mToggleNavigationStatic; 
        private TToggle mToggleNavigationDynamic;
        private TButton mButtonVerify;

        private RTMManager mRTMManager;
        private Animator mAnimator;

        // Use this for initialization
        void Start()
        {
            Buddy.GUI.Header.OnClickParameters.Add(Lauchparameters);
            mRTMManager = GetComponent<RTMManager>();
            mAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Lauchparameters()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                TText lTextVolume = iBuilder.CreateWidget<TText>();
                lTextVolume.SetLabel("Réglage du volume");
                mSliderVolume = iBuilder.CreateWidget<TSlider>();
                mSliderVolume.SlidingValue = Buddy.Actuators.Speakers.Volume * 100F;
                mSliderVolume.OnSlide.Add(UpdateVolume);

                mToggleNavigationStatic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationStatic.SetLabel("Navigation Statique");
                mToggleNavigationStatic.ToggleValue = mRTMManager.mStaticSteering;
                mToggleNavigationStatic.OnToggle.Add(SetNavigationStatic);

                mToggleNavigationDynamic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationDynamic.SetLabel("Navigation Dynamique");
                mToggleNavigationDynamic.ToggleValue = !mRTMManager.mStaticSteering;
                mToggleNavigationDynamic.OnToggle.Add(SetNavigationDynamic);


                if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName("CALL"))
                {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel("Utilisateur(s) associé(s)");

                    mButtonVerify = iBuilder.CreateWidget<TButton>();
                    mButtonVerify.SetLabel("Vérifier");
                    mButtonVerify.SetIcon(Buddy.Resources.Get<Sprite>("Atlas_Education_IconRefresh", Context.APP));


                    mButtonVerify.OnClick.Add(() =>
                    {
                        DBManager.Instance.IsRefreshButtonPushed = true;
                        CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
                        DBManager.Instance.StartDBManager();
                        StartCoroutine(DBManager.Instance.RefreshPlanning());
                        CloseParameters();
                        //mAnimator.Play("CONNECTING");
                    });
                }
            },
            () => { CloseParameters(); }, Buddy.Resources.Get<Sprite>("os_icon_close", Context.OS),
            () => { SaveParam(); CloseParameters(); }, Buddy.Resources.Get<Sprite>("os_icon_check", Context.OS)
           );

            Buddy.GUI.Header.OnClickParameters.Clear();
            Buddy.GUI.Header.OnClickParameters.Add(CloseParameters);
        }

        private void CloseParameters()
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.OnClickParameters.Clear();
            Buddy.GUI.Header.OnClickParameters.Add(Lauchparameters);
        }

        public void UpdateVolume(float iValue)
        {
            float lValue = iValue / 100F;
            //if (Mathf.Abs(Buddy.Actuators.Speakers.Volume - lValue) > 0.05)
            //{
                Debug.Log("PRE Volume set to " + Buddy.Actuators.Speakers.Volume);
                Debug.Log("PRE slider set to " + lValue);
                Buddy.Actuators.Speakers.Volume = lValue;
                //if (!Buddy.Actuators.Speakers.IsBusy)
                //    Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_1);
                Debug.Log("POST Volume set to " + Buddy.Actuators.Speakers.Volume);
            //}
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