using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActMotionDetectionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.IsMotionDetectionOn = Parameters.Integers[0] == 1;
        }
    }
}