using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActMoveHead : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.CanMoveHead = true;
        }
    }
}