using Buddy.UI;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    public class RedLightGreenLightLayout : AWindowLayout
    {
		private Dropdown mDropDownValOne;
		private Button lQuitButton;
		
        public override void Build()
        {		
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
			mDropDownValOne = CreateWidget<Dropdown>();
            /*
             * Set widgets parameters
             */
			
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("easy"));
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("medium"));
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("hard"));
			mDropDownValOne.AddOption(BYOS.Instance.Dictionary.GetString("impossible"));
			
			if (RedLightGreenLightData.Instance.Difficulty == 0) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("easy"));
			} else if (RedLightGreenLightData.Instance.Difficulty == 1) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("medium"));
			} else if (RedLightGreenLightData.Instance.Difficulty == 2) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("hard"));
			} else if (RedLightGreenLightData.Instance.Difficulty == 3) {
				mDropDownValOne.SetDefault(BYOS.Instance.Dictionary.GetString("impossible"));
			}
            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */ 
			 

            /*
            * Set command to widgets
            * At each interaction with a widget, a callback will be called
            * ==> What must happen when I interacted with a widget ?
            */

			mDropDownValOne.OnSelectEvent((string iLabel, object iAttachedObj, int iIndex) => {
				RedLightGreenLightData.Instance.Difficulty = iIndex;
			});
		}

        public override void LabelizeWidgets()
        {
                mDropDownValOne.Label = "DIFFICULTY";
        }
    }
}