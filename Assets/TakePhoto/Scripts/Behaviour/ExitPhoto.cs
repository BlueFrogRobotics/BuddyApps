using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;
using BuddyOS.Command;


namespace BuddyApp.TakePhoto
{
	public class ExitPhoto : AStateMachineBehaviour
	{
		private AnimManager mAnimationManager;

		public override void Init()
		{

			Debug.Log("init exit");
			mAnimationManager = GetComponentInGameObject<AnimManager>(8);
			Debug.Log("init exit done");
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimationManager.Sigh();
			mMood.Set(MoodType.NEUTRAL);
			QuitApp();
		}

		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}