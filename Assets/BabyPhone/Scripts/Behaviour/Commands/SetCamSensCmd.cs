using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetCamSensCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.CameraSensitivity = Parameters.Integers[0];
        }
    }
}
