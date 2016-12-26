using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActScreenSaverCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.IsScreanSaverOn = Parameters.Integers[0] == 1;
        }
    }
}
