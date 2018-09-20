using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Tutorial
{
	/// <summary>
	/// State for the application's menu : we display a menu with a vertical list toast. 
	/// The vertical list toast allows the developer to display a list of clickable items (buttons) shown as vertical box.
	/// Many parameters are available for these box such as icons / actions / color of the writing...
	/// In this state, we will also explain how to collect user vocal answer using the speech to text 
	/// in order for Buddy to understand what the user said.
	/// </summary>
	public sealed class MenuState : AStateMachineBehaviour
	{

		// This dictionary will store the text of each button and the transition to trigger when clicked.
		private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

		public override void Start()
		{
			base.Start();

			// We initialize the dictionary, adding the text for the buttons (using the dictionnary) 
			// and the name of the transition to trigger
			if (mButtonContent.Count == 0) {
				mButtonContent.Add(Buddy.Resources.GetString("menustatemotion"), "MotionTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatephoto"), "TakePhotoTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatewidget"), "WidgetTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatequit"), "QuitTrigger");
			}
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
			Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("menu"));

			// Display menu toaster with following parameters
			Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

				// For each element of the previously build dictionnary,
				// we will create the corresponding button
				foreach (KeyValuePair<string, string> lButtonContent in mButtonContent) {
					// We create the container
					TVerticalListBox lBox = iBuilder.CreateBox();
					// We create an event OnClick to be triggered when the user click on the box
					lBox.OnClick.Add(() => { Debug.Log("Click " + lButtonContent.Key); Trigger(lButtonContent.Value); });
					// We label our button with the informations in the dictionary
					lBox.SetLabel(lButtonContent.Key);
					// We don't need the left button so we will just hide it
					lBox.LeftButton.Hide();
					// We place the text of the button in the center of the box
					lBox.SetCenteredLabel(true);
					// And we set the backroung color for the button
					lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
				}
			});
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// When we exit the state, we need to remove the title and the toast:
			Buddy.GUI.Header.HideTitle();
			Buddy.GUI.Toaster.Hide();
		}
	}
}

