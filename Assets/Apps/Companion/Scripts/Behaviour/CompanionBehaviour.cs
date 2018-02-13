using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Companion
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class CompanionBehaviour : MonoBehaviour
	{
		/*
         * Modified data from the UI interaction
         */
		[SerializeField]
		private Text text;

		/*
         * API of the robot
         */
		private TextToSpeech mTextToSpeech;

		/*
         * Data of the application. Save on disc when app quit happened
         */
		private CompanionData mAppData;
		private float mLastVoiceUpdate;

		/*
         * Init refs to API and your app data
         */
		void Start()
		{
			mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
			mAppData = CompanionData.Instance;
			mLastVoiceUpdate = 0F;
		}

		/*
         * A sample of use of data (here for basic display purpose)
         */
		void Update()
		{
			// ensure motors stay in same state as config
			
			// TODO: do this only if needed, need getter...
			if(Time.time - mLastVoiceUpdate > 2F) {
				BYOS.Instance.Interaction.TextToSpeech.SetPitch(1.0F + 0.1F * BYOS.Instance.Interaction.InternalState.Positivity);
				BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(1.0F + 0.1F * BYOS.Instance.Interaction.InternalState.Energy);
				mLastVoiceUpdate = Time.time;

			}

			if (text.enabled != CompanionData.Instance.Debug) {
				text.enabled = CompanionData.Instance.Debug;
            }

			if (BYOS.Instance.Primitive.Motors.Wheels.Locked == CompanionData.Instance.CanMoveBody) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked wheels");
				BYOS.Instance.Primitive.Motors.Wheels.Locked = !CompanionData.Instance.CanMoveBody;
			}

			if (BYOS.Instance.Primitive.Motors.YesHinge.Locked == CompanionData.Instance.CanMoveHead) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked head");
				BYOS.Instance.Primitive.Motors.YesHinge.Locked = !CompanionData.Instance.CanMoveHead;
				BYOS.Instance.Primitive.Motors.NoHinge.Locked = !CompanionData.Instance.CanMoveHead;
			}
		}
	}
}