using BuddyOS.Command;
using BuddyOS.UI;

namespace BuddyApp.Recipe
{
    public class RecipeLayout : AWindowLayout
    {
        public override void Build()
        {
            Button lQuitButton = AddWidget<Button>(FIRST_LINE);

            lQuitButton.ClickCommands.Add(new HomeCmd());
        }
        public override void Labelize()
        {
            GetWidget<Button>(FIRST_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}
