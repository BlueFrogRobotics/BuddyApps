using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Tutorial
{
	/// <summary>
	/// In this state we show how to ask a question to the user with vocal and tactile answer options.
	/// In this example, we ask the user to confirm he wants to quit the app.
	/// It's important for the user to have the choice between the touch screen and the vocal 
	/// so the developer should provide both on its application (You don't need to but it's a good UX practice)
	/// Keep in mind that the user can also quit the app when it press the button Quit (top right).
	/// </summary>
	public sealed class QuitState : AStateMachineBehaviour
	{
		// Number of listenning iteration before quit
		// Used as a timeout
		private int mNumberListen;

		// When we want a parameter to be field directly in the Unity editor, we use SerializeField
		// to keep it private and still give access from the unity interface.
		[SerializeField]
		private int MaxListenningIter;

		public override void Start()
		{
			base.Start();

			//We need Buddy to listen at least once, if the developer forgot to enter the number of listen we initialize it at 1.
			if (MaxListenningIter == 0)
				MaxListenningIter = 1;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mNumberListen = 0;

			// ParameterToast can display many widgets and also display two buttons and set actions when you click on it. 
			Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
				// First we put the widget we want to use, there is a lot of widget option, they are listed in the documentation.
				iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("quit"));
			},
				// Then we add Action to the two buttons and label on those buttons.
				() => {
					// If the user click on no, we log (print) it
					ExtLog.I(ExtLogModule.APP, typeof(QuitState), LogStatus.INFO, LogInfo.RUNNING, "Click no");

					// If the user click on no, we hide the toast
					Buddy.GUI.Toaster.Hide();

					// then we go back to the menu
					Trigger("MenuTrigger");
				},

				// We put the correct tag on the button, using the dictionary key "no"
				Buddy.Resources.GetString("no"),

				// Same for the button yes...
				() => {
					ExtLog.I(ExtLogModule.APP, typeof(QuitState), LogStatus.INFO, LogInfo.RUNNING, "Click yes");
					Buddy.GUI.Toaster.Hide();
					QuitApp();
				},

				Buddy.Resources.GetString("yes")

			);

			// While showing the toast, the robot also say a random sentence from the dico
			// to give the instructions, then listen to the user's answer
			Buddy.Vocal.SayAndListen(
				Buddy.Resources.GetRandomString("quit"),
				null,
				(iInput) => { OnEndListen(iInput); });
		}

		/// <summary>
		/// This function is called when an answer is received from the user
		/// </summary>
		/// <param name="iInput">User speech input</param>
		private void OnEndListen(SpeechInput iInput)
		{
			// We collect the human answer in Buddy.Vocal.LastHeardInput.Utterance and check if it
			// is one of the expected sentences in the dico
			if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "yes")) {

				// if the user says yes, we hide the toast and quit the app
				Buddy.GUI.Toaster.Hide();
				QuitApp();

			} else if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "no")) {

				// If the user says no, we hide the toast and get back to the menu
				Buddy.GUI.Toaster.Hide();
				Trigger("MenuTrigger");
			} else {
				if (mNumberListen < MaxListenningIter) {
					// if the human answer is outside of planned sentences, we increment the
					// number of listen and we listen again.
					mNumberListen++;
					Buddy.Vocal.Listen(
						iInputRec => { OnEndListen(iInputRec); }
						);
				} else {
					// If we launch the listen too many times, it's like a timeout and
					// we get back to the menu
					Buddy.GUI.Toaster.Hide();
					Trigger("MenuTrigger");
				}
			}
		}
	}
}

