using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActSoundDetectionCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.IsSoundDetectionOn = Parameters.Integers[0] == 1;
        }
    }
}
