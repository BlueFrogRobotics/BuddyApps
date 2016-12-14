using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Call
{
    internal class ActivateCmd : ACommand
    {
        protected override void ExecuteImpl()
        {
            CallData.Instance.IsActive = Parameters.Integers[0] == 1;
        }
    }
}