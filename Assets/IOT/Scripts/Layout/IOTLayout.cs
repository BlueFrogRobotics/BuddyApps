using BuddyOS;
using BuddyOS.UI;
using BuddyOS.Command;

namespace BuddyApp.IOT
{
    public class IOTLayout : AWindowLayout
    {
        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings
             */
            Button lQuitButton = AddWidget<Button>(THIRD_LINE);

            /*
             * Set widgets parameters
             */

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */

            /*
             * Set command to widgets
             * At each interaction with a widget, a command will be updated with the current widget (input) value and will be executed
             * ==> What must happen when I interacted with a widget ?
             */
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<Button>(THIRD_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}