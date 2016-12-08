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

            lCanMoveBody.IsActive = CompanionData.Instance.CanMoveBody;
            lCanMoveHead.IsActive = CompanionData.Instance.CanMoveHead;
            lUseCamera.IsActive = CompanionData.Instance.UseCamera;

            lCanMoveBody.SwitchCommands.Add(new ActMoveBody());
            lCanMoveHead.SwitchCommands.Add(new ActMoveHead());
            lUseCamera.SwitchCommands.Add(new ActCamera());
        }

        public override void Labelize()
        {
            GetWidget<OnOff>(FIRST_LINE).Label.text = "Enable body movement";
            GetWidget<OnOff>(SECOND_LINE).Label.text = "Enable head movement";
            GetWidget<OnOff>(THIRD_LINE).Label.text = "Enable camera";
        }
    }
}