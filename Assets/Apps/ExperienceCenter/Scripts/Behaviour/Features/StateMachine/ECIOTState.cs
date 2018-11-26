using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
	public class ECIOTState : StateMachineBehaviour
	{
		
		private IOTBehaviour mBehaviour;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            Debug.Log("on ECIOTState");
            mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IOTBehaviour> ();
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            Buddy.Vocal.EnableTrigger = false;
            Buddy.Vocal.Stop();
            mBehaviour.InitBehaviour ();
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBehaviour.StopBehaviour ();
		}

	}
}
