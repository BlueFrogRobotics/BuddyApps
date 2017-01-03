using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class ParametersGuardian : MonoBehaviour
    {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private InputField password;

        [SerializeField]
        private GaugeOnOff gaugeOnOffMovement;

        [SerializeField]
        private GaugeOnOff gaugeOnOffSound;

        [SerializeField]
        private GaugeOnOff gaugeOnOffFire;

        [SerializeField]
        private GaugeOnOff gaugeOnOffKidnap;

        [SerializeField]
        private UnityEngine.UI.Button buttonDebugSound;

        [SerializeField]
        private UnityEngine.UI.Button buttonDebugMovement;

        [SerializeField]
        private UnityEngine.UI.Button buttonDebugTemperature;

        [SerializeField]
        private BuddyOS.UI.Dropdown contactList;

        [SerializeField]
        private UnityEngine.UI.Button buttonHeadControl;

        [SerializeField]
        private UnityEngine.UI.Button buttonValidate;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        [SerializeField]
        private Text mTextFire;

        [SerializeField]
        private Text mTextSound;

        [SerializeField]
        private Text mTextMouv;

        [SerializeField]
        private Text mTextKidnap;

        [SerializeField]
        private Text mTextContact;

        [SerializeField]
        private Text mTextPassword;

        public InputField Password { get { return password; } }

        public Slider SliderMovement { get { return gaugeOnOffMovement.Slider; } }
        public Slider SliderSound { get { return gaugeOnOffSound.Slider; } }
        public Slider SliderFire { get { return gaugeOnOffFire.Slider; } }
        public Slider SliderKidnap { get { return gaugeOnOffKidnap.Slider; } }

        public UnityEngine.UI.Button ButtonDebugSound { get { return buttonDebugSound; } }
        public UnityEngine.UI.Button ButtonDebugMovement { get { return buttonDebugMovement; } }
        public UnityEngine.UI.Button ButtonDebugTemperature { get { return buttonDebugTemperature; } }
        public UnityEngine.UI.Button ButtonHeadControl { get { return buttonHeadControl; } }

        public UnityEngine.UI.Button ButtonValidate { get { return buttonValidate; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }

        private bool mHasInitCommands=false;
        private Dictionary mDictionnary;


        // Use this for initialization
        void Start()
        {
            mDictionnary = BYOS.Instance.Dictionary;
            gaugeOnOffFire.DisplayPercentage = true;
            gaugeOnOffKidnap.DisplayPercentage = true;
            gaugeOnOffMovement.DisplayPercentage = true;
            gaugeOnOffSound.DisplayPercentage = true;

            titleText.text = mDictionnary.GetString("paramTitle");
            mTextFire.text = mDictionnary.GetString("detectFire");
            mTextMouv.text = mDictionnary.GetString("detectMouv");
            mTextSound.text = mDictionnary.GetString("detectSound");
            mTextKidnap.text = mDictionnary.GetString("detectKidnap");
            mTextContact.text = mDictionnary.GetString("contact");
            mTextPassword.text = mDictionnary.GetString("password");
            buttonHeadControl.GetComponentInChildren<Text>().text= mDictionnary.GetString("headOrientation").ToUpper();

            //contactList.SetDefault("PERSONNE");
            contactList.AddOption(mDictionnary.GetString("nobody"), GuardianData.Contact.NOBODY);
            contactList.AddOption("RODOLPHE HASSELVANDER", GuardianData.Contact.RODOLPHE);
            contactList.AddOption("JEAN MICHEL MOURIER", GuardianData.Contact.J2M);
            contactList.AddOption("MAUD VERRAES", GuardianData.Contact.MAUD);
            contactList.AddOption("BENOIT PIRONNET", GuardianData.Contact.BENOIT);
            contactList.AddOption("MARC GOURLAN", GuardianData.Contact.MARC);
            contactList.AddOption("FRANCK DE VISME", GuardianData.Contact.FRANCK);
            contactList.AddOption("WALID ABDERRAHMANI", GuardianData.Contact.WALID);
            contactList.SetDefault(1);
            contactList.SetDefault(0);
            GuardianData.Instance.Recever = GuardianData.Contact.NOBODY;

        }

        // Update is called once per frame
        void Update()
        {
            if(!mHasInitCommands)
            {
                //contactList.SetDefault("PERSONNE");
                mHasInitCommands = true;
                gaugeOnOffFire.SwitchCommands.Add(new ActFireDetectionCmd());
                gaugeOnOffMovement.SwitchCommands.Add(new ActMovementDetectionCmd());
                gaugeOnOffKidnap.SwitchCommands.Add(new ActKidnappingDetectionCmd());
                gaugeOnOffSound.SwitchCommands.Add(new ActSoundDetectionCmd());
                contactList.UpdateCommands.Add(new ContactGuardianCmd());
            }
        }

        void OnEnable()
        {
            GuardianData.Instance.FireDetectionIsActive = gaugeOnOffFire.IsActive;
            GuardianData.Instance.MovementDetectionIsActive = gaugeOnOffMovement.IsActive;
            GuardianData.Instance.KidnappingDetectionIsActive = gaugeOnOffKidnap.IsActive;
            GuardianData.Instance.SoundDetectionIsActive = gaugeOnOffSound.IsActive;
        }
    }
}