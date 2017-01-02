using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.BoseApp
{
    internal class SetValPlaylistFiveCmd : ACommand
    {
        public SetValPlaylistFiveCmd() { }
        public SetValPlaylistFiveCmd(string iVal)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[1] { iVal };
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BoseAppData.Instance.playlistFive = lVal;
        }
    }
}