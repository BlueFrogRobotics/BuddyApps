using System;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections.Generic;
using OpenCVUnity;
using System.IO;
using UnityEngine.Animations;

namespace BuddyApp.TakePhoto
{
    // Voir si on peut remplacer par un Shared take photo après


    public sealed class Settings : AStateMachineBehaviour
    {
		// This dictionary will store the text of each button and the transition to trigger when clicked.
		private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

		public override void Start()
		{

			Debug.LogWarning("[TAKEPHOTO APP] Settings Start ");

			// We initialize the dictionary, adding the text for the buttons (using the dictionnary) 
			// and the name of the transition to trigger
			if (mButtonContent.Count == 0)
			{
				mButtonContent.Add(Buddy.Resources.GetString("menustatemotion"), "MotionTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatephoto"), "TakePhotoTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatewidget"), "WidgetTrigger");
				mButtonContent.Add(Buddy.Resources.GetString("menustatequit"), "QuitTrigger");
			}
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.LogWarning("[TAKEPHOTO APP] Settings state enter ");

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

			
					  //Debug.LogWarning("Enter settings");
					  // Store previous email adress to share
					  string lPreviousValue = string.Empty;
			// Enqueue en ParameterToast request that will be display after all previous queued toasts
			Buddy.GUI.Dialoger.Display<ParameterToast>().With((iBuilder) => { // This callback will be called on Toast display
			TText lText = iBuilder.CreateWidget<TText>(); // Create a new text widget
			lText.SetLabel(Buddy.Resources.GetString("emailtoshare")); // Set the content of the widget
				/*TToggle lToggle = iBuilder.CreateWidget<TToggle>(); // Create a new toggle
                lToggle.SetLabel("my toggle"); // You can labeled it
                lToggle.ToggleValue = true; // The default value
                lToggle.OnToggle.Add((iVal) => { // Callback on each modification
                    Debug.Log("Toggle : " + iVal);
                });*/


			TTextField lField = iBuilder.CreateWidget<TTextField>(); // Create an input field

			lPreviousValue = TakePhotoData.Instance.mailtoshare;

			// if the adresse mail is already saved in the user params
			if (string.IsNullOrEmpty(TakePhotoData.Instance.mailtoshare))
			{
				lField.SetPlaceHolder(Buddy.Resources.GetString("defaultemail"));
			}
			else
			{
				lField.SetPlaceHolder(TakePhotoData.Instance.mailtoshare);
			}

			lField.OnChangeValue.Add((iVal) => { // Callback on each modification
				//Debug.Log("Text field : " + iVal);
				TakePhotoData.Instance.mailtoshare = iVal;
			});
			}, () => { // Callback on the left lateral button click
			//Debug.Log("Cancel");

			// set to previous value
			TakePhotoData.Instance.mailtoshare = lPreviousValue;
			Buddy.GUI.Dialoger.Hide(); // You must hide manually the Toaster
			}, "Cancel",
			() => { // Callback on the right lateral button click
				//Debug.Log("OK");
			Buddy.GUI.Dialoger.Hide(); // You must hide manually the Toaster
			}, "OK");


}

// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
{
Debug.LogWarning("[TAKEPHOTO APP] Settings state update ");
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
