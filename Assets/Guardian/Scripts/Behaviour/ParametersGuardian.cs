using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class ParametersGuardian : MonoBehaviour
    {

        [SerializeField]
        private OnOff onOffMovement;

        [SerializeField]
        private OnOff onOffSound;

        [SerializeField]
        private OnOff onOffFire;

        [SerializeField]
        private OnOff onOffKidnap;

        [SerializeField]
        private InputField password;

        [SerializeField]
        private Gauge gaugeMovement;

        [SerializeField]
        private Gauge gaugeSound;

        [SerializeField]
        private Gauge gaugeFire;

        [SerializeField]
        private Gauge gaugeKidnap;

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

        public OnOff OnOffMovement { get { return onOffMovement; } }
        public OnOff OnOffSound { get { return onOffSound; } }
        public OnOff OnOffFire { get { return onOffFire; } }
        public OnOff OnOffKidnap { get { return onOffKidnap; } }
        public InputField Password { get { return password; } }

        public Gauge GaugeMovement { get { return gaugeMovement; } }
        public Gauge GaugeSound { get { return gaugeSound; } }
        public Gauge GaugeFire { get { return gaugeFire; } }
        public Gauge GaugeKidnap { get { return gaugeKidnap; } }

        public UnityEngine.UI.Button ButtonDebugSound { get { return buttonDebugSound; } }
        public UnityEngine.UI.Button ButtonDebugMovement { get { return buttonDebugMovement; } }
        public UnityEngine.UI.Button ButtonDebugTemperature { get { return buttonDebugTemperature; } }

        public UnityEngine.UI.Button ButtonValidate { get { return buttonValidate; } }
        public UnityEngine.UI.Button ButtonBack { get { return buttonBack; } }

        private bool mHasInitCommands=false;


        // Use this for initialization
        void Start()
        {
            gaugeFire.DisplayPercentage = true;
            gaugeKidnap.DisplayPercentage = true;
            gaugeMovement.DisplayPercentage = true;
            gaugeSound.DisplayPercentage = true;
            
        }

        // Update is called once per frame
        void Update()
        {
            if(!mHasInitCommands)
            {
                mHasInitCommands = true;
                onOffFire.OnCommands.Add(new ActFireDetectionCmd());
                onOffFire.OffCommands.Add(new DsactFireDetectionCmd());
                onOffMovement.OnCommands.Add(new ActMovementDetectionCmd());
                onOffMovement.OffCommands.Add(new DsactMovementDetectionCmd());
                onOffKidnap.OnCommands.Add(new ActKidnappingDetectionCmd());
                onOffKidnap.OffCommands.Add(new DsactKidnappingDetectionCmd());
                onOffSound.OnCommands.Add(new ActSoundDetectionCmd());
                onOffSound.OffCommands.Add(new DsactSoundDetectionCmd());
            }
        }

        void OnEnable()
        {
            GuardianData.Instance.FireDetectionIsActive = onOffFire.IsActive;
            GuardianData.Instance.MovementDetectionIsActive = onOffMovement.IsActive;
            GuardianData.Instance.KidnappingDetectionIsActive = onOffKidnap.IsActive;
            GuardianData.Instance.SoundDetectionIsActive = onOffSound.IsActive;
        }
    }
}