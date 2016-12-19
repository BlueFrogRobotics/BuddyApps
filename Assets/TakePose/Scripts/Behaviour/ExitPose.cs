using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;
using BuddyOS.Command;

public class ExitPose : AStateMachineBehaviour
{
	private AnimManager mAnimationManager;

	public override void Init()
	{
		mAnimationManager = GetComponentInGameObject<AnimManager>(2);
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
