using UnityEngine;
using System.Collections;
using BuddyFeature.Detection;

namespace BuddyApp.Guardian
{
    [RequireComponent(typeof(BuddyFeature.Detection.MovementDetector))]
    [RequireComponent(typeof(BuddyFeature.Detection.KidnappingDetector))]
    [RequireComponent(typeof(BuddyFeature.Detection.SoundDetector))]
    [RequireComponent(typeof(BuddyFeature.Detection.FireDetector))]
    public class Detectors : MonoBehaviour
    {
        private BuddyFeature.Detection.MovementDetector mMovementDetector;
        private BuddyFeature.Detection.FireDetector mFireDetector;
        private BuddyFeature.Detection.KidnappingDetector mKidnappingDetector;
        private BuddyFeature.Detection.SoundDetector mSoundDetector;

        public BuddyFeature.Detection.FireDetector FireDetector { get { return mFireDetector; } }
        public BuddyFeature.Detection.SoundDetector SoundDetector { get { return mSoundDetector; } }
        public BuddyFeature.Detection.MovementDetector MovementDetector { get { return mMovementDetector; } }
        public BuddyFeature.Detection.KidnappingDetector KidnappingDetector { get { return mKidnappingDetector; } }

        void Awake()
        {
            mMovementDetector = GetComponent<BuddyFeature.Detection.MovementDetector>();
            mSoundDetector = GetComponent<BuddyFeature.Detection.SoundDetector>();
            mFireDetector = GetComponent<BuddyFeature.Detection.FireDetector>();
            mKidnappingDetector = GetComponent<BuddyFeature.Detection.KidnappingDetector>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}