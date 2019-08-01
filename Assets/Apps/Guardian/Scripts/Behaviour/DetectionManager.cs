using BlueQuark;

using UnityEngine;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// Manager class that have reference to the differents detectors and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(Navigation))]
	public sealed class DetectionManager : MonoBehaviour
	{

		public const float MAX_SOUND_THRESHOLD = 0.03F;
		public const float KIDNAPPING_THRESHOLD = 4.5F;
		public const float MAX_MOVEMENT_THRESHOLD = 5.0F;
        public const int MAX_TEMPERATURE_THRESHOLD = 40;

		private Animator mAnimator;
		private KidnappingDetector mKidnappingDetection;
		private MotionDetector mMotionDetection;
		private NoiseDetector mNoiseDetection;
		private ThermalDetector mFireDetection;

        /// <summary>
        /// Speaker volume
        /// </summary>
        public int Volume { get; set; }

        public float CurrentTimer { get; set; }
        public float Countdown { get; set; }

        public string Logs { get; private set; }

		public bool PreviousScanLeft { get; set; }

		public bool IsDetectingFire { get; set; }
		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingKidnapping { get; set; }
		public bool IsDetectingSound { get; set; }
        public bool IsPasswordCorrect { get; set; }
        public bool IsAlarmWorking { get; set; }

		public Alert Detected { get; set; }

        /// <summary>
        /// True if the detectors callbacks have been set
        /// </summary>
        public bool HasLinkedDetector { get; private set; }

        
		/// <summary>
		/// Enum of the different alerts that Guardian app can send
		/// </summary>
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
        }

		void Start()
		{
			Volume = (int)(Buddy.Actuators.Speakers.Volume * 100F);
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
            mFireDetection = Buddy.Perception.ThermalDetector;
            mKidnappingDetection = Buddy.Perception.KidnappingDetector;
        }

        private void Update()
        {
            if(IsDetectingFire && mFireDetection.IsHotterThan(MAX_TEMPERATURE_THRESHOLD))
            {
                Detected = Alert.FIRE;
                mAnimator.SetTrigger("Alert"); 
            }
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
            if(GuardianData.Instance.MovementDetection) {
                MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
                lMotionParam.SensibilityThreshold = GuardianData.Instance.MovementDetectionThreshold * MAX_MOVEMENT_THRESHOLD / 100.0F;
                lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, 240);
                mMotionDetection.OnDetect.AddP(OnMovementDetected, lMotionParam);
            }
            HasLinkedDetector = true;
            mNoiseDetection.OnDetect.AddP(OnSoundDetected, 0.0F);
            Buddy.Sensors.ThermalCamera.OnNewFrame.Add((iInput) => OnNewFrame(iInput));
            ///TODO: subscribe to kidnapping detector
        }

		/// <summary>
		/// Unsubscibe to the detectors callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
            HasLinkedDetector = false;
            if (GuardianData.Instance.MovementDetection)
                mMotionDetection.OnDetect.RemoveP(OnMovementDetected);
            mNoiseDetection.OnDetect.RemoveP(OnSoundDetected);
            Buddy.Sensors.ThermalCamera.OnNewFrame.Remove((iInput) => OnNewFrame(iInput));
            ///TODO: unsubscribe to kidnapping detector
        }

        /// <summary>
        /// Called when fire has been detected
        /// </summary>
        private bool OnThermalDetected(ObjectEntity[] iObject)
		{
			if (!IsDetectingFire)
				return true;

			Detected = Alert.FIRE;
			mAnimator.SetTrigger("Alert");
            return true;
		}

		/// <summary>
		/// Called when noise has been detected
		/// </summary>
		private bool OnSoundDetected(float iSound)
		{
			if (!IsDetectingSound)
				return true;
			if (iSound > (((float)GuardianData.Instance.SoundDetectionThreshold / 100.0F)) * MAX_SOUND_THRESHOLD) {
				Detected = Alert.SOUND;
				mAnimator.SetTrigger("Alert");
			}
			return true;
		}

		/// <summary>
		/// Called when movement has been detected
		/// </summary>
		private bool OnMovementDetected(MotionEntity[] iMotions)
		{
            if (!IsDetectingMovement || iMotions.Length<3)
				return true;

			Detected = Alert.MOVEMENT;
			mAnimator.SetTrigger("Alert");
            
            return true;
		}

		/// <summary>
		/// Called when buddy is being kidnapped
		/// </summary>
		private void OnKidnappingDetected()
		{
			if (!IsDetectingKidnapping)
				return;

			Detected = Alert.KIDNAPPING;
			mAnimator.SetTrigger("Alert");
		}

        private void OnNewFrame(ThermalCameraFrame iFrame)
        {
            if (!IsDetectingFire)
                return;

            if (mFireDetection.GetHottestTemperature() > DetectionManager.MAX_TEMPERATURE_THRESHOLD)
            {
                Detected = Alert.FIRE;
                mAnimator.SetTrigger("Alert");
            }
        }

        private void OnMediaSaved()
        {

        }

        public void OnMailSent()
        {
            
        }
    }
}
