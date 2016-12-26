using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class SetScreenSaverLightCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            int lVal = Parameters.Integers[0];
            BabyPhoneData.Instance.ScreenSaverLight = lVal;
        }
    }
}