using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Guardian
{
    [RequireComponent(typeof(SaveAudio))]
    [RequireComponent(typeof(SaveVideo))]

    [RequireComponent(typeof(RoombaNavigation))]
    public class DetectionManager : MonoBehaviour
    {
        private Animator mAnimator;

        public string Logs { get; private set; }

        public SaveAudio SaveAudio { get; private set; }
        public SaveVideo SaveVideo { get; private set; }

        public RoombaNavigation Roomba { get; private set; }

        public NoiseStimulus NoiseStimulus { get; private set;}
        public MoveSideStimulus MoveSideStimulus { get; private set; }
        public ThermalStimulus ThermalStimulus { get; private set; }
        public AccelerometerStimulus AccelerometerStimulus { get; private set; }


        public Stimuli Stimuli { get; set; }

        public bool IsDetectingFire { get; set; }
        public bool IsDetectingMovement { get; set; }
        public bool IsDetectingKidnapping { get; set; }
        public bool IsDetectingSound { get; set; }

        public Alert Detected { get; set; }

        public enum Alert : int
        {
            MOVEMENT,
            SOUND,
            FIRE,
            KIDNAPPING
        }

        void Awake()
        {
            mAnimator = GetComponent<Animator>();
            GuardianActivity.Init(mAnimator, this);
            GuardianActivity.mDetectionManager = this;
        }

        void Start()
        {
            Debug.Log("AH! start detection manager");
            Init();
            LinkDetectorsEvents();

        }

        void Update()
        {
        }

        public void Init()
        {
            Debug.Log("AH! init detection manager");
            
            Stimuli = BYOS.Instance.Perception.Stimuli;

            AStimulus moveSideStimulus;
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.MOVING, out moveSideStimulus);
            moveSideStimulus.Enable();
            MoveSideStimulus = (MoveSideStimulus)moveSideStimulus;

            AStimulus soundStimulus;
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.NOISE_LOUD, out soundStimulus);
            soundStimulus.Enable();
            NoiseStimulus = (NoiseStimulus)soundStimulus;

            AStimulus fireStimulus;
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.FIRE_DETECTED, out fireStimulus);
            fireStimulus.Enable();
            ThermalStimulus = (ThermalStimulus)fireStimulus;

            AStimulus kidnappingStimulus;
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.KIDNAPPING, out kidnappingStimulus);
            kidnappingStimulus.Enable();
            AccelerometerStimulus = (AccelerometerStimulus)kidnappingStimulus;

            BYOS.Instance.Perception.MovementTracker.Enable();
            SaveAudio = GetComponent<SaveAudio>();
            SaveVideo = GetComponent<SaveVideo>();
            Roomba = BYOS.Instance.Navigation.Roomba;
            Roomba.enabled = false;
        }

        public void OnFireDetected()
        {
            if (!IsDetectingFire)
                return;

            Detected = Alert.FIRE;
            mAnimator.SetTrigger("Alert");
        }

        public void OnSoundDetected()
        {
            if (!IsDetectingSound)
                return;
            Debug.Log("son lol");
            Detected = Alert.SOUND;
            mAnimator.SetTrigger("Alert");
        }

        public void OnMovementDetected()
        {
            if (!IsDetectingMovement)
                return;
            //Debug.Log("mouv lol");
            Detected = Alert.MOVEMENT;
            mAnimator.SetTrigger("Alert");
        }

        public void OnKidnappingDetected()
        {
            if (!IsDetectingKidnapping)
                return;

            Detected = Alert.KIDNAPPING;
            mAnimator.SetTrigger("Alert");
        }

        public void AddLog(string iLog)
        {
            Logs += iLog + "\n";
        }

        public void LinkDetectorsEvents()
        {
            Stimuli.RegisterStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            Stimuli.RegisterStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            Stimuli.RegisterStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
        }

        public void UnlinkDetectorsEvents()
        {
            Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
        }
    }
}
