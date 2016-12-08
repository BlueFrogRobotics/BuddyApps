using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class ParametersGuardian : MonoBehaviour
    {

        //[SerializeField]
        //private OnOff onOffMovement;

        //[SerializeField]
        //private OnOff onOffSound;

        //[SerializeField]
        //private OnOff onOffFire;

        //[SerializeField]
        //private OnOff onOffKidnap;

        [SerializeField]
        private InputField password;

        //[SerializeField]
        //private Gauge gaugeMovement;

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
        private UnityEngine.UI.Button buttonValidate;

        [SerializeField]
        private UnityEngine.UI.Button buttonBack;

        //public OnOff OnOffMovement { get { return gaugeOnOffMovement; } }
        //public OnOff OnOffSound { get { return onOffSound; } }
        //public OnOff OnOffFire { get { return onOffFire; } }
        //public OnOff OnOffKidnap { get { return onOffKidnap; } }
        public InputField Password { get { return password; } }

        public Slider SliderMovement { get { return gaugeOnOffMovement.Slider; } }
        public Slider SliderSound { get { return gaugeOnOffSound.Slider; } }
        public Slider SliderFire { get { return gaugeOnOffFire.Slider; } }
        public Slider SliderKidnap { get { return gaugeOnOffKidnap.Slider; } }

        public UnityEngine.UI.Button ButtonDebugSound { get { return buttonDebugSound; } }
        public UnityEngine.UI.Button ButtonDebugMovement { get { return buttonDebugMovement; } }
        public UnityEngine.UI.Button ButtonDebugTemperature { get { return buttonDebugTemperature; } }

        public UnityEngine.UI.Button ButtonValidate { get { return buttonValidate; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }

        private bool mHasInitCommands=false;


        // Use this for initialization
        void Start()
        {
            gaugeOnOffFire.DisplayPercentage = true;
            gaugeOnOffKidnap.DisplayPercentage = true;
            gaugeOnOffMovement.DisplayPercentage = true;
            gaugeOnOffSound.DisplayPercentage = true;
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!mHasInitCommands)
            {
                mHasInitCommands = true;
                gaugeOnOffFire.SwitchCommands.Add(new ActFireDetectionCmd());
                gaugeOnOffMovement.SwitchCommands.Add(new ActMovementDetectionCmd());
                gaugeOnOffKidnap.SwitchCommands.Add(new ActKidnappingDetectionCmd());
                gaugeOnOffSound.SwitchCommands.Add(new ActSoundDetectionCmd());
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