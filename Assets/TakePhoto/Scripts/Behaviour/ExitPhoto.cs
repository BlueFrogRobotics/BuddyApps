using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

public class ExitPhoto : AStateMachineBehaviour
{
	public override void Init()
	{
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//link.animationManager.Sigh ();
		mMood.Set(MoodType.NEUTRAL);
		BYOS.Instance.AppManager.Quit();
	}

	protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
	{
	}

	protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
	{
	}
}
