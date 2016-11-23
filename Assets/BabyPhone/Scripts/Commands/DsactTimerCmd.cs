using System.Collections;
using BuddyOS.Command;

namespace BuddyApp.BabyPhone
{
    internal class DsactTimerCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            BabyPhoneData.Instance.TimerIsActive = false;
        }
    }
}