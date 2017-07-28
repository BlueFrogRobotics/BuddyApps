using Buddy.UI;
using Buddy.Command;
using Buddy;
using System;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public class GuardianLayout : AWindowLayout
    {
        private LabeledButton mHeadOrientation;
        private GaugeOnOff mMovementDetection;
        private LabeledButton mMovementDebug;
        private GaugeOnOff mSoundDetection;
        private LabeledButton mSoundDebug;
        private OnOff mFireDetection;
        private LabeledButton mFireDebug;
        private OnOff mKidnappingDetection;
        private Dropdown mContacts;

        public override void Build()
        {
            Title = BYOS.Instance.Dictionary.GetString("detectionparameters");
            mHeadOrientation = CreateWidget<LabeledButton>();
            mMovementDetection = CreateWidget<GaugeOnOff>();
            mMovementDebug = CreateWidget<LabeledButton>();
            mSoundDetection = CreateWidget<GaugeOnOff>();
            mSoundDebug = CreateWidget<LabeledButton>();
            mFireDetection = CreateWidget<OnOff>();
            mFireDebug = CreateWidget<LabeledButton>();
            mKidnappingDetection = CreateWidget<OnOff>();
            mContacts = CreateWidget<Dropdown>();

            mMovementDetection.IsActive = GuardianData.Instance.MovementDetection;
            mMovementDetection.DisplayPercentage = true;
            mMovementDetection.Slider.wholeNumbers = true;
            mMovementDetection.Slider.value = GuardianData.Instance.MovementDetectionThreshold;

            mSoundDetection.IsActive = GuardianData.Instance.SoundDetection;
            mSoundDetection.DisplayPercentage = true;
            mSoundDetection.Slider.wholeNumbers = true;
            mSoundDetection.Slider.value = GuardianData.Instance.SoundDetectionThreshold;

            mFireDetection.IsActive = GuardianData.Instance.FireDetection;
            mKidnappingDetection.IsActive = GuardianData.Instance.KidnappingDetection;

            mHeadOrientation.OnClickEvent(() => { GuardianData.Instance.HeadOrientation = true; });
            mMovementDebug.OnClickEvent(() => { GuardianData.Instance.MovementDebug = true; });
            mSoundDebug.OnClickEvent(() => { GuardianData.Instance.SoundDebug = true; });
            mFireDebug.OnClickEvent(() => { GuardianData.Instance.FireDebug = true; });

            mMovementDetection.OnSwitchEvent((bool iVal) => {
                GuardianData.Instance.MovementDetection = iVal;
            });

            mMovementDetection.OnUpdateEvent((int iVal) => {
                GuardianData.Instance.MovementDetectionThreshold = iVal;
            });

            mSoundDetection.OnSwitchEvent((bool iVal) => {
                GuardianData.Instance.SoundDetection = iVal;
            });

            mSoundDetection.OnUpdateEvent((int iVal) => {
                GuardianData.Instance.SoundDetectionThreshold = iVal;
            });

            mFireDetection.OnSwitchEvent((bool iVal) => {
                GuardianData.Instance.FireDetection = iVal;
            });

            mKidnappingDetection.OnSwitchEvent((bool iVal) => {
                GuardianData.Instance.KidnappingDetection = iVal;
            });

            foreach (GuardianData.Contacts lContact in Enum.GetValues(typeof(GuardianData.Contacts)))
                mContacts.AddOption(lContact.ToString(), lContact);

            mContacts.SetDefault((int)GuardianData.Instance.Contact);

            mContacts.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) => {
                GuardianData.Instance.Contact = (GuardianData.Contacts)iAttachedObj;
            });
        }

        public override void LabelizeWidgets()
        {
            mHeadOrientation.OuterLabel = BYOS.Instance.Dictionary.GetString("headorientation");
            mHeadOrientation.InnerLabel = BYOS.Instance.Dictionary.GetString("changeheadorientation");
            mMovementDebug.OuterLabel = "debug mouvement";
            mMovementDebug.InnerLabel = "debug";
            mFireDebug.OuterLabel = "debug thermique";
            mFireDebug.InnerLabel = "debug";
            mSoundDebug.OuterLabel = "debug son";
            mSoundDebug.InnerLabel = "debug";
            mMovementDetection.Label = BYOS.Instance.Dictionary.GetString("movementdetection");
            mFireDetection.Label = BYOS.Instance.Dictionary.GetString("firedetection");
            mKidnappingDetection.Label = BYOS.Instance.Dictionary.GetString("kidnappingdetection");
            mSoundDetection.Label = BYOS.Instance.Dictionary.GetString("sounddetection");
            mContacts.Label = BYOS.Instance.Dictionary.GetString("whotocontact");
        }

        public override void Update()
        {
            //if (!IsDisplayed)
                Debug.Log("Desactive");
        }
    }
}