using UnityEngine;
using System.Collections;
using BuddyOS.Command;
using BuddyOS.UI;
using System;

namespace BuddyApp.RLGL
{
    public class RLGLLayout : AWindowLayout
    {
        public override void Build()
        {
            Dropdown lDropDownValLevel = AddWidget<Dropdown>(FIRST_LINE);
            Button lQuitButton = AddWidget<Button>(SECOND_LINE);

            lDropDownValLevel.AddOption("EASY", RLGLData.Level.LEVEL_EASY);
            lDropDownValLevel.AddOption("MEDIUM", RLGLData.Level.LEVEL_MEDIUM);
            lDropDownValLevel.AddOption("HARD", RLGLData.Level.LEVEL_HARD);

            lDropDownValLevel.UpdateCommands.Add(new SetValLevel());
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<Dropdown>(FIRST_LINE).Label.text = "DIFFICULTY";
            GetWidget<Button>(SECOND_LINE).Label.text = "QUIT APPLICATION";
        }
    }

}
