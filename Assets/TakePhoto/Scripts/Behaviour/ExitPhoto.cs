using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;
using BuddyOS.Command;

public class ExitPhoto : AStateMachineBehaviour
{
	private AnimManager mAnimationManager;

	public override void Init()
	{
		mAnimationManager = GetComponentInGameObject<AnimManager>(10);
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		mAnimationManager.Sigh ();
		mMood.Set(MoodType.NEUTRAL);
		BYOS.Instance.AppManager.Quit();
		//new HomeCmd().Execute();
	}

	protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
	{
	}

	protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
	{
	}
}
