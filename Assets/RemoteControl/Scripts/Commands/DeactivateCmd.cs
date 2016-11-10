using BuddyOS;

namespace BuddyApp.Remote
{
    internal class DeactivateCmd : ACommand
    {
        public static DeactivateCmd Create()
        {
            return CreateInstance<DeactivateCmd>();
        }

        protected override void ExecuteImpl()
        {
            RemoteControlData.Instance.IsActive = false;
        }
    }
}