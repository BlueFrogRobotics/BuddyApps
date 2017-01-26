using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
    internal class ActShowQRCode : ACommand
    {
        protected override void ExecuteImpl()
        {
            CompanionData.Instance.ShowQRCode = Parameters.Integers[0] == 1;
        }
    }
}