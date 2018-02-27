using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECWelcomeState : StateMachineBehaviour
	{
		
		private WelcomeBehaviour mBehaviour;
		private IdleBehaviour mIdleBehaviour;
		private QuestionsBehaviour mQuestionsBehaviour;

		private bool mBehaviourInit;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<WelcomeBehaviour> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			mQuestionsBehaviour = GameObject.Find ("AIBehaviour").GetComponent<QuestionsBehaviour> ();
			mBehaviourInit = false;

		}

		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!mBehaviourInit && mIdleBehaviour.behaviourEnd && mQuestionsBehaviour.behaviourEnd) {
				mBehaviour.InitBehaviour ();
				mBehaviourInit = true;
			}
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBehaviour.StopBehaviour ();
		}
			
	}
}
