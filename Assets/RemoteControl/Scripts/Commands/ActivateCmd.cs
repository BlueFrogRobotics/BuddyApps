using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    internal class ActivateCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            RemoteControlData.Instance.IsActive = Parameters.Integers[0] == 1;
        }
    }
}