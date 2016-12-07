using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.UI;
using BuddyOS.Command;

namespace BuddyApp.Guardian
{
    public class GuardianLayout : AWindowLayout
    {

        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings
             */
            GaugeOnOff lGaugeFireDetection = AddWidget<GaugeOnOff>(FIRST_LINE);
            GaugeOnOff lGaugeMovementDetection = AddWidget<GaugeOnOff>(SECOND_LINE);
            GaugeOnOff lGaugeKidnappingDetection = AddWidget<GaugeOnOff>(THIRD_LINE);
            GaugeOnOff lGaugeSoundDetection = AddWidget<GaugeOnOff>(FOURTH_LINE);
            Button lQuitButton = AddWidget<Button>(FIFTH_LINE);




            lGaugeFireDetection.Slider.minValue = 0;
            lGaugeFireDetection.Slider.maxValue = 10;
            lGaugeFireDetection.Slider.wholeNumbers = true;
            lGaugeFireDetection.DisplayPercentage = true;


            lGaugeMovementDetection.Slider.minValue = 0;
            lGaugeMovementDetection.Slider.maxValue = 10;
            lGaugeMovementDetection.Slider.wholeNumbers = true;
            lGaugeMovementDetection.DisplayPercentage = true;

            lGaugeSoundDetection.Slider.minValue = 0;
            lGaugeSoundDetection.Slider.maxValue = 10;
            lGaugeSoundDetection.Slider.wholeNumbers = true;
            lGaugeSoundDetection.DisplayPercentage = true;

            lGaugeKidnappingDetection.Slider.minValue = 0;
            lGaugeKidnappingDetection.Slider.maxValue = 10;
            lGaugeKidnappingDetection.Slider.wholeNumbers = true;
            lGaugeKidnappingDetection.DisplayPercentage = true;

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
            lGaugeFireDetection.IsActive = GuardianData.Instance.FireDetectionIsActive;
            lGaugeMovementDetection.IsActive = GuardianData.Instance.MovementDetectionIsActive;
            lGaugeSoundDetection.IsActive = GuardianData.Instance.SoundDetectionIsActive;
            lGaugeKidnappingDetection.IsActive = GuardianData.Instance.KidnappingDetectionIsActive;

            /*
             * Set command to widgets
             * At each interaction with a widget, a command will be updated with the current widget (input) value and will be executed
             * ==> What must happen when I interacted with a widget ?
             */
            lGaugeFireDetection.SwitchCommands.Add(new ActFireDetectionCmd());
            //lGaugeFireDetection.OffCommands.Add(new DsactFireDetectionCmd());
            lGaugeMovementDetection.SwitchCommands.Add(new ActMovementDetectionCmd());
            //lGaugeMovementDetection.OffCommands.Add(new DsactMovementDetectionCmd());
            lGaugeKidnappingDetection.SwitchCommands.Add(new ActKidnappingDetectionCmd());
            //lGaugeKidnappingDetection.OffCommands.Add(new DsactKidnappingDetectionCmd());
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<GaugeOnOff>(FIRST_LINE).Label.text = "FIRE DETECTION";
            GetWidget<GaugeOnOff>(SECOND_LINE).Label.text = "MOVEMENT DETECTION";
            GetWidget<GaugeOnOff>(THIRD_LINE).Label.text = "KIDNAPPING DETECTION";
            GetWidget<GaugeOnOff>(FOURTH_LINE).Label.text = "SOUND DETECTION";
            GetWidget<Button>(FIFTH_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}
