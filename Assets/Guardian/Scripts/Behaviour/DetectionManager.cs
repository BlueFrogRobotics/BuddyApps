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

            //FireDetector = GetComponent<FireDetector>();
            //SoundDetector = GetComponent<SoundDetector>();
            //MovementDetector = GetComponent<MovementDetector>();
            //KidnappingDetector = GetComponent<KidnappingDetector>();

            SaveAudio = GetComponent<SaveAudio>();
            SaveVideo = GetComponent<SaveVideo>();

            Roomba = GetComponent<RoombaNavigation>();
            Roomba.enabled = false;

            GuardianActivity.Init(mAnimator, this);
        }

        void Start()
        {
        }

        void Update()
        {
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

            Detected = Alert.SOUND;
            mAnimator.SetTrigger("Alert");
        }

        public void OnMovementDetected()
        {
            if (!IsDetectingMovement)
                return;

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
            //FireDetector.OnDetection += OnFireDetected;
            //SoundDetector.OnDetection += OnSoundDetected;
            //MovementDetector.OnDetection += OnMovementDetected;
            //KidnappingDetector.OnDetection += OnKidnappingDetected;
        }

        public void UnlinkDetectorsEvents()
        {
            //FireDetector.OnDetection -= OnFireDetected;
            //SoundDetector.OnDetection -= OnSoundDetected;
            //MovementDetector.OnDetection -= OnMovementDetected;
            //KidnappingDetector.OnDetection -= OnKidnappingDetected;
        }
    }
}
