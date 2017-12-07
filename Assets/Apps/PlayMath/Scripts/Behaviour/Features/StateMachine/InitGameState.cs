using UnityEngine;

namespace BuddyApp.PlayMath{
	public class InitGameState : AStateMachineBehaviour {

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			GameObject.Find("UI/Four_Answer").GetComponent<QuestionBehaviour>().ResetGame();
            animator.SetTrigger("StartGame");
		}
	}
}
