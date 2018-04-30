using Buddy;
using Buddy.UI;

namespace BuddyApp.RemoteControl
{
    public class RemoteControlLayout : AWindowLayout
    {
        private OnOff mDiscreteMode;
		
        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
            mDiscreteMode = CreateWidget<OnOff>();

            mDiscreteMode.OnSwitchEvent((bool iVal) => {
                RemoteControlData.Instance.DiscreteMode = iVal;
            });

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
            mDiscreteMode.IsActive = RemoteControlData.Instance.DiscreteMode;

		}

        public override void LabelizeWidgets()
        {
            mDiscreteMode.Label = BYOS.Instance.Dictionary.GetString("discretemode");
        }
    }
}