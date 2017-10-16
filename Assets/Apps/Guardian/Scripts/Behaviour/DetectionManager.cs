using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(RoombaNavigation))]
	public class DetectionManager : MonoBehaviour
	{

		public const float MAX_SOUND_THRESHOLD = 0.3F;
		public const float KIDNAPPING_THRESHOLD = 4.5F;
		public const float MAX_MOVEMENT_THRESHOLD = 100.0F;


		private Animator mAnimator;
		private KidnappingDetection mKidnappingDetection;
		private MotionDetection mMotionDetection;
		private NoiseDetection mNoiseDetection;
		private ThermalDetection mFireDetection;

        /// <summary>
        /// Speaker volume
        /// </summary>
        public int Volume { get; set; }

        public float CurrentTimer { get; set; }
        public float Countdown { get; set; }

        public string Logs { get; private set; }

		public bool PreviousScanLeft { get; set; }

		public RoombaNavigation Roomba { get; private set; }

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
			Volume = BYOS.Instance.Primitive.Speaker.GetVolume();
            Init();
		}


		/// <summary>
		/// Init the detectors and the roomba navigation
		/// </summary>
		public void Init()
		{
            HasLinkedDetector = false;

			mMotionDetection = BYOS.Instance.Perception.Motion;
			mNoiseDetection = BYOS.Instance.Perception.Noise;
			mFireDetection = BYOS.Instance.Perception.Thermal;		
			mKidnappingDetection = BYOS.Instance.Perception.Kidnapping;

			Roomba = BYOS.Instance.Navigation.Roomba;
			Roomba.enabled = false;
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
            HasLinkedDetector = true;
            mMotionDetection.OnDetect(OnMovementDetected, GuardianData.Instance.MovementDetectionThreshold*MAX_MOVEMENT_THRESHOLD/100);
            mNoiseDetection.OnDetect(OnSoundDetected);
            mFireDetection.OnDetect(OnThermalDetected, 50);
            mKidnappingDetection.OnDetect(OnKidnappingDetected, KIDNAPPING_THRESHOLD);
            BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

		/// <summary>
		/// Unsubscibe to the detectors callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
            HasLinkedDetector = false;
            mKidnappingDetection.StopAllOnDetect();
            mFireDetection.StopAllOnDetect();
            mNoiseDetection.StopAllDetection();
            mMotionDetection.StopAllOnDetect();
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
			Debug.Log("============== Sound detected! detector");
			if (!IsDetectingSound)
				return true;

			if (iSound > (1 - ((float)GuardianData.Instance.SoundDetectionThreshold / 100.0f)) * MAX_SOUND_THRESHOLD) {
				Debug.Log("============== Threshold passed!");
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
		private bool OnKidnappingDetected()
		{
			if (!IsDetectingKidnapping)
				return true;

			Detected = Alert.KIDNAPPING;
			mAnimator.SetTrigger("Alert");
            return true;
		}

        private void OnMediaSaved()
        {

        }

        public void OnMailSent()
        {
            Debug.Log("le mail a ete fabuleusement envoye");
        }
    }
}
