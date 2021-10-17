using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public class ParameterManager : MonoBehaviour
    {
        private TSlider mSliderVolume;
        private TSlider mSliderVolumeAgora;
        private TToggle mToggleNavigationStatic;
        private TToggle mToggleNavigationDynamic;
        private TToggle mToggleAllowPhoto;
        //private TToggle mToggleTouch;
        private TButton mButtonVerify;

        private RTCManager mRTCManager;
        private RTMManager mRTMManager;
        private Animator mAnimator;

        [SerializeField]
        private GameObject UIList;
        private float mLastValue = 200F;

        // Use this for initialization
        void Start()
        {
            Buddy.GUI.Header.OnClickParameters.Add(Lauchparameters);
            mRTCManager = GetComponent<RTCManager>();
            mRTMManager = GetComponent<RTMManager>();
            mAnimator = GetComponent<Animator>();
        }

        public void Lauchparameters()
        {
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                TText lTextVolume = iBuilder.CreateWidget<TText>();
                lTextVolume.SetLabel("Réglage du volume");
                mSliderVolume = iBuilder.CreateWidget<TSlider>();
                mSliderVolume.SlidingValue = Buddy.Actuators.Speakers.Volume * 100F;
                mSliderVolume.OnSlide.Add(UpdateVolume);


                TText lTextVolumeAgora = iBuilder.CreateWidget<TText>();
                lTextVolumeAgora.SetLabel("Réglage du volume d'appel");
                mSliderVolumeAgora = iBuilder.CreateWidget<TSlider>();
                mSliderVolumeAgora.MaxSlidingValue = 400F;
                mSliderVolumeAgora.MinSlidingValue = 0F;
                mSliderVolumeAgora.SlidingValue = mLastValue;
                mSliderVolumeAgora.OnSlide.Add(UpdateVolumeAgora);

                mToggleNavigationStatic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationStatic.SetLabel("Navigation Statique");
                mToggleNavigationStatic.ToggleValue = mRTMManager.mStaticSteering;
                mToggleNavigationStatic.OnToggle.Add(SetNavigationStatic);

                mToggleNavigationDynamic = iBuilder.CreateWidget<TToggle>();
                mToggleNavigationDynamic.SetLabel("Navigation Dynamique");
                mToggleNavigationDynamic.ToggleValue = !mRTMManager.mStaticSteering;
                mToggleNavigationDynamic.OnToggle.Add(SetNavigationDynamic);

                mToggleAllowPhoto = iBuilder.CreateWidget<TToggle>();
                mToggleAllowPhoto.SetLabel("Autoriser la prise de photo");
                mToggleAllowPhoto.ToggleValue = !mRTMManager.mAllowPhoto;
                mToggleAllowPhoto.OnToggle.Add(InformAllowPhoto);

                //mToggleTouch = iBuilder.CreateWidget<TToggle>();
                //mToggleTouch.SetLabel("Réaction aux caresses");
                //mToggleTouch.ToggleValue = true;
                //mToggleTouch.OnToggle.Add(SetTouchToggle);


                if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName("CALL")) {
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel("Utilisateur(s) associé(s)");

                    mButtonVerify = iBuilder.CreateWidget<TButton>();
                    mButtonVerify.SetLabel("Vérifier");
                    mButtonVerify.SetIcon(Buddy.Resources.Get<Sprite>("Atlas_Education_IconRefresh", Context.APP));
                    mButtonVerify.OnClick.Add(() => {
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

        private void UpdateVolume(float iValue)
        {
            float lValue = iValue / 100F;
            Buddy.Actuators.Speakers.Volume = lValue;
        }

        private void UpdateVolumeAgora(float iValue)
        {
            mRTCManager.SetSpeakerVolumeMax((int) iValue);
            mLastValue = iValue;
        }

        private void SetNavigationStatic(bool iValue)
        {
            if (iValue) {
                mRTMManager.SetStaticSteering(true);
                mToggleNavigationDynamic.ToggleValue = false;
            } else {
                mRTMManager.SetStaticSteering(false);
                mToggleNavigationDynamic.ToggleValue = true;

                Buddy.Actuators.Wheels.UnlockWheels();
            }
        }

        private void SetNavigationDynamic(bool iValue)
        {
            if (iValue) {
                mRTMManager.SetStaticSteering(false);
                mToggleNavigationStatic.ToggleValue = false;

                Buddy.Actuators.Wheels.UnlockWheels();
            } else {
                mRTMManager.SetStaticSteering(true);
                mToggleNavigationStatic.ToggleValue = true;
            }
        }

        private void InformAllowPhoto(bool iValue)
        {
            mRTMManager.InformPhotoAllowed();
        }

        //private void SetTouchToggle(bool iSetTouchToggle)
        //{
        //    mRTMManager.SetTouch(iSetTouchToggle);
        //}

    }
}