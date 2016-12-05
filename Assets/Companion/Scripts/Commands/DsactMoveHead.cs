using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class DsactMoveHead : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.CanMoveHead = false;
        }
    }
}