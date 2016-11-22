using BuddyOS.Command;

namespace BuddyApp.Remote
{
    internal class DeactivateCmd : ACommand
    {
        public static DeactivateCmd Create()
        {
            return new DeactivateCmd();
        }

        protected override void ExecuteImpl()
        {
            RemoteControlData.Instance.IsActive = false;
        }
    }
}