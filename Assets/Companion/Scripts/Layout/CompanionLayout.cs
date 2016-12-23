using BuddyOS.UI;

namespace BuddyApp.Companion
{
    public class CompanionLayout : AWindowLayout
    {
        public override void Build()
        {
            OnOff lCanMoveBody = AddWidget<OnOff>(FIRST_LINE);
            OnOff lCanMoveHead = AddWidget<OnOff>(SECOND_LINE);
            OnOff lUseCamera = AddWidget<OnOff>(THIRD_LINE);
            GaugeOnOff lHeadPosition = AddWidget<GaugeOnOff>(FOURTH_LINE);

            lHeadPosition.Slider.minValue = -30.0F;
            lHeadPosition.Slider.maxValue = 30.0F;
            lHeadPosition.Slider.wholeNumbers = true;

            lCanMoveBody.IsActive = CompanionData.Instance.CanMoveBody;
            lCanMoveHead.IsActive = CompanionData.Instance.CanMoveHead;
            lUseCamera.IsActive = CompanionData.Instance.UseCamera;
            lHeadPosition.IsActive = CompanionData.Instance.CanSetHeadPos;
            lHeadPosition.Slider.value = CompanionData.Instance.HeadPosition;

            lCanMoveBody.SwitchCommands.Add(new ActMoveBody());
            lCanMoveHead.SwitchCommands.Add(new ActMoveHead());
            lUseCamera.SwitchCommands.Add(new ActCamera());
            lHeadPosition.UpdateCommands.Add(new SetHeadPos());
            lHeadPosition.SwitchCommands.Add(new ActHeadPos());
        }

        public override void Labelize()
        {
            GetWidget<OnOff>(FIRST_LINE).Label.text = "Enable body movement";
            GetWidget<OnOff>(SECOND_LINE).Label.text = "Enable head movement";
            GetWidget<OnOff>(THIRD_LINE).Label.text = "Enable camera";
            GetWidget<GaugeOnOff>(FOURTH_LINE).Label.text = "Head position";
        }
    }
}