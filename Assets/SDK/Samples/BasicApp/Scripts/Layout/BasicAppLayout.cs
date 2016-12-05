using BuddyOS;
using BuddyOS.UI;
using BuddyOS.Command;

namespace BuddyApp.Basic
{
    public class BasicAppLayout : AWindowLayout
    {
        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings
             */
            GaugeOnOff lGaugeValOne = AddWidget<GaugeOnOff>(FIRST_LINE);
            SearchField lSearchFieldValTwo = AddWidget<SearchField>(SECOND_LINE);
            Button lQuitButton = AddWidget<Button>(THIRD_LINE);

            /*
             * Set widgets parameters
             */
            lGaugeValOne.Slider.minValue = 0;
            lGaugeValOne.Slider.maxValue = 10;
            lGaugeValOne.Slider.wholeNumbers = true;
            lGaugeValOne.DisplayPercentage = true; /* Only the display will be in percentage, the value will still be within 0 and 10 */

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */ 
            lGaugeValOne.Slider.value = BasicAppData.Instance.One;
            lGaugeValOne.IsActive = BasicAppData.Instance.OneIsActive;
            lSearchFieldValTwo.Field.text = BasicAppData.Instance.Two;

            /*
             * Set command to widgets
             * At each interaction with a widget, a command will be updated with the current widget (input) value and will be executed
             * ==> What must happen when I interacted with a widget ?
             */ 
            lGaugeValOne.UpdateCommands.Add(new SetValOneCmd());
            lGaugeValOne.OnCommands.Add(new ActValOneCmd());
            lGaugeValOne.OffCommands.Add(new DsactValOneCmd());
            lSearchFieldValTwo.UpdateCommands.Add(new SetValTwoCmd());
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<GaugeOnOff>(FIRST_LINE).Label.text = "AN INTEGER";
            GetWidget<SearchField>(SECOND_LINE).Label.text = "A STRING";
            GetWidget<Button>(THIRD_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}