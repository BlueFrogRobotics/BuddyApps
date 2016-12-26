using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActRobotMotionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.IsMotionOn = Parameters.Integers[0] == 1;
        }
    }
}
