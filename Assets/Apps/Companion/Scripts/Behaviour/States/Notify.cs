using Buddy;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class Notify : AStateMachineBehaviour
	{
		private float mNotifTime;
		private bool mAlarmSound;

		//private Reaction mReaction;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			BYOS.Instance.Primitive.Speaker.FX.Load(
					BYOS.Instance.Resources.Load<AudioClip>("alarm"), 1
				);
		}


		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			mDetectionManager.mFacePartTouched = FaceTouch.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NOTIFY;
			mState.text = "Notify";
			Debug.Log("state: Notify ");

			mNotifTime = 0F;
			Interaction.Mood.Set(MoodType.SCARED);

			BYOS.Instance.Primitive.Speaker.FX.Loop = true;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mNotifTime += Time.deltaTime;

			if (!mDetectionManager.ActiveUrgentReminder) {
				Trigger("INTERACT");
			} else {
				Debug.Log("mNotifTime: " + mNotifTime + " " + mAlarmSound);
				if (Interaction.TextToSpeech.HasFinishedTalking) {
					if (!mAlarmSound) {
						Debug.Log("Start alarm");
						//Play alarm sound
						BYOS.Instance.Primitive.Speaker.FX.Play(1);
						mAlarmSound = true;
						mActionManager.InformNotifPriority(ReminderState.SHOWN, false);
						mNotifTime = 0F;
					} else if (mAlarmSound && (mNotifTime > 3F)) {
						Debug.Log("Stop alarm and tell notif");
						// Stop alarm sound
						Primitive.Speaker.FX.Stop();
						mAlarmSound = false;
						//Tell the notif
						mActionManager.InformNotifPriority(ReminderState.DELIVERED);
					}
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			BYOS.Instance.Primitive.Speaker.FX.Loop = false;
			Primitive.Speaker.FX.Stop();
		}
	}
}