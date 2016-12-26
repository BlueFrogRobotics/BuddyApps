using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class ActSaveSettingsCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.DoSaveSetting = Parameters.Integers[0] == 1;
        }
    }
}
