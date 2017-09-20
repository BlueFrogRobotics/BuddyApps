using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Guardian
{
	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(SaveAudio))]
	[RequireComponent(typeof(SaveVideo))]
	[RequireComponent(typeof(RoombaNavigation))]
	public class DetectionManager : MonoBehaviour
	{

		public const float MAX_SOUND_THRESHOLD = 0.3F;
		public const float KIDNAPPING_THRESHOLD = 4.5F;
		public const float MAX_MOVEMENT_THRESHOLD = 200.0F;

		private Animator mAnimator;
		private KidnappingDetection mKidnappingDetection;
		private MotionDetection mMotionDetection;
		private NoiseDetection mNoiseDetection;
		private ThermalDetection mFireDetection;

		public string Logs { get; private set; }

		public SaveAudio SaveAudio { get; private set; }
		public SaveVideo SaveVideo { get; private set; }

		public RoombaNavigation Roomba { get; private set; }

		//public NoiseStimulus NoiseStimulus { get; private set; }
		//public MoveSideStimulus MoveSideStimulus { get; private set; }
		//public ThermalStimulus ThermalStimulus { get; private set; }
		//public AccelerometerStimulus AccelerometerStimulus { get; private set; }
		//public MotionDetection MovementTracker { get; private set; }


		//public Stimuli Stimuli { get; set; }

		public bool IsDetectingFire { get; set; }
		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingKidnapping { get; set; }
		public bool IsDetectingSound { get; set; }

		public Alert Detected { get; set; }

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
            Debug.Log("awake mono");
            mAnimator = GetComponent<Animator>();
			GuardianActivity.Init(mAnimator, this);
		}

		void Start()
		{
            Debug.Log("start mono");
            Init();
			//LinkDetectorsEvents();
		}

		void Update()
		{
		}


		/// <summary>
		/// Init the stimulis and enable them.
		/// Also init the roomba and the media recorders
		/// </summary>
		public void Init()
		{

            //Stimuli = BYOS.Instance.Perception.Stimuli;
            //MovementTracker = BYOS.Instance.Perception.Motion;
            HasLinkedDetector = false;

			mMotionDetection = BYOS.Instance.Perception.Motion;
			//mMotionDetection.OnDetect(OnMovementDetected);

			mNoiseDetection = BYOS.Instance.Perception.Noise;
			//mNoiseDetection.OnDetect(OnSoundDetected);

			mFireDetection = BYOS.Instance.Perception.Thermal;
			//mFireDetection.OnDetect(OnThermalDetected);
			mFireDetection.Threshold = 50;
			
					
			mKidnappingDetection = BYOS.Instance.Perception.Kidnapping;

			//AStimulus moveSideStimulus;
			//BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.MOVING, out moveSideStimulus);
			//moveSideStimulus.enabled = true;
			//MoveSideStimulus = (MoveSideStimulus)moveSideStimulus;

			//AStimulus soundStimulus;
			//BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.NOISE_LOUD, out soundStimulus);
			//soundStimulus.enabled = true;
			//NoiseStimulus = (NoiseStimulus)soundStimulus;

			//AStimulus fireStimulus;
			//BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.FIRE_DETECTED, out fireStimulus);
			//fireStimulus.enabled = true;
			//ThermalStimulus = (ThermalStimulus)fireStimulus;

			//AStimulus kidnappingStimulus;
			//BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.KIDNAPPING, out kidnappingStimulus);
			//kidnappingStimulus.enabled = true;
			//AccelerometerStimulus = (AccelerometerStimulus)kidnappingStimulus;


			//BYOS.Instance.Perception.MovementTracker.Enable();
			SaveAudio = GetComponent<SaveAudio>();
			SaveVideo = GetComponent<SaveVideo>();
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
		/// Subscribe to the stimulis callbacks
		/// </summary>
		public void LinkDetectorsEvents()
		{
            HasLinkedDetector = true;
            mMotionDetection.OnDetect(OnMovementDetected);
            mNoiseDetection.OnDetect(OnSoundDetected);
            mFireDetection.OnDetect(OnThermalDetected, 50);
            //mFireDetection.Threshold = 50;
            mKidnappingDetection.OnDetect(OnKidnappingDetected, KIDNAPPING_THRESHOLD);
            BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            //Stimuli.RegisterStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            //Stimuli.RegisterStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            //Stimuli.RegisterStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            //Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
        }

		/// <summary>
		/// Unsubscibe to the stimulis callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
            //Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            //Stimuli.RemoveStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            //Stimuli.RemoveStimuliCallback(StimulusEvent.FIRE_DETECTED, OnFireDetected);
            //Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnappingDetected);
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

		private void OnSoundDetected()
		{
			Debug.Log("============== Sound detected stimulus!");
			if (IsDetectingSound) {
				Detected = Alert.SOUND;
				mAnimator.SetTrigger("Alert");
			}
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
			if (!IsDetectingMovement)
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

        public void OnMailSent()
        {
            Debug.Log("le mail a ete fabuleusement envoye");
        }
    }
}
