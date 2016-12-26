using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActHeadPos : ACommand
    {
        protected override void ExecuteImpl()
        {
            bool lCanSetHeadPos = Parameters.Integers[0] == 1;
            CompanionData.Instance.CanSetHeadPos = lCanSetHeadPos;
            if (!lCanSetHeadPos)
                new SetPosYesCmd(0).Execute();
        }
    }
}