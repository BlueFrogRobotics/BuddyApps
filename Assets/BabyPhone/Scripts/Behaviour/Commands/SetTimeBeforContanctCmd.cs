using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetTimeBeforContactCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.TimeBeforContact = Parameters.Integers[0];
        }
    }
}