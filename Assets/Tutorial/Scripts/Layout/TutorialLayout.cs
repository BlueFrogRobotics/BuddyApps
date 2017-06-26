using Buddy;
using Buddy.UI;

namespace BuddyApp.Tutorial
{
    public class TutorialLayout : AWindowLayout
    {
        private GaugeOnOff mGaugeValOne;
        private TextField mSearchFieldValTwo;

        public TutorialLayout()
        {
        }

        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings ?
             */
            mGaugeValOne = CreateWidget<GaugeOnOff>();
            mSearchFieldValTwo = CreateWidget<TextField>();

            /*
             * Set widgets parameters
             */
            mGaugeValOne.Slider.minValue = 0;
            mGaugeValOne.Slider.maxValue = 10;
            mGaugeValOne.Slider.wholeNumbers = true;
            mGaugeValOne.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within min and max */

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
            mGaugeValOne.Slider.value = TutorialData.Instance.One;
            mGaugeValOne.IsActive = TutorialData.Instance.OneIsActive;
            mSearchFieldValTwo.FieldText = TutorialData.Instance.Two;

            /*
             * Set command to widgets
             * ==> What must happen when I interacted with a widget ?
             * At each interaction with a widget, a method will be called with the current widget (input) value
             * You can also add an object inherited from ACommand where CommandParam will be automatically updated and the method ExecuteImpl called
             */
            mGaugeValOne.OnUpdateEvent((iVal) => TutorialData.Instance.One = iVal);
            mGaugeValOne.OnSwitchEvent((iVal) => TutorialData.Instance.OneIsActive = iVal);
            mSearchFieldValTwo.OnUpdateEvent((iVal) => TutorialData.Instance.Two = iVal);
        }

        public override void LabelizeWidgets()
        {
            mGaugeValOne.Label = BYOS.Instance.Dictionary.GetString("integer");
            mSearchFieldValTwo.Label = BYOS.Instance.Dictionary.GetString("string");
        }
    }
}