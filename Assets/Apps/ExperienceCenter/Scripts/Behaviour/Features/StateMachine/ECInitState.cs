using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.ExperienceCenter{
public class ECInitState : StateMachineBehaviour {

		private TcpServer mTcpServer;
		private static bool FirstRun = true;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			Debug.LogWarningFormat ("First Run : {0}", FirstRun);
			if (FirstRun) {
				ExperienceCenterData.Instance.StatusTcp = "Offline";
				ExperienceCenterData.Instance.IPAddress = "-";
				ExperienceCenterData.Instance.StopDistance = 0.4F;
				ExperienceCenterData.Instance.NoiseTime = 0.2F;
				ExperienceCenterData.Instance.SpeedThreshold = 0.01F;
				ExperienceCenterData.Instance.TableDistance = 1.5F;
				ExperienceCenterData.Instance.IOTDistance = 1.5F;
				ExperienceCenterData.Instance.NoHingeAngle = 60F;
				ExperienceCenterData.Instance.NoHingeSpeed = 90F;
				ExperienceCenterData.Instance.HeadPoseTolerance = 0.2F;
				ExperienceCenterData.Instance.WelcomeTimeOut = 5F;
				ExperienceCenterData.Instance.EnableHeadMovement = true;
				ExperienceCenterData.Instance.EnableBaseMovement = true;
				ExperienceCenterData.Instance.VoiceTrigger = true;
				ExperienceCenterData.Instance.EnableBML = false;
				FirstRun = false;
			}
			mTcpServer =  GameObject.Find ("AIBehaviour").GetComponent<TcpServer> ();
			mTcpServer.Init ();
	}
			
}
}
