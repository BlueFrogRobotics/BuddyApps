using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.Companion
{


	public enum Detected
	{
		NONE,
		MOVEMENT,
		SOUND,
		THERMAL,
		KIDNAPPING,
		TOUCH,
		BATTERY,
		TRIGGER,
		HUMAN_RGB
	}

	public enum FaceTouch
	{
		NONE,
		MOUTH,
		RIGHT_EYE,
		LEFT_EYE,
		OTHER
	}

	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	public class DetectionManager : MonoBehaviour
	{

		public const float MAX_SOUND_THRESHOLD = 0.2F;
		public const float KIDNAPPING_THRESHOLD = 4.5F;
		public const float MAX_MOVEMENT_THRESHOLD = 4.0F;
		public const int MIN_TEMP = 25;

		public Detected mDetectedElement;
		public FaceTouch mFacePartTouched;

		//private Animator mAnimator;
		private KidnappingDetection mKidnappingDetection;
		//private MotionDetection mMotionDetection;
		//private NoiseDetection mNoiseDetection;
		private ThermalDetection mThermalDetection;
		private HumanRecognition mHumanReco;
		private Face mFace;
		private float mTimeElementTouched;
		private int mEyeCounter;
		private float mLastEyeTime;
		private int mMouthCounter;
		private float mLastMouthTime;
		private ActionManager mActionManager;
		private float mTimeOtherTouched;

		/// <summary>
		/// Speaker volume
		/// </summary>
		public int Volume { get; set; }

		public string Logs { get; private set; }

		public bool IsDetectingThermal { get; set; }
		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingKidnapping { get; set; }
		public bool IsDetectingRGBHuman { get; set; }
		public bool IsDetectingBattery { get; set; }
		public bool IsDetectingTrigger { get; set; }
		//public bool IsDetectingSound { get; set; }

		void Start()
		{
			mTimeElementTouched = 0F;
			mTimeOtherTouched = 0F;
			mDetectedElement = Detected.NONE;
			mFacePartTouched = FaceTouch.NONE;
			Volume = BYOS.Instance.Primitive.Speaker.GetVolume();
			//mMotionDetection = BYOS.Instance.Perception.Motion;
			//mNoiseDetection = BYOS.Instance.Perception.Noise;
			mThermalDetection = BYOS.Instance.Perception.Thermal;
			mKidnappingDetection = BYOS.Instance.Perception.Kidnapping;
			mHumanReco = BYOS.Instance.Perception.Human;
			BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();

			mActionManager = GetComponent<ActionManager>();
			mFace = BYOS.Instance.Interaction.Face;
			LinkDetectorsEvents();
		}

		void Update()
		{

			if (BYOS.Instance.Primitive.Battery.EnergyLevel < 15 && BYOS.Instance.Primitive.Battery.EnergyLevel < 0.000001) {
				Debug.Log("WARNING BATTERY NOT DETECTED!!!");
			}

			// If nothing else touchedc (eye, mouth) validate the other touched
			if (Time.time - mTimeOtherTouched > 0.5F && mTimeOtherTouched != 0F) {

				//mDetectedElement = Detected.TOUCH;
				mFacePartTouched = FaceTouch.OTHER;
				mActionManager.HeadReaction();
				mTimeOtherTouched = 0F;
			}

			if (BYOS.Instance.Primitive.Battery.EnergyLevel < 15 && BYOS.Instance.Primitive.Battery.EnergyLevel > 0.000001)

				if (mDetectedElement == Detected.NONE && IsDetectingBattery)
					mDetectedElement = Detected.BATTERY;
				else if (BYOS.Instance.Interaction.SphinxTrigger.HasTriggered) {
					Debug.Log("VOCAL TRIGGERED");
					if (IsDetectingTrigger)
						mDetectedElement = Detected.TRIGGER;
				} else if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
					if (Time.time - mTimeElementTouched > 0.5F) {
						mTimeOtherTouched = Time.time;
					}
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



		private void MouthClicked()
		{
			Debug.Log("face touched mouth");
			mTimeElementTouched = Time.time;
			//mDetectedElement = Detected.TOUCH;
			mFacePartTouched = FaceTouch.MOUTH;
			mDetectedElement = Detected.TOUCH;

			//Cancel other touch
			mTimeOtherTouched = 0F;
		}

		private void RightEyeClicked()
		{
			Debug.Log("face touched r eye");
			mTimeElementTouched = Time.time;
			//mDetectedElement = Detected.TOUCH;
			mFacePartTouched = FaceTouch.RIGHT_EYE;
			mActionManager.EyeReaction();

			//Cancel other touch
			mTimeOtherTouched = 0F;
		}

		private void LeftEyeClicked()
		{
			Debug.Log("face touched l eye");
			mTimeElementTouched = Time.time;
			//mDetectedElement = Detected.TOUCH;
			mFacePartTouched = FaceTouch.LEFT_EYE;
			mActionManager.EyeReaction();

			//Cancel other touch
			mTimeOtherTouched = 0F;
		}




		/// <summary>
		/// Called when fire has been detected
		/// </summary>
		private bool OnThermalDetected(ObjectEntity[] iObject)
		{
			//Debug.Log("Thermal detection!");
			if (mDetectedElement == Detected.NONE && IsDetectingThermal)
				mDetectedElement = Detected.THERMAL;
			return true;
		}


		private bool OnHumanDetected(HumanEntity[] obj)
		{
			Debug.Log("Human detection!");
			if (mDetectedElement == Detected.NONE && IsDetectingRGBHuman)
				mDetectedElement = Detected.HUMAN_RGB;
			return true;
		}

		/// <summary>
		/// Called when noise has been detected
		/// </summary>
		//private bool OnSoundDetected(float iSound)
		//{
		//	if (!IsDetectingSound)
		//		return true;

		//	if (iSound > (MAX_SOUND_THRESHOLD)) {
		//		mDetectedElement = Detected.SOUND;
		//	}
		//	return true;
		//}

		/// <summary>
		/// Called when movement has been detected
		/// </summary>
		//private bool OnMovementDetected(MotionEntity[] iMotions)
		//{
		//	if (!IsDetectingMovement || iMotions.Length < 3)
		//		return true;

		//	//Detected = Alert.MOVEMENT;
		//	//mAnimator.SetTrigger("Alert");

		//	return true;
		//}

		/// <summary>
		/// Called when buddy is being kidnapped
		/// </summary>
		private bool OnKidnappingDetected()
		{
			if (mDetectedElement == Detected.NONE && IsDetectingKidnapping)
				mDetectedElement = Detected.KIDNAPPING;
			return true;
		}


		/// <summary>
		/// Subscribe to the detectors callbacks
		/// </summary>
		public void LinkDetectorsEvents()
		{
			//mMotionDetection.OnDetect(OnMovementDetected, MAX_MOVEMENT_THRESHOLD);
			//mNoiseDetection.OnDetect(OnSoundDetected);
			mThermalDetection.OnDetect(OnThermalDetected, MIN_TEMP);
			mKidnappingDetection.OnDetect(OnKidnappingDetected, KIDNAPPING_THRESHOLD);
			//mHumanReco.OnDetect(OnHumanDetected, BodyPart.FULL_BODY & BodyPart.FACE & BodyPart.LOWER_BODY & BodyPart.UPPER_BODY);
			mFace.OnClickLeftEye.Add(LeftEyeClicked);
			mFace.OnClickRightEye.Add(RightEyeClicked);
			mFace.OnClickMouth.Add(MouthClicked);
			//BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
			BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_320_H_240;
			BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
		}

		/// <summary>
		/// Unsubscibe to the detectors callbacks
		/// </summary>
		public void UnlinkDetectorsEvents()
		{
			mKidnappingDetection.StopAllOnDetect();
			mThermalDetection.StopAllOnDetect();
			//mNoiseDetection.StopAllDetection();
			//mMotionDetection.StopAllOnDetect();
			mHumanReco.StopAllOnDetect();
			mFace.OnClickLeftEye.Clear();
			mFace.OnClickRightEye.Clear();
			mFace.OnClickMouth.Clear();
			BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
		}
	}
}
