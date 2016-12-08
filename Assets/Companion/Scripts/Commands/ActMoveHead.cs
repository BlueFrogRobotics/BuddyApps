using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActMoveHead : ACommand
    {
        protected override void ExecuteImpl()
        {
            bool lMoveHead = Parameters.Integers[0] == 1;
            CompanionData.Instance.CanMoveHead = lMoveHead;
        }
    }
}