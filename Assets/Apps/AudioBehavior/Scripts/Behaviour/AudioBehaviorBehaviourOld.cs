using BlueQuark;

using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace BuddyApp.AudioBehavior
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class AudioBehaviorBehaviourOld : MonoBehaviour
	{
		/*
         * Data of the application. Save on disc when app is quitted
         */

		ThermalDetector mThermalDetector;

		private AudioBehaviorData mAppData;

		private bool mThermal;
		private bool mHumanTracking;
		private bool mNewTrigger;
		private bool mGoTowardHuman;
		private bool mYolo;

		private int mSoundLoc;
		private int mLastAverageAmbiant;
		private int mRotation;
		private int mMotion;

		private const float ROTATION_VELOCITY = 30F;

		private float mLastTime;
		private float mTimeHumanDetected;
		private float mAngleLastDetect;
		private float mLastSoundLocTime;
		private float mAngleAtTrigger;
		private float mTimeTrigger;
		private float mTargetOdom;

		private List<int> mAverageAmbiant;

		public void ToggleYolo()
		{
			mYolo = !mYolo;

			if (mYolo)
			{
				mThermal = false;
				Buddy.Actuators.Head.Yes.SetPosition(3F);
				// Creation & Settings of parameters that will be used in detection
				Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
					new HumanDetectorParameter
					{
						HumanDetectionMode = HumanDetectionMode.VISION,
					}
				);
			}
			else
			{
				Buddy.Perception.HumanDetector.OnDetect.Clear();
				Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
			}
		}

		public void ToggleThermal()
		{
			mThermal = !mThermal;

			//if (mThermal) {
			//    mYolo = false;
			//    Buddy.Actuators.Head.Yes.SetPosition(5F);
			//    // Creation & Settings of parameters that will be used in detection
			//    mThermalDetector.mCallback = OnHumanDetect;
			//    mThermalDetector.Start();
			//} else {
			//mThermalDetector.Stop();
			Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
			//}
		}

		public void ToggleHumanTracking()
		{
			mHumanTracking = !mHumanTracking;

			if (!mThermal && !mYolo)
				ToggleThermal();
		}

		void Start()
		{

            //mThermalDetector = new ThermalDetector();

            /*
			* You can setup your App activity here.
			*/
            AudioBehaviorActivity.Init(null);
            mNewTrigger = false;
			mHumanTracking = false;
			mGoTowardHuman = false;
			Buddy.Vocal.EnableTrigger = true;
            
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
			Buddy.Sensors.Microphones.EnableSoundLocalization = true;
            
            mAverageAmbiant = new List<int>();
            
            Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
                //    Buddy.Sensors.Microphones.SoundLocalizationParameters.Algorithm,
                SoundLocalizationParameters.DEFAULT_RESOLUTION,
				40);
            
            Buddy.Sensors.Microphones.BeamformingParameters = new BeamformingParameters(6);
			Debug.LogError("Nb callbacks before clear " + Buddy.Vocal.OnCompleteTrigger.Count);
			Buddy.Vocal.OnCompleteTrigger.Clear();
			Buddy.Vocal.OnCompleteTrigger.Add(BuddyTrigged);
            ToggleYolo();
            //Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
        }

		/*
       *   On a human detection this function is called.
       */
		private void OnHumanDetect(HumanEntity[] iHumans)
		{
			mTimeHumanDetected = Time.time;
			mAngleLastDetect = Buddy.Actuators.Wheels.Odometry.AngleDeg();

			if (mGoTowardHuman || mHumanTracking)
			{
				double lCentered;
				if (mYolo)
					lCentered = iHumans[0].Center.x / Buddy.Sensors.RGBCamera.Width;
				else
					lCentered = iHumans[0].Center.x;
				if (lCentered < 0.6F && lCentered > 0.4F)
				{
					mGoTowardHuman = false;
					Buddy.Actuators.Wheels.Stop();
					mMotion = 0;
					// otherwise, try to put the human in the center
				}
				else if (lCentered < 0.5F && mMotion != 1)
				{
					Debug.LogWarning("turn 45 human is at: " + lCentered);
					mMotion = 1;
					Buddy.Actuators.Wheels.SetVelocities(0F, ROTATION_VELOCITY);
				}
				else if (lCentered > 0.5 && mMotion != 2)
				{
					Debug.LogWarning("turn -45 human is at: " + lCentered);
					mMotion = 2;
					Buddy.Actuators.Wheels.SetVelocities(0F, -ROTATION_VELOCITY);
				}

				// If we find human before sound loc destination, stop navigation and center on human
			}
			else if (Math.Abs(mTargetOdom - mAngleLastDetect) < 20 && mNewTrigger && Buddy.Navigation.IsBusy)
			{
				Buddy.Navigation.Stop();
				mGoTowardHuman = true;
			}
		}

		private void BuddyTrigged(SpeechHotword iHotWord)
		{

			mNewTrigger = true;
			// Reset listen + callback of search
			Debug.LogWarning("Buddy Triggered time + nb callbacks " + Time.time + " " +
			Buddy.Vocal.OnCompleteTrigger.Count);

			Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
			//if (mThermal)
			//    mThermalDetector.mCallback = OnHumanDetect;
			Buddy.Vocal.Stop();
			Buddy.Actuators.Wheels.Stop();

			mAngleAtTrigger = Buddy.Actuators.Wheels.Odometry.AngleDeg();
			mTimeTrigger = Time.time;

			// Restart Yolo
			//if (mYolo) {
			//    mThermal = false;
			//    Buddy.Actuators.Head.Yes.SetPosition(5F);
			//    // Creation & Settings of parameters that will be used in detection
			//    Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
			//        new HumanDetectorParameter {
			//            SensorMode = SensorMode.VISION,
			//        }
			//    );
			//}

			Debug.LogWarning("my angle " + mSoundLoc + " my time" + (Time.time - mLastSoundLocTime));
			Debug.LogWarning("os angle " + iHotWord.SoundLocalization);
			Debug.LogWarning("reco score " + iHotWord.RecognitionScore);
			Debug.LogWarning("angle at trigger " + mAngleAtTrigger);
			Debug.LogWarning("time " + DateTime.Now.ToString());


			Debug.LogWarning("Last human time!!!! " + (Time.time - mTimeHumanDetected));
			// Check if human already present, then just stay in front.
			if (Time.time - mTimeHumanDetected < 0.8F && (Math.Abs(iHotWord.SoundLocalization) < 60 || iHotWord.SoundLocalization == Microphones.NO_SOUND_LOCALIZATION /* TODO NEED FIX TOF || Buddy.Sensors.TimeOfFlightSensors.Back.FilteredValue < 1300F*/))
			{
				mGoTowardHuman = true;

                Buddy.Vocal.Say("Je t'aicoute "/*temps " + lTime*/);
                OnEndSearch();

				// Check if soundloc ok
			}
			else if (iHotWord.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION)
			{
				mGoTowardHuman = false;
				Buddy.Actuators.Wheels.Stop();
				Debug.LogWarning("motion to last sound loc : " + iHotWord.SoundLocalization);
				Buddy.Navigation.Run<DisplacementStrategy>().Rotate(iHotWord.SoundLocalization, ROTATION_VELOCITY * 2, OnEndMoveSoundLoc);
				mRotation = iHotWord.SoundLocalization;
				mTargetOdom = iHotWord.SoundLocalization + mAngleAtTrigger;

				if (mTargetOdom > 180F)
					mTargetOdom = mTargetOdom - 360F;
				else if (mTargetOdom < -180F)
					mTargetOdom = mTargetOdom + 360F;

				// Otherwise, look for human if yolo
			}
			else
			{
				Buddy.Behaviour.SetMood(Mood.THINKING);
				float lTime = Time.time - mTimeTrigger;
				Debug.LogWarning("No motion temps " + lTime);
				Buddy.Vocal.Say("Je te cherche "/*temps " + lTime*/);
				Debug.LogWarning("No motion bcs last sound loc too old: " + Time.time + " " + mLastSoundLocTime + " = " + (Time.time - mLastSoundLocTime));

				// find human
				// TODO find with thermal?
				if (mYolo)
				{
					Buddy.Actuators.Wheels.SetVelocities(0F, ROTATION_VELOCITY);
					mGoTowardHuman = true;
					Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound,
					new HumanDetectorParameter
					{
						HumanDetectionMode = HumanDetectionMode.VISION,
					}
				);

					//} else if (mThermal) {
					//    mThermalDetector.mCallback = OnHumanFound;
				}
				else
				{
					Buddy.Vocal.Say("Mais je t'écoute");
					OnEndSearch();
				}

			}

		}

		private void OnEndMoveSoundLoc()
		{
			Debug.LogWarning("End motion of soundloc ");
			mGoTowardHuman = true;
			if (Time.time - mTimeHumanDetected < 0.2F || !mYolo && !mThermal)
			{
				Debug.LogWarning("End motion of soundloc and human seen ");
				OnEndSearch();
				// Go further
			}
			else
			{
				Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.THINKING);
				Debug.LogWarning("End motion of soundloc and ...");

				// A human was detected after human trigger
				if (mTimeHumanDetected > mTimeTrigger)
				{


					Debug.LogWarning("human seen, rotate  " + (-Math.Sign(mRotation)) *
						(Math.Abs(mRotation) - (Math.Abs(Buddy.Actuators.Wheels.Odometry.AngleDeg() - mAngleLastDetect) % 360)));

					Debug.LogWarning("human seen, rotate to " + mAngleLastDetect);
					Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleLastDetect, 80F, (Action<Vector3>)OnEndSearch);
				}
				else
				{
					if (mYolo)
					{
						// Keep turning the same way
						Debug.LogWarning("no human try to find with yolo, turn " + (-Math.Sign(mRotation)) * ROTATION_VELOCITY);
						// Wait for stop navigation strategy before rotation
						StartCoroutine("DelayedRotate");
						Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound,
						new HumanDetectorParameter
						{
							HumanDetectionMode = HumanDetectionMode.VISION,
						}
						);
						//} else if (mThermal) {
						//    // Keep turning the same way
						//    Buddy.Actuators.Wheels.SetVelocities(0F, (-mRotation / Math.Abs(mRotation)) * 45F);
						//    mThermalDetector.mCallback = OnHumanFound;
					}
					else
					{
						// Go back to initial position
						Debug.LogWarning("no human seen, rotate to " + (Math.Sign(mRotation)) * (360 - Math.Abs(mRotation)));
						Buddy.Navigation.Run<DisplacementStrategy>().Rotate(Math.Sign(mRotation) * (360 - Math.Abs(mRotation)), 80F, OnEndMoveNoHuman);
					}
				}
			}

		}

		private IEnumerator DelayedRotate()
		{
			yield return new WaitForSeconds(0.1F);
			Buddy.Actuators.Wheels.SetVelocities(0F, (Math.Sign(mRotation)) * ROTATION_VELOCITY);
		}

		private void OnEndSearch(Vector3 obj)
		{
			Buddy.Behaviour.SetMood(Mood.NEUTRAL);
			OnEndSearch();
		}

		private void OnEndMoveNoHuman()
		{
			Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

			Debug.LogWarning("We did not find a human!!");
			float lTime = Time.time - mTimeTrigger;
			Debug.LogWarning("No motion temps " + lTime);
			Buddy.Vocal.Say("Je ne t'ai pas trouvai mais je t'aicoute ");

			Buddy.Behaviour.SetMood(Mood.NEUTRAL);

			Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleAtTrigger, 80F, (Action<Vector3>)OnEndSearch);
		}

		private void OnHumanFound(HumanEntity[] obj)
		{
			if (mNewTrigger)
			{
				Buddy.Actuators.Wheels.ImmediateStop();
				mNewTrigger = false;
				Debug.LogWarning("On Human found nb callbacks " + Buddy.Perception.HumanDetector.OnDetect.Count);
				Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

				Debug.LogWarning("On Human found post nb callbacks " + Buddy.Perception.HumanDetector.OnDetect.Count);

				float lTime = Time.time - mTimeTrigger;
				Debug.LogWarning("Ha te voila! Je t'aicoute " + lTime);
				Buddy.Vocal.Say("Ha te voila! Je t'aicoute "/*temps " + lTime*/);
				//if (mThermal)
				//    mThermalDetector.mCallback = OnHumanDetect;
				Buddy.Behaviour.SetMood(Mood.HAPPY, 2F);
				Debug.LogWarning("We found a human!!");
				OnEndSearch();
			}
		}

		private void OnEndSearch()
		{
			mNewTrigger = false;
			mGoTowardHuman = false;
			Buddy.Actuators.Wheels.Stop();
			//Buddy.Perception.HumanDetector.OnDetect.Clear();
			Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
			//if (mThermal)
			//    mThermalDetector.mCallback = OnHumanDetect;

			Buddy.Actuators.Head.Yes.SetPosition(9F);
			Debug.LogWarning("motion end ");
			// We can disable Yolo for now

			//Buddy.Vocal.Listen(EndListening, SpeechRecognitionMode.GRAMMAR_ONLY);
			EndListening();
		}

		private void EndListening(/*SpeechInput iObj*/)
		{
			// Just to be sure
			Buddy.Actuators.Wheels.Stop();

			if (/*!iObj.IsInterrupted*/ true)
			{
				//Buddy.Sensors.Microphones.EnableSoundLocalization = false;
				//Buddy.Sensors.Microphones.EnableBeamforming = false;
				//Buddy.Sensors.Microphones.EnableEchoCancellation = true;
				float lTime = Time.time - mTimeTrigger;
				Buddy.Vocal.Say("J'ai fini l'aicoute "/*temps " + lTime*/, EndSpeaking);
			}
		}

		private void EndSpeaking(SpeechOutput iObj)
		{
			if (!iObj.IsInterrupted)
			{

				// Just to be sure
				Buddy.Actuators.Wheels.Stop();

				Buddy.Behaviour.SetMood(Mood.NEUTRAL);
				//Buddy.Sensors.Microphones.EnableEchoCancellation = false;
				//Buddy.Sensors.Microphones.EnableSoundLocalization = true;
				//Buddy.Sensors.Microphones.EnableBeamforming = true;
				Debug.LogWarning("BeamForming enabled ");

				if (mYolo)
					Buddy.Actuators.Head.Yes.SetPosition(3F);
				else
					Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
			}
		}

		private void Update()
		{
			if (mAverageAmbiant.Count == 15)
			{
				mLastAverageAmbiant = (int)mAverageAmbiant.Average();
				Debug.LogWarning("last ambiant average" + mLastAverageAmbiant);

				// Remove values to update every 15 seconds
				mAverageAmbiant.Clear();

				// Change soundloc threshold with ambiant: 55 -> 65, 35 -> 10, 75 -> 85

				int lThresh = 10;
				//if (mLastAverageAmbiant < 62)
				//	lThresh = mLastAverageAmbiant - 10;
    //            else if (mLastAverageAmbiant < 70)
    //                lThresh = mLastAverageAmbiant;
    //            else
    //                lThresh = mLastAverageAmbiant + 20;


                Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
					SoundLocalizationParameters.DEFAULT_RESOLUTION, lThresh);

				//int lThresh = (int)Math.Pow(mLastAverageAmbiant, 3) / 3000;
				//Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
				//Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, lThresh);
				Debug.LogWarning("New SoundLoc threshold: " + lThresh);
			}
			else if (Time.time - mLastTime > 1F)
			{
				mLastTime = Time.time;
				mAverageAmbiant.Add(Buddy.Sensors.Microphones.AmbientSound);
				//Debug.LogWarning("last ambiant " + Buddy.Sensors.Microphones.AmbiantSound);                
			}


			if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION)
			{
				if (mSoundLoc != Buddy.Sensors.Microphones.SoundLocalization)
				{
					Debug.LogWarning("New sound loc " + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString());
					mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
					mLastSoundLocTime = Time.time;
				}
			}
		}
	}
}
