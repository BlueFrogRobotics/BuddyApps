using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class DsactMoveBody : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.CanMoveBody = false;
        }
    }
}