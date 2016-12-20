using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActVolCmd : ACommand
    {
        protected override void ExecuteImpl()
        {

            BabyPhoneData.Instance.IsVolumeOn = Parameters.Integers[0] == 1;
        }
    }
}
