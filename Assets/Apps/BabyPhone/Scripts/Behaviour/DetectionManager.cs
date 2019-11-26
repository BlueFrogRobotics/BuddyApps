using BlueQuark;

using UnityEngine;

namespace BuddyApp.BabyPhone
{
	/// <summary>
	/// Manager class that have reference to the differents detectors and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(Navigation))]
	public sealed class DetectionManager : MonoBehaviour
	{
		public const float MAX_SOUND_THRESHOLD = 0.03F;
		public const float MAX_MOVEMENT_THRESHOLD = 5.0F;

		private Animator mAnimator;
		private MotionDetector mMotionDetection;
		private NoiseDetector mNoiseDetection;
        private float mNoiseThreshold;

        public float Countdown { get; set; }

        public string Logs { get; private set; }

		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingSound { get; set; }

		public Alert Detected { get; set; }

        /// <summary>
        /// True if the detectors callbacks have been set
        /// </summary>
        public bool HasLinkedDetector { get; private set; }

        
		/// <summary>
		/// Enum of the different alerts that BabyPhone app can send
		/// </summary>
		public enum Alert : int
		{
			MOVEMENT,
			SOUND
		}


        void Awake()
		{
            mAnimator = GetComponent<Animator>();
			BabyPhoneActivity.Init(mAnimator, this);
            mNoiseThreshold = 0.0F;
        }

		void Start()
		{
            Init();
        }


		/// <summary>
		/// Init the detectors and the roomba navigation
		/// </summary>
		public void Init()
		{
            HasLinkedDetector = false;

            mMotionDetection = Buddy.Perception.MotionDetector;
            mNoiseDetection = Buddy.Perception.NoiseDetector;
        }

        private void Update()
        {
        }

        /// <summary>
        /// Add a string to the log string
        /// </summary>
        /// <param name="iLog"></param>
        public void AddLog(string iLog)
		{
			Logs += iLog + "\n";
		}

		/// <summary>
		/// Subscribe to the detectors callbacks
		/// </summary>
		public void LinkDetectorsEvents()
		{
            if(IsDetectingMovement) {
                MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
                lMotionParam.SensibilityThreshold = BabyPhoneData.Instance.MovementDetectionThreshold * MAX_MOVEMENT_THRESHOLD / 100.0F;
                lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, 240);
                mMotionDetection.OnDetect.AddP(OnMovementDetected, lMotionParam);
            }
            if (IsDetectingSound)
            {
                mNoiseDetection.OnDetect.AddP(OnSoundDetected, 0.0F);
                mNoiseThreshold = (((float)BabyPhoneData.Instance.SoundDetectionThreshold / 100.0F)) * MAX_SOUND_THRESHOLD;
            }
           HasLinkedDetector = true;
        }

		/// <summary>
		/// Unsubscibe to the detectors callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
            HasLinkedDetector = false;
            if (IsDetectingMovement)
                mMotionDetection.OnDetect.RemoveP(OnMovementDetected);
            if (IsDetectingSound)
                mNoiseDetection.OnDetect.RemoveP(OnSoundDetected);
        }

		/// <summary>
		/// Called when noise has been detected
		/// </summary>
		private bool OnSoundDetected(float iSound)
		{
			if (!IsDetectingSound || iSound <= mNoiseThreshold) 
				return true;

			Detected = Alert.SOUND;
			mAnimator.SetTrigger("Alert");

			return true;
		}

		/// <summary>
		/// Called when movement has been detected
		/// </summary>
		private bool OnMovementDetected(MotionEntity[] iMotions)
		{
            if (!IsDetectingMovement || iMotions.Length < 3)
				return true;

			Detected = Alert.MOVEMENT;
			mAnimator.SetTrigger("Alert");
            
            return true;
		}

        public void AlertSimulation()
        {
            Detected = Alert.MOVEMENT;
            mAnimator.SetTrigger("Alert");
        }
    }
}
