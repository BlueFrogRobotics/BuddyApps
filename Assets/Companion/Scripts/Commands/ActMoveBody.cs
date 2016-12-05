using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActMoveBody : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.CanMoveBody = true;
        }
    }
}