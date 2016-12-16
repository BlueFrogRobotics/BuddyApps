using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    [RequireComponent(typeof(FireDetector))]
    [RequireComponent(typeof(SoundDetector))]
    [RequireComponent(typeof(MovementDetector))]
    [RequireComponent(typeof(KidnappingDetector))]
    public class DetectionManager : MonoBehaviour
    {

        private FireDetector mFireDetector;
        private SoundDetector mSoundDetector;
        private MovementDetector mMovementDetector;
        private KidnappingDetector mKidnappingDetector;

        public FireDetector FireDetector { get { return mFireDetector; } }
        public SoundDetector SoundDetector { get { return mSoundDetector; } }
        public MovementDetector MovementDetector { get { return mMovementDetector; } }
        public KidnappingDetector KidnappingDetector { get { return mKidnappingDetector; } }

        public enum Alert : int { NONE = 0, FIRE = 1, SOUND = 2, MOVEMENT = 3, KIDNAPPING = 4 };

        void Awake()
        {
            mFireDetector = GetComponent<FireDetector>();
            mSoundDetector = GetComponent<SoundDetector>();
            mMovementDetector = GetComponent<MovementDetector>();
            mKidnappingDetector = GetComponent<KidnappingDetector>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (mMovementDetector.IsMovementDetected)
              //  Debug.Log("mouv detected");
        }
    }
}
