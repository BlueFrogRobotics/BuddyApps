using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class PMCertificateState : AStateMachineBehaviour {

		private Animator mCertificateAnimator;

        private CertificateBehaviour mCertificateBehaviour;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mCertificateAnimator = GameObject.Find("UI/EndGame_Certificate").GetComponent<Animator>();
			mCertificateAnimator.SetTrigger("open");

            GameObject.Find("UI/EndGame_Certificate").GetComponent<CertificateBehaviour>().InitState();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mCertificateAnimator.SetTrigger("close");
        }
    }
}
