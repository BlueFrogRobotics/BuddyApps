using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.BoseApp
{
    class SetValRecastTokenCmd : ACommand
    {
        public SetValRecastTokenCmd() { }
        public SetValRecastTokenCmd(string iVal)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[1] { iVal };
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BoseAppData.Instance.recastToken = lVal;
        }
    }
}