using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActAnimCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.IsAnimationOn = Parameters.Integers[0] == 1;
        }
    }
}
