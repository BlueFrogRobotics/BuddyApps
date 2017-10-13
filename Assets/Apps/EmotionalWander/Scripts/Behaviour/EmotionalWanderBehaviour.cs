using UnityEngine.UI;
using UnityEngine;

using Buddy;
using System.Collections;

namespace BuddyApp.EmotionalWander
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class EmotionalWanderBehaviour : MonoBehaviour
	{


		/*
         * API of the robot
         */
		private TextToSpeech mTTS;
		private Dictionary mDict;
		private IRSensors mIRSensors;
		private USSensors mUSSensors;
		private Mood mMood;
		private NoHinge mNoHinge;
		private Wheels mWheels;
		private YesHinge mYesHinge;


		private const float MIN_DIST_IR = 0.4f;
		private const float MIN_DIST_US = 0.5f;

		private bool mWander;
		private MoodType mWanderMood;
		private float mUpdateTime;
		private float mTTSTime;
		private float mRandomSpeechTime;
		private bool mDoHeadAnimation;
		private float mWanderTime;
		private float mRandomWanderTime;
		private bool mHeadPlaying;
		private float mHeadPos;
		private bool mIsFacingRandDirection;
		private bool mInitialized;
		private bool mFirst;
		private bool mChangingDirection;
		private float mEmoteTime;
		private bool mIsFaceDown;
		private float mTurningTime;
		private VocalManager mVocMan;



		/*
         * Init refs to API and your app data
         */
		void Start()
		{
			mWander = false;
			mFirst = true;
			mUpdateTime = 0F;
			mWanderMood = MoodType.NEUTRAL;
			mHeadPos = -5F;
			mDict = BYOS.Instance.Dictionary;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mIRSensors = BYOS.Instance.Primitive.IRSensors;
			mUSSensors = BYOS.Instance.Primitive.USSensors;
			mMood = BYOS.Instance.Interaction.Mood;
			mNoHinge = BYOS.Instance.Primitive.Motors.NoHinge;
			mWheels = BYOS.Instance.Primitive.Motors.Wheels;
			mYesHinge = BYOS.Instance.Primitive.Motors.YesHinge;
			mVocMan = BYOS.Instance.Interaction.VocalManager;
			mVocMan.EnableTrigger = true;
			mVocMan.EnableDefaultErrorHandling = true;
			mVocMan.OnEndReco = OnSpeechReco;

			mVocMan.StartListenBehaviour = StartListenBehaviour;
			mVocMan.StopListenBehaviour = StopListenBehaviour;
		}

		void OnEnable()
		{
			//if (!mInitialized)
			//	Start();

			//if (mFirst) {
			//	mFirst = false;
			//} else {
				mTurningTime = 7F;
				mHeadPlaying = false;
				mChangingDirection = false;
				mIsFacingRandDirection = false;
				mDoHeadAnimation = true;
				mUpdateTime = Time.time;
				mWanderTime = Time.time;
				mTTSTime = Time.time;
				mEmoteTime = Time.time;
				mRandomSpeechTime = Random.Range(20F, 40F);
				mRandomWanderTime = Random.Range(10F, 30F);
				if (mMood != null)
					mMood.Set(MoodType.NEUTRAL);
				mYesHinge.SetPosition(mHeadPos);
				FaceRandomDirection(80F);
			//}
		}


		void Update()
		{
			if (mWander) {
				mMood.Set(mWanderMood);
				if (mUpdateTime < 0.1F) {
					mUpdateTime += Time.deltaTime;
					return;
				}

				//Say something once in a while to attract attention
				if (Time.time - mTTSTime > mRandomSpeechTime)
					SaySomething(mWanderMood);

				mUpdateTime = 0;

				//Make the actual motion
				EmotionalWander(mWanderMood);

			}
		}




		void OnSpeechReco(string iSpeech)
		{
			string lSpeech = iSpeech.ToLower();
			if (lSpeech.Contains("sad") || lSpeech.Contains("triste")) {
				StartWander((int)MoodType.SAD);
			} else if (lSpeech.Contains("happy") || lSpeech.Contains("heureux") || lSpeech.Contains("heureuse")) {
				StartWander((int)MoodType.HAPPY);
			} else if (lSpeech.Contains("neutral") || lSpeech.Contains("neutre")) {
				StartWander((int)MoodType.NEUTRAL);
			} else if (lSpeech.Contains("lovely") || lSpeech.Contains("amoureux") || lSpeech.Contains("amoureuse")) {
				StartWander((int)MoodType.LOVE);
			} else if (lSpeech.Contains("angry") || lSpeech.Contains("énervé") || lSpeech.Contains("énervée")) {
				StartWander((int)MoodType.ANGRY);
			} else if (lSpeech.Contains("scared") || lSpeech.Contains("effrayé") || lSpeech.Contains("effrayée") ) {
				StartWander((int)MoodType.SCARED);
			} else if (lSpeech.Contains("sick") || lSpeech.Contains("malade")) {
				StartWander((int)MoodType.SICK);
			} else if (lSpeech.Contains("tired") || lSpeech.Contains("fatigué") || lSpeech.Contains("fatiguée")) {
				StartWander((int)MoodType.TIRED);
			} else if (lSpeech.Contains("stop") || lSpeech.Contains("arrête")) {
				StopWander();
			}
		}



		////////////////////////////////////
		// Speech
		private void SaySomething(MoodType mWanderMood)
		{
			//switch (Random.Range(0, 5)) {
			switch (mWanderMood) {
				case MoodType.NEUTRAL:
					mTTS.Say("Everything is alright", true);
					break;
				case MoodType.SCARED:
					mTTS.Say("Help, I'm scared!", true);
					break;
				case MoodType.ANGRY:
					mTTS.Say("Argg! I'm angry! [200] Argg", true);
					break;
				case MoodType.LOVE:
					mTTS.Say("I love you!", true);
					break;
				case MoodType.SAD:
					mTTS.Say("I'm so sad, life is hard...", true);
					break;
				case MoodType.HAPPY:
					mTTS.Say("I'm so happy!", true);
					break;
				case MoodType.SICK:
					mTTS.Say("Cof cof, I'm sick", true);
					break;
				case MoodType.TIRED:
					mTTS.Say("I'm so tired, I need some Watts", true);
					break;
			}
			//mTTS.Say(lSentence, true);
			mTTSTime = Time.time;
			mRandomSpeechTime = Random.Range(20F, 40F);
		}


		////////////////////////////////////
		// Wander behaviour
		public void EmotionalWander(MoodType mWanderMood)
		{
			switch (mWanderMood) {
				case MoodType.SAD:
					SadWander();
					break;
				case MoodType.HAPPY:
					HappyWander();
					break;
				case MoodType.SICK:
					SickWander();
					break;
				case MoodType.TIRED:
					TiredWander();
					break;
				case MoodType.SCARED:
					ScaredWander();
					break;
				case MoodType.ANGRY:
					AngryWander();
					break;
				case MoodType.LOVE:
					LoveWander();
					break;
				default:
					NeutralWander();
					break;
			}
		}

		public void NeutralWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime) {
				//Head looks at random direction
				PlaySearchingHeadAnimation();
				//If no obstacle, go forward
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection)
					mWheels.SetWheelsSpeed(200F, 200F, 400);
				else if (AnyObstructionsInfrared())
					FaceRandomDirection(130F);


			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(100F);
			}
		}


		public void SadWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime * 2) {

				//Head looks down and sometimes up
				PlaySadHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					if (mTurningTime > 4F) {
						float lSpeed = Random.Range(90F, 150F);
						if (Random.Range(0, 2) == 0) {
							mWheels.SetWheelsSpeed(Random.Range(0.3F * lSpeed, lSpeed), lSpeed, 1000);
							mTurningTime = Random.Range(0F, 2F);
						} else {
							mWheels.SetWheelsSpeed(lSpeed, Random.Range(0.3F * lSpeed, lSpeed), 1000);
							mTurningTime = Random.Range(0F, 2F);
						}
					}
				} else if (AnyObstructionsInfrared()) {
					FaceRandomDirection(50F);
					mTurningTime = 7F;
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				mTurningTime = 7F;
				ChangeDirection(50F);
			}
		}



		public void LoveWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime * 2) {

				//Head looks mostely up
				PlayHappyHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					if (mTurningTime > 4F) {
						float lSpeed = Random.Range(150F, 250F);
						if (Random.Range(0, 2) == 0) {
							mWheels.SetWheelsSpeed(Random.Range(0.2F * lSpeed, lSpeed), lSpeed, 1000);
							mTurningTime = Random.Range(0F, 2F);
						} else {
							mWheels.SetWheelsSpeed(lSpeed, Random.Range(0.2F * lSpeed, lSpeed), 1000);
							mTurningTime = Random.Range(0F, 2F);
						}
					}
				} else if (AnyObstructionsInfrared()) {
					FaceRandomDirection(150F);
					mTurningTime = 7F;
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				mTurningTime = 7F;
				ChangeDirection(150F);
			}
		}



		public void SickWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime / 2) {

				//Head looks mostely up
				PlaySickHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					float lSpeed = Random.Range(15F, 70F);
					if (Random.Range(0, 2) == 0) {
						mWheels.SetWheelsSpeed(Random.Range(0.5F * lSpeed, lSpeed), lSpeed, 1000);
					} else {
						mWheels.SetWheelsSpeed(lSpeed, Random.Range(0.5F * lSpeed, lSpeed), 1000);
					}

				} else if (AnyObstructionsInfrared(0.1F)) {
					BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.SIGH);
					FaceRandomDirection(50F);
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(50F);
			}
		}


		public void TiredWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime / 2) {

				//Head looks mostely up
				PlaySadHeadAnimation();

				//If no obstacle, go forward

				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					float lSpeed = Random.Range(15F, 70F);

					mWheels.SetWheelsSpeed(lSpeed, lSpeed, 1000);

				} else if (AnyObstructionsInfrared()) {
					BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.YAWN);
					FaceRandomDirection(50F);
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(50F);
			}
		}

		public void ScaredWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime / 4) {

				//Head looks mostely up
				PlayHappyHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					float lSpeed = Random.Range(100F, 300F);

					mWheels.SetWheelsSpeed(lSpeed, lSpeed, 1000);

				} else if (AnyObstructionsInfrared()) {
					FaceRandomDirection(250F);
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(200F);
			}
		}

		public void AngryWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime * 4) {

				//Head looks mostely up
				PlayAngryHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				if (!AnyObstructionsInfrared() && !mIsFacingRandDirection) {
					// Keep the same speed / turning angle for some seconds
					float lSpeed = Random.Range(200F, 300F);

					mWheels.SetWheelsSpeed(lSpeed, lSpeed, 1000);

				} else if (AnyObstructionsInfrared()) {
					FaceRandomDirection(200F);
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(250F);
			}
		}

		public void HappyWander()
		{
			if (mDoHeadAnimation && Time.time - mWanderTime < mRandomWanderTime * 2) {

				//Head looks mostely up
				PlayHappyHeadAnimation();

				//If no obstacle, go forward
				mTurningTime += Time.deltaTime;
				// Keep the same speed / turning angle for some seconds
				float lSpeed = 150F;
				if (Random.Range(0, 2) == 0) {
					mWheels.SetWheelsSpeed(-lSpeed, lSpeed, 1000);
				} else {
					mWheels.SetWheelsSpeed(lSpeed, -lSpeed, 1000);
				}

			} else {
				//Do a small animation and change direction
				StopHeadAnimation();
				mIsFacingRandDirection = false;
				mDoHeadAnimation = false;
				ChangeDirection(150F);
			}
		}

		//Change mood every now and then
		/*if (Time.time - mEmoteTime > 5F) {
			switch (UnityEngine.Random.Range(0, 3)) {
				case 0:
					mFace.SetEvent(FaceEvent.SMILE);
					break;
				case 1:
					mFace.SetEvent(FaceEvent.YAWN);
					break;
				case 2:
					mFace.SetEvent(FaceEvent.BLINK_DOUBLE);
					break;
				case 3:
					StartCoroutine(LoveFaceCo());
					break;
				case 4:
					StartCoroutine(HappyFaceCo());
					break;
				case 5:
					StartCoroutine(AngryFaceCo());
					break;
			}
			mEmoteTime = Time.time;
		}*/


		public void StartWander(int iMood)
		{
			mWander = true;
			mWanderMood = (MoodType)iMood;
			mMood.Set(mWanderMood);
		}

		public void StopWander()
		{
			mWander = false;
			mMood.Set(MoodType.NEUTRAL);
		}




		private void FaceRandomDirection(float iSpeed)
		{
			if (mIsFacingRandDirection)
				return;

			StartCoroutine(FaceRandomDirectionCo(iSpeed));
			mIsFacingRandDirection = true;
		}

		//Reminder : positive angle makes Buddy turn counter-clockwise, and negative makes him turn clockwise
		//The point here is to detect where we can turn to, to avoid having to turn in too random directions
		private IEnumerator FaceRandomDirectionCo(float iSpeed)
		{
			bool lCollisionLeft = IsCollisionEminent(mIRSensors.Left.Distance, 0.6F);
			bool lCollisionMiddle = IsCollisionEminent(mIRSensors.Middle.Distance, 0.6F);
			bool lCollisionRight = IsCollisionEminent(mIRSensors.Right.Distance, 0.6F);

			float lRandomAngle = 0F;
			//Everything is obstructed ahead, better to make a full round rotation
			if (lCollisionLeft && lCollisionMiddle && lCollisionRight) {
				lRandomAngle = Random.Range(110F, 250F);
				if (lRandomAngle > 180F)
					lRandomAngle = lRandomAngle - 360F;
			}
			//Front and left side are obstructed
			else if (lCollisionLeft && lCollisionMiddle)
				lRandomAngle = Random.Range(-120F, -40F);
			//Only left side is blocked
			else if (lCollisionLeft)
				lRandomAngle = Random.Range(-60F, -30F);
			//Front and right side are obstructed
			else if (lCollisionRight && lCollisionMiddle)
				lRandomAngle = Random.Range(40F, 120F);
			//Only right side is blocked
			else if (lCollisionRight)
				lRandomAngle = Random.Range(30F, 60F);
			//This is just in case we gathered bad IR data
			else {
				lRandomAngle = Random.Range(60F, 300F);
				if (lRandomAngle > 180F)
					lRandomAngle = lRandomAngle - 360F;
			}

			mWheels.TurnAngle(lRandomAngle, iSpeed, 0.02F);
			Debug.Log("[TURNING] turning " + lRandomAngle);
			while (mWheels.Status != MovingState.REACHED_GOAL) {
				yield return null;
			}
			Debug.Log("[TURNING] reached goal");

			mIsFacingRandDirection = false;
		}



		private void PlaySearchingHeadAnimation()
		{
			if (!mHeadPlaying) {
				mHeadPlaying = true;
				StartCoroutine(SearchingHeadCo());
			}
		}

		private void PlaySadHeadAnimation()
		{
			if (!mHeadPlaying) {
				mHeadPlaying = true;
				StartCoroutine(SadHeadCo());
			}
		}

		private void PlayHappyHeadAnimation()
		{
			if (!mHeadPlaying) {
				mHeadPlaying = true;
				StartCoroutine(HappyHeadCo());
			}
		}


		private void PlaySickHeadAnimation()
		{
			if (!mHeadPlaying) {
				mHeadPlaying = true;
				StartCoroutine(SickHeadCo());
			}
		}

		private void PlayAngryHeadAnimation()
		{
			if (!mHeadPlaying) {
				mHeadPlaying = true;
				StartCoroutine(AngryHeadCo());
			}
		}

		private void StopHeadAnimation()
		{
			mHeadPlaying = false;
		}


		//This makes the head look right and left on random angles
		private IEnumerator SearchingHeadCo()
		{
			while (mHeadPlaying) {

				switch (Random.Range(0, 2)) {
					case 0:
						TurnHeadNo(Random.Range(10F, 30F), Random.Range(40F, 60F));
						break;
					case 1:
						TurnHeadYes(Random.Range(mHeadPos - 10F, mHeadPos + 20F), Random.Range(40F, 60F));
						break;
				}
				yield return new WaitForSeconds(1.3F);
			}

		}


		//This makes the head look up and then back down
		private IEnumerator SadHeadCo()
		{
			while (mHeadPlaying) {
				TurnHeadYes(Random.Range(mYesHinge.MinimumAngle / 2, mYesHinge.MinimumAngle * 2 / 3), Random.Range(10F, 25F));
				yield return new WaitForSeconds(4.3F);
				TurnHeadYes(Random.Range(mYesHinge.MaximumAngle / 1.5F, mYesHinge.MaximumAngle), Random.Range(15F, 30F));
				yield return new WaitForSeconds(mRandomWanderTime);
			}
		}

		//This makes the head look up + left / right
		private IEnumerator HappyHeadCo()
		{
			while (mHeadPlaying) {
				TurnHeadYes(Random.Range(mYesHinge.MinimumAngle / 1.5F, mYesHinge.MinimumAngle * 2 / 3), Random.Range(50F, 70F));
				TurnHeadNo(Random.Range(10F, 30F), Random.Range(50F, 70F));
				yield return new WaitForSeconds(1F);
			}
		}

		//This makes the head look up and then back down
		private IEnumerator SickHeadCo()
		{
			while (mHeadPlaying) {
				if (Random.Range(0, 5) < 4) {
					TurnHeadYes(Random.Range(mYesHinge.MaximumAngle / 2, mYesHinge.MaximumAngle), Random.Range(15F, 30F));
				} else {
					TurnHeadNo(Random.Range(20F, 40F), 70F);
					yield return new WaitForSeconds(0.5F);
					TurnHeadNo(Random.Range(30F, 50F), 100F);
					yield return new WaitForSeconds(0.5F);
					TurnHeadNo(Random.Range(30F, 40F), 60F);
					yield return new WaitForSeconds(0.5F);
					TurnHeadNo(Random.Range(0F, 0F), 40F);
					yield return new WaitForSeconds(0.5F);

				}
				yield return new WaitForSeconds(2.3F);
			}
		}

		//This makes the head look up and then back down
		private IEnumerator AngryHeadCo()
		{
			while (mHeadPlaying) {
				TurnHeadNo(Random.Range(20F, 40F), 100F);
				yield return new WaitForSeconds(0.5F);
				TurnHeadNo(Random.Range(30F, 50F), 150F);
				yield return new WaitForSeconds(0.5F);
				TurnHeadNo(Random.Range(30F, 40F), 100F);
				yield return new WaitForSeconds(0.5F);
				TurnHeadNo(Random.Range(0F, 0F), 120F);
				yield return new WaitForSeconds(0.5F);

				yield return new WaitForSeconds(4.3F);
			}
		}


		private void TurnHeadNo(float iHeadNo, float iSpeed)
		{
			if (mNoHinge.CurrentAnglePosition > 0F)
				iHeadNo = -iHeadNo;

			mNoHinge.SetPosition(iHeadNo);
		}

		private void TurnHeadYes(float iHeadYes, float iSpeed)
		{
			mYesHinge.SetPosition(iHeadYes, iSpeed);
		}


		private void TurnHeadUp(float iHeadUp)
		{
			mYesHinge.SetPosition(Random.Range(mYesHinge.MinimumAngle / 2, mYesHinge.MinimumAngle * 2 / 3), 15F);
		}

		private void TurnHeadDown()
		{
			mYesHinge.SetPosition(Random.Range(mYesHinge.MaximumAngle / 2, mYesHinge.MaximumAngle), 50F);
		}


		//Detects if there is any obstacle ahead (using IR and/or US)
		private bool AnyObstructionsInfrared(float iThreshold = MIN_DIST_IR)
		{
			float lLeftIRDistance = mIRSensors.Left.Distance;
			float lMiddleIRDistance = mIRSensors.Middle.Distance;
			float lRightIRDistance = mIRSensors.Right.Distance;
			//float lRightUSDistance = mUSSensors.Right.Distance;
			//float lLeftUSDistance = mUSSensors.Left.Distance;
			return IsCollisionEminent(lLeftIRDistance, iThreshold)
				|| IsCollisionEminent(lMiddleIRDistance, iThreshold)
				|| IsCollisionEminent(lRightIRDistance, iThreshold);
			//|| IsCollisionEminent(lRightUSDistance, MIN_DIST_US)
			//|| IsCollisionEminent(lLeftUSDistance, MIN_DIST_US);
		}

		private bool IsCollisionEminent(float iCollisionDistance, float iThreshold = MIN_DIST_IR)
		{
			return iCollisionDistance != 0.0F && iCollisionDistance < iThreshold;
		}

		private void ChangeDirection(float iSpeed)
		{
			if (mChangingDirection)
				return;

			mChangingDirection = true;
			StartCoroutine(ChangeDirectionCo(iSpeed));
		}

		//Change mood, play an animation and change direction
		private IEnumerator ChangeDirectionCo(float iSpeed)
		{
			//Use the choregraph to perform some animation
			/*switch (Random.Range(0, 5)) {
				case 0:
					mEmotion.EnableChoregraph();
					mEmotion.SetEvent(EmotionEvent.SHY);
					yield return new WaitForSeconds(5F);
					mEmotion.DisableChoregraph();
					break;
				case 1:
					mEmotion.EnableChoregraph();
					mEmotion.SetEvent(EmotionEvent.DISCOVER);
					yield return new WaitForSeconds(3.2F);
					mEmotion.DisableChoregraph();
					break;
				default:
					break;
			}*/

			//Change direction
			mYesHinge.SetPosition(15F);
			float lRandomAngle = Random.Range(-45F, 45F);
			mNoHinge.SetPosition(lRandomAngle);

			yield return new WaitForSeconds(1.5F);

			mWheels.TurnAngle(lRandomAngle, iSpeed, 0.02F);
			mNoHinge.SetPosition(0F);

			yield return new WaitForSeconds(1.5F);

			mDoHeadAnimation = true;
			mRandomWanderTime = Random.Range(10F, 30F);
			mWanderTime = Time.time;
			mEmoteTime = Time.time;
			mChangingDirection = false;
		}


		private void StartListenBehaviour()
		{

		}

		private void StopListenBehaviour()
		{

		}


	}
}