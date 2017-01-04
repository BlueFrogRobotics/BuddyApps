using UnityEngine;
using BuddyOS.UI;
using BuddyOS;
using System.Collections.Generic;

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
        private GaugeOnOff animationBrightness;

        [SerializeField]
        private Dropdown animationSelection;

        [SerializeField]
        private GaugeOnOff soundDetection;

        [SerializeField]
        private GaugeOnOff motionDetection;

        [SerializeField]
        private Gauge timeBeforContact;

        [SerializeField]
        private Dropdown ifBabyCry;

        //[SerializeField]
        //private GaugeOnOff screenSaver;

        [SerializeField]
        private OnOff canRobotMove;

        [SerializeField]
        private OnOff saveSettings;

        private Dictionary mDictionary;
        private AudioClip[] mLullabies;
        private List<string> mLullabyName;
        private int mLullabyIndice;

        void OnEnable()
        {
            mDictionary = BYOS.Instance.Dictionary;
        }

        void OnDisable()
        {
            Debug.Log("seuil ecoute" + BabyPhoneData.Instance.MicrophoneSensitivity);
        }

        void Start()
        {
            Init();
            Labelize();
        }

        public void Labelize()
        {
            babyName.Label.text = mDictionary.GetString("mybb");
            contactSelection.Label.text = "CONTACT";
            lullabyVolume.Label.text = (mDictionary.GetString("vol")).ToUpper();
            lullabySelection.Label.text = mDictionary.GetString("lull");
            animationBrightness.Label.text = mDictionary.GetString("bright");
            animationSelection.Label.text = mDictionary.GetString("anim");
            soundDetection.Label.text = mDictionary.GetString("soundetect");
            motionDetection.Label.text = mDictionary.GetString("motiondetect");
            timeBeforContact.Label.text = mDictionary.GetString("timebefor");
            ifBabyCry.Label.text = mDictionary.GetString("ifbb");
            //screenSaver.Label.text = mDictionary.GetString("saver");
            canRobotMove.Label.text = mDictionary.GetString("mob");
            saveSettings.Label.text = mDictionary.GetString("savesett");
        }

        public void Init()
        {
            // baby name
            babyName.UpdateCommands.Add(new SetBabyNameCmd());

            ////contact
            contactSelection.AddOption("DEFAULT", BabyPhoneData.Contact.DEFAULT); //mDictionary.GetString("default")
            contactSelection.AddOption("RODOLPHE", BabyPhoneData.Contact.RODOLPHE);
            contactSelection.AddOption("JEAN MICHEL MOURIER", BabyPhoneData.Contact.J2M);
            contactSelection.AddOption("MAUD VERRAES", BabyPhoneData.Contact.MAUD);
            contactSelection.AddOption("BENOIT PIRONNET", BabyPhoneData.Contact.BENOIT);
            contactSelection.AddOption("MARC GOURLAN", BabyPhoneData.Contact.MARC);
            contactSelection.AddOption("KARAMA GUIMBAL", BabyPhoneData.Contact.KARAMA);
            contactSelection.SetDefault((int)BabyPhoneData.Instance.Recever);
            contactSelection.UpdateCommands.Add(new ContactBabyPhoneCmd());

            ////volume
            lullabyVolume.DisplayPercentage = true;
            lullabyVolume.Slider.minValue = 0;
            lullabyVolume.Slider.maxValue = 100;
            lullabyVolume.Slider.value = BabyPhoneData.Instance.LullabyVolume;
            lullabyVolume.UpdateCommands.Add(new SetLullabyVolCmd());

            ////volume On/off
            lullabyVolume.IsActive = BabyPhoneData.Instance.IsVolumeOn;
            lullabyVolume.SwitchCommands.Add(new ActVolCmd());

            ////lullaby selection
            lullabySelection.AddOption(mDictionary.GetString("lull0"), BabyPhoneData.Lullaby.LULL_0); //mDictionary.GetString("default")
            lullabySelection.AddOption(mDictionary.GetString("lull1"), BabyPhoneData.Lullaby.LULL_1);
            lullabySelection.AddOption(mDictionary.GetString("lull2"), BabyPhoneData.Lullaby.LULL_2);
            lullabySelection.AddOption(mDictionary.GetString("lull3"), BabyPhoneData.Lullaby.LULL_3);
            lullabySelection.AddOption(mDictionary.GetString("lull4"), BabyPhoneData.Lullaby.LULL_4);
            lullabySelection.SetDefault((int)BabyPhoneData.Instance.LullabyToPlay);
            lullabySelection.UpdateCommands.Add(new LullabyBabyPhoneCmd());

            ////animation's light
            animationBrightness.DisplayPercentage = true;
            animationBrightness.Slider.minValue = 0;
            animationBrightness.Slider.maxValue = 100;
            animationBrightness.Slider.value = BabyPhoneData.Instance.AnimationLight;
            animationBrightness.UpdateCommands.Add(new SetAnimLightCmd());

            ////animation's light On/Off
            animationBrightness.IsActive = BabyPhoneData.Instance.IsAnimationOn;
            animationBrightness.SwitchCommands.Add(new ActAnimCmd());

            ////animation selection
            animationSelection.AddOption(mDictionary.GetString("owl"), BabyPhoneData.Animation.OWL);
            animationSelection.AddOption(mDictionary.GetString("chris"), BabyPhoneData.Animation.CHRISTMAS);
            animationSelection.SetDefault((int)BabyPhoneData.Instance.AnimationToPlay);
            animationSelection.UpdateCommands.Add(new AnimBabyPhoneCmd());

            ////time befor contact
            timeBeforContact.DisplayPercentage = false;
            timeBeforContact.Suffix = " MIN";
            timeBeforContact.Slider.minValue = 0;
            timeBeforContact.Slider.maxValue = 10; // 30 min
            timeBeforContact.Slider.value = BabyPhoneData.Instance.TimeBeforContact;
            timeBeforContact.UpdateCommands.Add(new SetTimeBeforContactCmd());

            ////if baby cries choice 
            ifBabyCry.AddOption(mDictionary.GetString("noact"), BabyPhoneData.Action.DEFAULT_ACTION);
            ifBabyCry.AddOption(mDictionary.GetString("playlul"), BabyPhoneData.Action.REPLAY_LULLABY);
            ifBabyCry.AddOption(mDictionary.GetString("playanim"), BabyPhoneData.Action.REPLAY_ANIMATION);
            ifBabyCry.AddOption(mDictionary.GetString("playboth"), BabyPhoneData.Action.REPLAY_BOTH);
            ifBabyCry.SetDefault((int)BabyPhoneData.Instance.ActionWhenBabyCries);
            ifBabyCry.UpdateCommands.Add(new IfBabyCriesCmd());

            ////sound detection
            soundDetection.DisplayPercentage = true;
            soundDetection.Slider.minValue = 0;
            soundDetection.Slider.maxValue = 10;
            soundDetection.Slider.value = BabyPhoneData.Instance.MicrophoneSensitivity;
            soundDetection.UpdateCommands.Add(new SetMicroSensCmd());

            ////animation's light On/Off
            soundDetection.IsActive = BabyPhoneData.Instance.IsSoundDetectionOn;
            soundDetection.SwitchCommands.Add(new ActSoundDetectionCmd());

            ////sound detection
            motionDetection.DisplayPercentage = true;
            motionDetection.Slider.minValue = 20;
            motionDetection.Slider.maxValue = 200;
            motionDetection.Slider.value = BabyPhoneData.Instance.CameraSensitivity;
            motionDetection.UpdateCommands.Add(new SetCamSensCmd());

            ////animation's light On/Off
            motionDetection.IsActive = BabyPhoneData.Instance.IsMotionDetectionOn;
            motionDetection.SwitchCommands.Add(new ActMotionDetectionCmd());

            //////screen saver
            //screenSaver.DisplayPercentage = true;
            //screenSaver.Slider.minValue = 0;
            //screenSaver.Slider.maxValue = 100;
            //screenSaver.Slider.value = BabyPhoneData.Instance.ScreenSaverLight;
            //screenSaver.UpdateCommands.Add(new SetScreenSaverLightCmd());

            //////screen saver On/off
            //screenSaver.IsActive = BabyPhoneData.Instance.IsScreanSaverOn;
            //screenSaver.SwitchCommands.Add(new ActScreenSaverCmd());

            ////robot mobility On/off
            canRobotMove.IsActive = BabyPhoneData.Instance.IsMotionOn;
            canRobotMove.SwitchCommands.Add(new ActRobotMotionCmd());

            ////save settings : le widget n'est pas encore implémenté
            saveSettings.IsActive = BabyPhoneData.Instance.DoSaveSetting;
            saveSettings.SwitchCommands.Add(new ActSaveSettingsCmd());
        }

        void Update()
        {
            if(soundDetection.Slider.value != BabyPhoneData.Instance.MicrophoneSensitivity)
                soundDetection.Slider.value = BabyPhoneData.Instance.MicrophoneSensitivity;
        }
        


    }
}
