using BuddyOS;

namespace BuddyApp.Remote
{
    internal class ActivateCmd : ACommand
    {
        public static ActivateCmd Create()
        {
            return CreateInstance<ActivateCmd>();
        }

        protected override void ExecuteImpl()
        {
            RemoteControlData.Instance.IsActive = true;
        }
    }
}