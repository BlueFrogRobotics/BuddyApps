using UnityEngine;
using System.Collections;
using BuddyOS.UI;
using BuddyOS;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneParameters : MonoBehaviour
    {
        [SerializeField]
        private TextField babyName;
        [SerializeField]
        private Dropdown contactSelection;

        [SerializeField]
        private GaugeOnOff lullabyVolume;

        [SerializeField]
        private Dropdown lullabySelection;

        [SerializeField]
        private GaugeOnOff animationLight;

        //[SerializeField]
        //private Button AnimationPlayPause;

        [SerializeField]
        private Dropdown animationSelection;

        [SerializeField]
        private Gauge microphone;

        [SerializeField]
        private Gauge timeBeforContact;

        [SerializeField]
        private Dropdown ifBabyCry;

        [SerializeField]
        private GaugeOnOff screenSaver;

        [SerializeField]
        private OnOff canRobotMove;

        //[SerializeField]
        //private OnOff saveSettings;

        private BabyPhoneData mBabyPhoneData;
        private Dictionary mDictionary;

        void Start()
        {
            mBabyPhoneData = BabyPhoneData.Instance;
            mDictionary = BYOS.Instance.Dictionary;

            //contact
            contactSelection.AddOption("NONE", BabyPhoneData.Contact.RODOLPHE);
            contactSelection.AddOption("JEAN MICHEL MOURIER", BabyPhoneData.Contact.J2M);
            contactSelection.AddOption("MAUD VERRAES", BabyPhoneData.Contact.MAUD);
            contactSelection.SetDefault(1);
            contactSelection.SetDefault(0);

            //volume
            lullabyVolume.DisplayPercentage = true;
            lullabyVolume.Slider.minValue = 0;
            lullabyVolume.Slider.maxValue = 100;
            lullabyVolume.Slider.value = BabyPhoneData.Instance.LullabyVolume;
            lullabyVolume.UpdateCommands.Add(new SetLullabyVolCmd());

            //volume On/off
            lullabyVolume.IsActive = BabyPhoneData.Instance.IsVolumeOn;
            lullabyVolume.SwitchCommands.Add(new ActVolCmd());

            //lullaby selection

            //animation's light
            animationLight.DisplayPercentage = true;
            animationLight.Slider.minValue = 0;
            animationLight.Slider.maxValue = 100;
            animationLight.Slider.value = BabyPhoneData.Instance.AnimationLight;
            animationLight.UpdateCommands.Add(new SetAnimLightCmd());

            //animation's light On/Off
            animationLight.IsActive = BabyPhoneData.Instance.IsAnimationOn;
            animationLight.SwitchCommands.Add(new ActVolCmd());

            //animation selection

            //microphone
            microphone.DisplayPercentage = true;
            microphone.Slider.minValue = 0;
            microphone.Slider.maxValue = 100;
            microphone.Slider.value = BabyPhoneData.Instance.AnimationLight;
            microphone.UpdateCommands.Add(new SetMicroSensCmd());

            //time befor contact
            timeBeforContact.Slider.minValue = 0;
            timeBeforContact.Slider.maxValue = 30; // 30 min
            timeBeforContact.Slider.value = BabyPhoneData.Instance.AnimationLight;
            timeBeforContact.UpdateCommands.Add(new SetMicroSensCmd());

            //if baby cries choice 

            //screen saver
            screenSaver.DisplayPercentage = true;
            screenSaver.Slider.minValue = 0;
            screenSaver.Slider.maxValue = 100;
            screenSaver.Slider.value = BabyPhoneData.Instance.ScreenSaverLight;
            screenSaver.UpdateCommands.Add(new SetScreenSaverLightCmd());

            //screen saver On/off
            screenSaver.IsActive = BabyPhoneData.Instance.IsScreanSaverOn;
            screenSaver.SwitchCommands.Add(new ActScreenSaverCmd());

            //robot mobility On/off
            canRobotMove.IsActive = BabyPhoneData.Instance.IsMotionOn;
            canRobotMove.SwitchCommands.Add(new ActRobotMotionCmd());

            //save settings : le widget n'est pas encore implémenté
        }

        public void Labelize()
        {
            // a appeler 
            //lullabyVolume.Label.text = mDictionary.GetString("vol");
        }

        //void Update()
        //{
        //string ContactName;
        //float Volume;
        //bool IsVolumeOn;
        //string LullabyToPlay;
        //float ScreenLightLevelForAnimation;
        //bool IsAnimationOn;
        //string AnimationToPlay;
        //float MicrophoneSensitivity;
        //int TimeBeforSartListening;
        //float ScreenSaver;
        //bool IsScreanSaverOn;
        //bool IsMobilityOn;
        //bool DoSaveSetting;

        //mBabyPhoneData.Volume = Volume.value;
        //mBabyPhoneData.IsVolumeOn = VolumeOnOff.
        //}
    }
}
