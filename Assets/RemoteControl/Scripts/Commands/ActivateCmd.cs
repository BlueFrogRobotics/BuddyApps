using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Remote
{
    internal class ActivateCmd : ACommand
    {
        public static ActivateCmd Create()
        {
            return new ActivateCmd();
        }

        protected override void ExecuteImpl()
        {
            RemoteControlData.Instance.IsActive = true;
        }
    }
}