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

        //public FireDetector FireDetector { get; private set; }
        //public SoundDetector SoundDetector { get; private set; }
        //public MovementDetector MovementDetector { get; private set; }
        //public KidnappingDetector KidnappingDetector { get; private set; }

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
            //Debug.Log("AH! 1");
            //FireDetector = BYOS.Instance.Perception.FireDetector;//GetComponent<FireDetector>();
            //SoundDetector = BYOS.Instance.Perception.SoundDetector;//GetComponent<SoundDetector>();
            //MovementDetector = BYOS.Instance.Perception.MovementDetector; //GetComponent<MovementDetector>();
            //KidnappingDetector = BYOS.Instance.Perception.KidnappingDetector; //GetComponent<KidnappingDetector>();
            AStimulus moveSideStimulus;
            //Debug.Log("AH! 2");
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.MOVING, out moveSideStimulus);
            //Debug.Log("AH! 3");
            moveSideStimulus.Enable();
            //Debug.Log("AH! 4");
            AStimulus soundStimulus;
            //Debug.Log("AH! 5");
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.NOISE_MEDIUM_LOUD, out soundStimulus);
            //Debug.Log("AH! 6");
            soundStimulus.Enable();
            //Debug.Log("AH! 7");
            AStimulus fireStimulus;
            //Debug.Log("AH! 8");
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.FIRE_DETECTED, out fireStimulus);
            fireStimulus.Enable();
            //Debug.Log("AH! 9");
            AStimulus kidnappingStimulus;
            //Debug.Log("AH! 10");
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.KIDNAPPING, out kidnappingStimulus);
            //Debug.Log("AH! 11");
            kidnappingStimulus.Enable();
            //FireDetector.Enable();
            //SoundDetector.Enable();
            BYOS.Instance.Perception.MovementTracker.Enable();
            //MovementDetector.Enable();
            //KidnappingDetector.Enable();
            //Debug.Log("AH! 12");
            SaveAudio = GetComponent<SaveAudio>();
            SaveVideo = GetComponent<SaveVideo>();
            //Debug.Log("AH! 13");
            Roomba = BYOS.Instance.Navigation.Roomba; //GetComponent<RoombaNavigation>();
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
            //if (!IsDetectingSound)
            //    return;
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
            Stimuli.RegisterStimuliCallback(StimulusEvent.NOISE_MEDIUM_LOUD, OnSoundDetected);
            Stimuli.RegisterStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
            //FireDetector.OnDetection += OnFireDetected;
            //SoundDetector.OnDetection += OnSoundDetected;
            //MovementDetector.OnDetection += OnMovementDetected;
            //KidnappingDetector.OnDetection += OnKidnappingDetected;
        }

        public void UnlinkDetectorsEvents()
        {
            Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.NOISE_MEDIUM_LOUD, OnSoundDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
            //FireDetector.OnDetection -= OnFireDetected;
            //SoundDetector.OnDetection -= OnSoundDetected;
            //MovementDetector.OnDetection -= OnMovementDetected;
            //KidnappingDetector.OnDetection -= OnKidnappingDetected;
        }
    }
}
