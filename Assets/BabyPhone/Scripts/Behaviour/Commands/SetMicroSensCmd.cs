using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetMicroSensCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.MicrophoneSensitivity = Parameters.Integers[0];
        }
    }
}
