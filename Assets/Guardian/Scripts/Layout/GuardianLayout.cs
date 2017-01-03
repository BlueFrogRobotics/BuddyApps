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
            Dropdown lDropDownContact = AddWidget<Dropdown>(FIFTH_LINE);
            Button lQuitButton = AddWidget<Button>(SIXTH_LINE);


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

            lDropDownContact.AddOption(BYOS.Instance.Dictionary.GetString("nobody"), GuardianData.Contact.NOBODY);
            lDropDownContact.AddOption("RODOLPHE HASSELVANDER", GuardianData.Contact.RODOLPHE);
            lDropDownContact.AddOption("JEAN MICHEL MOURIER", GuardianData.Contact.J2M);
            lDropDownContact.AddOption("MAUD VERRAES", GuardianData.Contact.MAUD);
            lDropDownContact.AddOption("BENOIT PIRONNET", GuardianData.Contact.BENOIT);
            lDropDownContact.AddOption("MARC GOURLAN", GuardianData.Contact.MARC);
            lDropDownContact.AddOption("FRANCK DE VISME", GuardianData.Contact.FRANCK);
            lDropDownContact.AddOption("WALID ABDERRAHMANI", GuardianData.Contact.WALID);
            lDropDownContact.SetDefault(1);
            lDropDownContact.SetDefault(0);


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
            lGaugeMovementDetection.SwitchCommands.Add(new ActMovementDetectionCmd());
            lGaugeKidnappingDetection.SwitchCommands.Add(new ActKidnappingDetectionCmd());
            lGaugeSoundDetection.SwitchCommands.Add(new ActSoundDetectionCmd());
            lDropDownContact.UpdateCommands.Add(new ContactGuardianCmd());
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<GaugeOnOff>(FIRST_LINE).Label.text = BYOS.Instance.Dictionary.GetString("detectFire");
            GetWidget<GaugeOnOff>(SECOND_LINE).Label.text = BYOS.Instance.Dictionary.GetString("detectMouv");
            GetWidget<GaugeOnOff>(THIRD_LINE).Label.text = BYOS.Instance.Dictionary.GetString("detectKidnap");
            GetWidget<GaugeOnOff>(FOURTH_LINE).Label.text = BYOS.Instance.Dictionary.GetString("detectSound");
            GetWidget<Dropdown>(FIFTH_LINE).Label.text = BYOS.Instance.Dictionary.GetString("contact");
            GetWidget<Button>(SIXTH_LINE).Label.text = BYOS.Instance.Dictionary.GetString("quit"); ;
        }
    }
}
