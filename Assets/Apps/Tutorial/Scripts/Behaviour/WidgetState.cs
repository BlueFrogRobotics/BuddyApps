using UnityEngine;

using BlueQuark;

using System;

namespace BuddyApp.Tutorial
{

	public sealed class WidgetState : AStateMachineBehaviour
	{
		private const int NUMBER_NUMPAD = 1337;
		private string mInputNumPad;
		private int mInputNumPadToInt;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// we use the key in the dictionary so that the robot verbalise instructions
			Buddy.Vocal.SayKey("widgetstateintro",

				// When the robot finish to tell the instructions
				oOutput => {

					// verbalise instructions for the following widget
					Buddy.Vocal.SayKey("widgetstatecountdown");
					// We display the countdown toast
					Buddy.GUI.Toaster.Display<CountdownToast>().With(5, 0, 0,
					(iCountDown) => { // When Clicked we pause it
						iCountDown.Playing = !iCountDown.Playing;
					},

					(iCountDown) => { // On each tic print 
						Debug.Log(iCountDown.Second);

						if (iCountDown.IsDone) {
							// When countDown is finished, we hide the current toast
							// and call the next one
							OnEndCountDown();
							Buddy.GUI.Toaster.Hide();
						}
					}
					);
				});
		}

		private void OnEndCountDown()
		{
			// We display the next widget
			Buddy.GUI.Toaster.Display<ParameterToast>().With(
				(iBuilder) => {
					// We add a numpad widget
					TNumPad lNumPad = iBuilder.CreateWidget<TNumPad>();

					// We define what happens when the numpad value is modified
					lNumPad.OnChangeValue.Add((iInput) => {
						// we collect the new value in mInputNumPad variable
						mInputNumPad = iInput; Debug.Log(iInput);
					});

					// We display info
					lNumPad.SetPlaceHolder(Buddy.Resources.GetString("widgetstatenumpadtext"));

					// And verbalise instructions
					Buddy.Vocal.SayKey("widgetstatenumpad");
				},

					   // We define here what happens when clicking on the left button
					   () => {
						   Buddy.GUI.Toaster.Hide();
						   Debug.Log("Click cancel");
						   // When clicking cancel, we go back to the app menu
						   Trigger("MenuTrigger");
					   },

					   // And the associated label of the button
					   "Cancel",

					   // Then what happens when clicking on the right button
					   () => {
						   Int32.TryParse(mInputNumPad, out mInputNumPadToInt);
						   // We compare the enter digits with the code
						   if (mInputNumPadToInt != NUMBER_NUMPAD) {
							   // Buddy tells the user if it's wrong
							   Buddy.Vocal.SayKey("widgetstatewrongnumpad");
						   } else {
							   // We quit the toast and get to the next if right code is given
							   Buddy.GUI.Toaster.Hide();
							   Debug.Log("Click valid");
							   OnEndNumpad();
						   }

						   // And the associated label of the button
					   }, "Valid"

					   );
		}

		private void OnEndNumpad()
		{
			// We display the next toast
			Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {
				// While building, buddy gives instructions
				Buddy.Vocal.Say("widgetstateverticallisttoast");
				TVerticalListBox lBox = iBuilder.CreateBox();

				// This is the callback when the box is clicked
				lBox.OnClick.Add(() => {
					Debug.Log("Click Box");
					//Check if Buddy is speaking to avoid Buddy saying multiple time the key
					// if the user click on the button during the speech
					if (!Buddy.Vocal.IsSpeaking) {
						Buddy.Vocal.SayKey("widgetstateboxclick");
					}
				});

				// Set the box label
				lBox.SetLabel("Box upper text", "box lower text");

				// We can add an icon on the left button
				// lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("icon"));

				// We set the color of the left button
				lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));

				// And define what happen when clicking on it
				lBox.LeftButton.OnClick.Add(() => {
					Debug.Log("Click Left");
					if (Buddy.Vocal.IsSpeaking)
						Buddy.Vocal.SayKey("widgetstateleftclick");
				});

				TRightSideButton lButton = lBox.CreateRightButton();
				// We can add an icon to your RightSideButton
				//lButton.SetIcon(Buddy.Resources.Get<Sprite>("icon"));

				// And define what happen when clicking on it
				lButton.OnClick.Add(() => {
					Debug.Log("Click right");
					OnEndVerticalList();
					Buddy.GUI.Toaster.Hide();
				});
			});
		}

		private void OnEndVerticalList()
		{
			// we display the next toast
			Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
				// A button
				iBuilder.CreateWidget<TButton>().SetLabel("ButtonLabel");
				// A slider
				iBuilder.CreateWidget<TSlider>().OnSlide.Add((iVal) => { Debug.Log(iVal); });
				// A boolean toggle
				iBuilder.CreateWidget<TToggle>().SetLabel("ToggleLabel");
				// A field for input text
				iBuilder.CreateWidget<TTextField>().SetPlaceHolder("PlaceHolder");
				// A text in a box
				iBuilder.CreateWidget<TTextBox>().SetPlaceHolder("PlaceHolder");
				// A text
				iBuilder.CreateWidget<TText>().SetLabel("A text");
				// When asking the user to rate something (e.g. 5 stars)
				iBuilder.CreateWidget<TRate>();
				// A field with hidden text for password
				iBuilder.CreateWidget<TPasswordField>().SetPlaceHolder("PlaceHolderPwd");
			},

			// Cancel button callback
			() => { Debug.Log("Click cancel"); },
			// And label
			"Cancel",

			 // next button callback
			 () => {
				 Debug.Log("Click next");
				 Buddy.Vocal.SayKey("widgetstateparametertoast");
				 Buddy.GUI.Toaster.Hide();
				 Trigger("MenuTrigger");
			 },
			// And label
			"Next"
			 );
		}
	}
}
