using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.BoseApp
{
    class SetValPlaylistThreeCmd : ACommand
    {
        public SetValPlaylistThreeCmd() { }
        public SetValPlaylistThreeCmd(string iVal)
        {
            Parameters = new CommandParam();
            Parameters.Strings = new string[1] { iVal };
        }

        protected override void ExecuteImpl()
        {
            string lVal = Parameters.Strings[0];
            BoseAppData.Instance.playlistThree = lVal;
        }
    }
}