using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActMoveBody : ACommand
    {
        protected override void ExecuteImpl()
        {
            bool lMoveBody = Parameters.Integers[0] == 1;
            CompanionData.Instance.CanMoveBody = lMoveBody;
        }
    }
}