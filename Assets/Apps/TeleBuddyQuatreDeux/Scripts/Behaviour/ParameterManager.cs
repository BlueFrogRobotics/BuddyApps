using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public class ParameterManager : MonoBehaviour
    {
        private TSlider mSliderVolume;
        private TToggle mToggleNavigationStatic; 
        private TToggle mToggleNavigationDynamic;
        private TButton mButtonVerify;

        private RTMManager mRTMManager;
        private Animator mAnimator;

        [SerializeField]
        private GameObject UIList;

        // Use this for initialization
        void Start()
        {
            Buddy.GUI.Header.OnClickParameters.Add(Lauchparameters);
            mRTMManager = GetComponent<RTMManager>();
            mAnimator = GetComponent<Animator>();
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
                        DBManager.Instance.StartDBManager();
                        mAnimator.SetTrigger("CONNECTING");
                        if (UIList.activeSelf)
                            UIList.SetActive(false);
                        DBManager.Instance.IsRefreshButtonPushed = false;
                        if (TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem != ConnectivityProblem.LaunchDatabase)
                            TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
                        CloseParameters();
                    });
                }
            },
            () => { CloseParameters(); }, Buddy.Resources.Get<Sprite>("os_icon_close", Context.OS),
            () => { CloseParameters(); }, Buddy.Resources.Get<Sprite>("os_icon_check", Context.OS)
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
            Buddy.Actuators.Speakers.Volume = lValue;
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