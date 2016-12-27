using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetTimerCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            int lVal = Parameters.Integers[0];
            BabyPhoneData.Instance.Timer = lVal;
        }
    }
}