using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetBabyNameCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.BabyName = Parameters.Strings[0];
        }
    }
}
