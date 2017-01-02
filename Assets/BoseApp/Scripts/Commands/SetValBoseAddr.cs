using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.BoseApp
{
    class SetValBoseAddrCmd : ACommand
    {
        public SetValBoseAddrCmd() { }
        public SetValBoseAddrCmd(string iVal)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[1] { iVal };
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BoseAppData.Instance.boseAddr = lVal;
        }
    }
}