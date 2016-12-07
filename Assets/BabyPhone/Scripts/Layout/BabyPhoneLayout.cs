using BuddyOS.UI;
using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneLayout : AWindowLayout
    {
        public override void Build()
        {
            GaugeOnOff lGaugeTimer = AddWidget<GaugeOnOff>(FIRST_LINE);
            Button lQuitButton = AddWidget<Button>(SECOND_LINE);

            lGaugeTimer.Slider.minValue = 1;
            lGaugeTimer.Slider.maxValue = 120;
            lGaugeTimer.Slider.wholeNumbers = true;

            lGaugeTimer.Slider.value = BabyPhoneData.Instance.Timer;
            lGaugeTimer.IsActive = BabyPhoneData.Instance.TimerIsActive;

            lGaugeTimer.UpdateCommands.Add(new SetTimerCmd());
            lGaugeTimer.SwitchCommands.Add(new ActTimerCmd());
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        public override void Labelize()
        {
            GetWidget<GaugeOnOff>(FIRST_LINE).Label.text = "TIMER (min)";
            GetWidget<Button>(SECOND_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}