using Buddy;
using Buddy.UI;

namespace BuddyApp.MemoryGame
{
	public class MemoryGameLayout : AWindowLayout
	{
		private Dropdown mDropDownValOne;

		private OnOff mFullBody;

		public override void Build()
		{
			/*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
			mDropDownValOne = CreateWidget<Dropdown>();
			mFullBody = CreateWidget<OnOff>();

			/*
             * Set widgets parameters
             */
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("easy"));
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("medium"));
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("hard"));

			if (MemoryGameData.Instance.Difficulty == 0) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("easy"));
			} else if (MemoryGameData.Instance.Difficulty == 1) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("medium"));

			} else if (MemoryGameData.Instance.Difficulty == 2) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("hard"));
			}

			mFullBody.IsActive = MemoryGameData.Instance.FullBody;

			/*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
			//mGaugeValOne.Slider.value = MemoryGameData.Instance.MyValue;

			/*
            * Set command to widgets
            * At each interaction with a widget, a callback will be called
            * ==> What must happen when I interacted with a widget ?
            */
			mDropDownValOne.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) => {
				MemoryGameData.Instance.Difficulty = iIndex;
			});


			mFullBody.OnSwitchEvent((bool iVal) => {
				
				MemoryGameData.Instance.FullBody = iVal;
			});
		}

		public override void LabelizeWidgets()
		{
			mDropDownValOne.Label = "Difficulty";
			mFullBody.Label = "Body Motion";
        }
	}
}