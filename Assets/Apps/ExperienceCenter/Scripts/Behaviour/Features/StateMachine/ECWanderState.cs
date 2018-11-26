using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
	public class ECWanderState : StateMachineBehaviour
	{

		private AnimatorManager mAnimatorManager;
		private WanderBehaviour mWalkBehaviour;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            Debug.Log("on ECWanderState");
            mWalkBehaviour = GameObject.Find ("AIBehaviour").GetComponent<WanderBehaviour> ();
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            Buddy.Vocal.EnableTrigger = false;
            Buddy.Vocal.Stop();
            mWalkBehaviour.InitBehaviour ();
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mWalkBehaviour.StopBehaviour ();
		}
			
	}
}
