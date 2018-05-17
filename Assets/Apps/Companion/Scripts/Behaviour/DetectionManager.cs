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
		MOUTH_TOUCH,
		TOUCH,
		BATTERY,
		TRIGGER,
		HUMAN_RGB,
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
		public const float KIDNAPPING_THRESHOLD = 12F;
		public const float MAX_MOVEMENT_THRESHOLD = 4.0F;
		public const int MIN_TEMP = 25;

		internal Detected mDetectedElement;
		internal FaceTouch mFacePartTouched;

		//private Animator mAnimator;
		private KidnappingDetection mKidnappingDetection;
		private HeadForcedStimulus mHeadForced;
		//private MotionDetection mMotionDetection;
		//private NoiseDetection mNoiseDetection;
		private ThermalDetection mThermalDetection;
		//private HumanRecognition mHumanReco;
		private Face mFace;
		private float mTimeElementTouched;
		private int mEyeCounter;
		private float mLastEyeTime;
		private int mMouthCounter;
		private float mLastMouthTime;
		private ActionManager mActionManager;
		private float mTimeOtherTouched;
		private bool mInit;
		private float mTimeSphinx;



		public string Logs { get; private set; }

		public List<Buddy.Reminder> ActiveReminders { get; set; }
		public float TimeLastTouch { get { return Math.Min(Time.time - mTimeElementTouched, Time.time - mTimeOtherTouched); } }
		public bool IsDetectingThermal { get; set; }
		public bool IsDetectingMovement { get; set; }
		public bool IsDetectingKidnapping { get; set; }
		public bool IsDetectingRGBHuman { get; set; }
		public bool IsDetectingBattery { get; set; }
		public bool IsDetectingTrigger { get; set; }
		//public bool IsDetectingSound { get; set; }

		[SerializeField]
		private UnityEngine.UI.Text mState;

		void Start()
		{
			CompanionActivity.Init(null, mState);

			mTimeElementTouched = 0F;
			mTimeOtherTouched = 0F;
			mDetectedElement = Detected.NONE;
			mFacePartTouched = FaceTouch.NONE;

			ActiveReminders = new List<Buddy.Reminder>();

			mInit = false;
			IsDetectingThermal = true;
			IsDetectingBattery = true;
			IsDetectingKidnapping = true;
			IsDetectingTrigger = true;

			//mMotionDetection = BYOS.Instance.Perception.Motion;
			//mNoiseDetection = BYOS.Instance.Perception.Noise;
			mThermalDetection = BYOS.Instance.Perception.Thermal;
			mKidnappingDetection = BYOS.Instance.Perception.Kidnapping;
			//mHumanReco = BYOS.Instance.Perception.Human;
			//BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();


			// Check if activeNotification
			BYOS.Instance.DataBase.Memory.Procedural.AlertCallbacks.Add(NewNotif);
			

			mActionManager = GetComponent<ActionManager>();
			mFace = BYOS.Instance.Interaction.Face;
			LinkDetectorsEvents();
		}

		private void NewNotif(List<Buddy.Reminder> iReminder)
		{
			ActiveReminders = iReminder;
			Debug.Log("[Companion][DetectionManager] need to notify");
		}
		

		internal void StartSphinxTrigger()
		{
			IsDetectingTrigger = true;
			BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
			mTimeSphinx = Time.time;
		}

		internal void StopSphinxTrigger()
		{
			IsDetectingTrigger = false;
			BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
		}

		void Update()
		{


			if (BYOS.Instance.Interaction.SphinxTrigger.FinishedSetup && !mInit) {
				Utils.LogI(LogContext.INTERACTION, "Launching Sphinx Update");
				//BYOS.Instance.Interaction.SphinxTrigger.SetThreshold((float)1e-26);
				mInit = true;
				StartSphinxTrigger();
			}

			// Avoid degradation of sphinx
			if (BYOS.Instance.Interaction.SphinxTrigger.FinishedSetup && IsDetectingTrigger && Time.time - mTimeSphinx > 20F)
				StartSphinxTrigger();

			//Debug.Log("VOCAL TRIGGERED");
			if (IsDetectingTrigger && CompanionData.Instance.CanTrigger)
				if (BYOS.Instance.Interaction.SphinxTrigger.HasTriggered) {
					//Debug.Log("Vocal triggered detector");

					mDetectedElement = Detected.TRIGGER;
				}

			if (BYOS.Instance.Primitive.Battery.EnergyLevel < 15 && BYOS.Instance.Primitive.Battery.EnergyLevel < 0.000001) {
				//Debug.Log("WARNING BATTERY NOT DETECTED!!!");
			}

			// If nothing else touched (eye, mouth) validate the other touched
			if (Time.time - mTimeOtherTouched > 0.3F && mTimeOtherTouched != 0F) {
				Debug.Log("Detection manager other touched");
				//mDetectedElement = Detected.TOUCH;
				mFacePartTouched = FaceTouch.OTHER;
				mDetectedElement = Detected.TOUCH;

				BYOS.Instance.Interaction.InternalState.AddCumulative(
					new EmotionalEvent(2, 1, "othertouch", "TOUCH_FACE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));

				mTimeOtherTouched = 0F;
			}

			if (BYOS.Instance.Primitive.Battery.EnergyLevel < 15 && BYOS.Instance.Primitive.Battery.EnergyLevel > 0.000001)
				if (mDetectedElement == Detected.NONE && IsDetectingBattery)
					mDetectedElement = Detected.BATTERY;


			if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && BYOS.Instance.Primitive.Motors.AreMovable) {

				int i = 0;

				for (i = 0; i < Input.touchCount; ++i) {

					if (mActionManager.CurrentAction == BUDDY_ACTION.TOUCH_INTERACT) {
						mActionManager.LookAt((int)Input.GetTouch(i).position.x, (int)Input.GetTouch(i).position.y);
						if (Input.GetTouch(i).phase == TouchPhase.Ended) {
							// Look back to center
							mActionManager.LookCenter();
							break;
						}
					}
				}


				if (i != Input.touchCount || Input.GetMouseButtonDown(0)) {

					Debug.Log("Detection manager other touched...");
					if (Time.time - mTimeElementTouched > 0.5F && mTimeOtherTouched == 0F) {
						mTimeOtherTouched = Time.time;
						Debug.Log("Detection manager other touched?");
					}
				}
			}

		}

		// TODO: this may need some addition later
		internal bool UserPresent(COMPANION_STATE iState)
		{
			if (iState == COMPANION_STATE.IDLE || iState == COMPANION_STATE.LOOK_FOR_USER || iState == COMPANION_STATE.WANDER)
				return false;
			else return true;
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
			mDetectedElement = Detected.MOUTH_TOUCH;

			mActionManager.StopAllActions();

			//Cancel other touch
			mTimeOtherTouched = 0F;
		}

		private void RightEyeClicked()
		{

			BYOS.Instance.Interaction.InternalState.AddCumulative(
				new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
			Debug.Log("face touched r eye");
			mTimeElementTouched = Time.time;
			//mDetectedElement = Detected.TOUCH;
			mFacePartTouched = FaceTouch.RIGHT_EYE;
			mDetectedElement = Detected.TOUCH;

			//Cancel other touch
			mTimeOtherTouched = 0F;
		}

		private void LeftEyeClicked()
		{

			BYOS.Instance.Interaction.InternalState.AddCumulative(
				new EmotionalEvent(-3, 1, "eyepoke", "POKE_EYE", EmotionalEventType.INTERACTION, InternalMood.ANGRY));
			Debug.Log("face touched l eye");
			mTimeElementTouched = Time.time;
			//mDetectedElement = Detected.TOUCH;
			mFacePartTouched = FaceTouch.LEFT_EYE;
			mDetectedElement = Detected.TOUCH;

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
			//BYOS.Instance.Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HEAD_FORCED_SOFT, OnHeadForcedSoft);
			//BYOS.Instance.Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HEAD_FORCED_HARD, OnHeadForcedHard);
			//BYOS.Instance.Perception.Stimuli.Controllers[StimulusEvent.HEAD_FORCED_SOFT].enabled = true;
			//BYOS.Instance.Perception.Stimuli.Controllers[StimulusEvent.HEAD_FORCED_HARD].enabled = true;

			//mHumanReco.OnDetect(OnHumanDetected, BodyPart.FULL_BODY & BodyPart.FACE & BodyPart.LOWER_BODY & BodyPart.UPPER_BODY);
			mFace.OnClickLeftEye.Add(LeftEyeClicked);
			mFace.OnClickRightEye.Add(RightEyeClicked);
			mFace.OnClickMouth.Add(MouthClicked);

			//BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
			//BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_320_H_240;
			StartSphinxTrigger();
		}

		private void OnHeadForcedHard()
		{
			mActionManager.TimedMood(MoodType.ANGRY);
		}

		private void OnHeadForcedSoft()
		{
			mActionManager.TimedMood(MoodType.SICK);
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

			//BYOS.Instance.Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HEAD_FORCED_SOFT, OnHeadForcedSoft);
			//BYOS.Instance.Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HEAD_FORCED_HARD, OnHeadForcedHard);
			//mHumanReco.StopAllOnDetect();
			mFace.OnClickLeftEye.Clear();
			mFace.OnClickRightEye.Clear();
			mFace.OnClickMouth.Clear();
			BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
		}
	}
}
