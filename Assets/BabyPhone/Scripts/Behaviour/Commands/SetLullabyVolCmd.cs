using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetLullabyVolCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.LullabyVolume = Parameters.Integers[0];
        }
    }
}