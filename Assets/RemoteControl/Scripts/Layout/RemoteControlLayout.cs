using BuddyOS.Command;
using BuddyOS.UI;

namespace BuddyApp.Remote
{
    public class RemoteControlLayout : AWindowLayout
    {
        public override void Build()
        {
            Button lQuitButton = AddWidget<Button>(THIRD_LINE);            
            lQuitButton.ClickCommands.Add(HomeCmd.Create());
        }

        public override void Labelize()
        {
            GetWidget<Button>(THIRD_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}