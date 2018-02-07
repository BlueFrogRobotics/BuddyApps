using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECInitState : StateMachineBehaviour
	{

		private TcpServer mTcpServer;
		private static bool FirstRun = true;
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.LogWarningFormat ("First Run : {0}", FirstRun);
			if (FirstRun) {
				ExperienceCenterData.Instance.StatusTcp = "Offline";
				ExperienceCenterData.Instance.IPAddress = "-";
				ExperienceCenterData.Instance.StopDistance = 0.4F;
				ExperienceCenterData.Instance.NoiseTime = 0.2F;
				ExperienceCenterData.Instance.TableDistance = 1.5F;
				ExperienceCenterData.Instance.IOTDistance = 1.5F;
				ExperienceCenterData.Instance.NoHingeAngle = 30F;
				ExperienceCenterData.Instance.NoHingeSpeed = 75F;
				ExperienceCenterData.Instance.HeadPoseTolerance = 0.2F;
				ExperienceCenterData.Instance.WelcomeTimeOut = 3.5F;
				ExperienceCenterData.Instance.MaxDistance = 1F;
				ExperienceCenterData.Instance.MinDistance = 0.7F;
				ExperienceCenterData.Instance.MaxSpeed = 200F;
				ExperienceCenterData.Instance.MinSpeed = -200F;
				ExperienceCenterData.Instance.EnableHeadMovement = true;
				ExperienceCenterData.Instance.EnableBaseMovement = true;
				ExperienceCenterData.Instance.VoiceTrigger = true;
				ExperienceCenterData.Instance.EnableBML = true;
				FirstRun = false;
			}

			mTcpServer = GameObject.Find ("AIBehaviour").GetComponent<TcpServer> ();
			mTcpServer.Init ();

			BYOS.Instance.Header.DisplayParametersButton = false;
		}
	}
}
