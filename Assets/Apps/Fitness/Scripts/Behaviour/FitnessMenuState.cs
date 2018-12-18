using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.Fitness
{
	/// <summary>
	/// State for the application's menu : we display a menu with a vertical list toast. 
	/// The vertical list toast allows the developer to display a list of clickable items (buttons) shown as vertical box.
	/// Many parameters are available for these box such as icons / actions / color of the writing...
	/// In this state, we will also explain how to collect user vocal answer using the speech to text 
	/// in order for Buddy to understand what the user said.
	/// </summary>
	public sealed class FitnessMenuState : AStateMachineBehaviour
	{

		// This dictionary will store the text of each button and the transition to trigger when clicked.
		private List<string> mButtonContent = new List<string>();

		public override void Start()
		{
			// We initialize the dictionary, adding the text for the buttons (using the dictionnary) 
			// and the name of the transition to trigger
			if (mButtonContent.Count == 0) {
				mButtonContent.Add("armex");
				mButtonContent.Add("legex");
				mButtonContent.Add("menuresume");
				mButtonContent.Add("menuquit");
			}

			Buddy.Vocal.SayKeyAndListen("menutitle", null, new string[] { "common", "fitness" }, OnEndListenning, null, SpeechRecognitionMode.GRAMMAR_ONLY);
		}

		private void OnEndListenning(SpeechInput iSpeechInput)
		{

			Debug.Log("OnEndListenning " + iSpeechInput.Rule);

			if (!iSpeechInput.IsInterrupted)
				if (string.IsNullOrEmpty(iSpeechInput.Rule))
					Buddy.Vocal.Listen(new string[] { "common", "fitness" }, OnEndListenning, SpeechRecognitionMode.GRAMMAR_ONLY);
				else {
					string lRule = Utils.GetRealStartRule(iSpeechInput.Rule);

					// User asked to quit (from common grammar)
					if (lRule == "quit")
						Buddy.Vocal.Say("bye", OnEndSpeaking);

					// User asked to resume (from fitness grammar)
					else if (lRule == "menuresume") {
						if (string.IsNullOrEmpty(FitnessData.Instance.Exercise))
							FitnessData.Instance.Exercise = "armex";
						Trigger("POSITIONING");

						// User asked an exercise (from fitness grammar)
					} else if (lRule == "legex" || lRule == "armex") {
						FitnessData.Instance.Exercise = lRule;
						Trigger("TUTORIAL");

						// User asked an other rule (from common grammar)
					} else {
						// Wrong rule, listen again!
						Buddy.Vocal.Listen(new string[] { "common", "fitness" }, OnEndListenning, SpeechRecognitionMode.GRAMMAR_ONLY);
					}

				}
		}

		private void OnEndSpeaking(SpeechOutput iSpeech)
		{
			// Call when the user asked to quit
			QuitApp();
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// We have different parts of the UI : Header - Toaster - footer.
			// Each of them control one part of the Buddy's screen, header controls the top, toaster will be all the widgets/UI in the middle 
			// and the footer controls the bottom.
			// Therefore to create a menu, you need to use a toaster and to add a title you need to use the header.
			// A button parameter is always visible on the top right of the screen however it is possible to hide it if the application
			// doesn't need parameters.

			// Remove parameter button
			Buddy.GUI.Header.DisplayParametersButton(false);

			// Display a title
			Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("menutitle"));

			// Display menu toaster with following parameters
			Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

				// For each element of the previously build dictionnary,
				// we will create the corresponding button
				foreach (string lButtonContent in mButtonContent) {
					// We create the container
					TVerticalListBox lBox = iBuilder.CreateBox();
					// We create an event OnClick to be triggered when the user click on the box
					lBox.OnClick.Add(() => {
						Debug.Log("Click " + lButtonContent);

						// User clicked so no need to listen anymore
						Buddy.Vocal.StopListening();

						// User clicked on quit
						if (lButtonContent == "menuquit")
							Buddy.Vocal.Say("bye", OnEndSpeaking);

						// User clicked on resume
						else if (lButtonContent == "menuresume") {
							if (string.IsNullOrEmpty(FitnessData.Instance.Exercise))
								FitnessData.Instance.Exercise = "armex";
							Trigger("POSITIONING");

						// user clicked on one of the exercise
						} else {
							FitnessData.Instance.Exercise = lButtonContent;
							Trigger("TUTORIAL");
						}
					});
					// We label our button with the informations in the dictionary
					lBox.SetLabel(Buddy.Resources.GetString(lButtonContent));
					// We don't need the left button so we will just hide it
					lBox.LeftButton.Hide();
					// We place the text of the button in the center of the box
					lBox.SetCenteredLabel(true);
				}
			});
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// When we exit the state, we need to remove the title and the toast:
			Buddy.GUI.Header.HideTitle();
			Buddy.Vocal.StopListening();
			Buddy.GUI.Toaster.Hide();
		}
	}
}