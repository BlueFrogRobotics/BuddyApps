using BuddyOS;
using BuddyOS.Command;
using BuddyOS.UI;

namespace BuddyApp.BoseApp
{
    public class BoseAppLayout : AWindowLayout
    {
        public override void Build()
        {
            /*
             * Create needed widgets
             * ==> Which widget do I need for my app settings
             */
            SearchField lSearchFieldRecastToken = AddWidget<SearchField>(FIRST_LINE);
            SearchField lSearchFieldRecastLangage = AddWidget<SearchField>(SECOND_LINE);
            SearchField lSearchFieldBoseAddr = AddWidget<SearchField>(THIRD_LINE);/*
            SearchField lSearchFieldPlaylistOne = AddWidget<SearchField>(FOURTH_LINE);
            SearchField lSearchFieldPlaylistTwo = AddWidget<SearchField>(FIFTH_LINE);
            SearchField lSearchFieldPlaylistThree = AddWidget<SearchField>(SIXTH_LINE);
            SearchField lSearchFieldPlaylistFour = AddWidget<SearchField>(SEVENTH_LINE);
            SearchField lSearchFieldPlaylistFive = AddWidget<SearchField>(EIGHTH_LINE);
            SearchField lSearchFieldPlaylistSix = AddWidget<SearchField>(NINTH_LINE);*/
            Button lQuitButton = AddWidget<Button>(TENTH_LINE);

            /*
             * Retrieve app data and display them inside the view
             * ==> What info must be displayed ?
             */
            lSearchFieldRecastToken.Field.text = BoseAppData.Instance.recastToken;
            lSearchFieldRecastLangage.Field.text = BoseAppData.Instance.recastLangage;
            lSearchFieldBoseAddr.Field.text = BoseAppData.Instance.boseAddr;/*
            lSearchFieldPlaylistOne.Field.text = BoseAppData.Instance.playlistOne;
            lSearchFieldPlaylistTwo.Field.text = BoseAppData.Instance.playlistTwo;
            lSearchFieldPlaylistThree.Field.text = BoseAppData.Instance.playlistThree;
            lSearchFieldPlaylistFour.Field.text = BoseAppData.Instance.playlistFour;
            lSearchFieldPlaylistFive.Field.text = BoseAppData.Instance.playlistFive;
            lSearchFieldPlaylistSix.Field.text = BoseAppData.Instance.playlistSix;*/


            /*
             * Set command to widgets
             * At each interaction with a widget, a command will be updated with the current widget (input) value and will be executed
             * ==> What must happen when I interacted with a widget ?
             */
            lSearchFieldRecastToken.UpdateCommands.Add(new SetValRecastTokenCmd());
            lSearchFieldRecastLangage.UpdateCommands.Add(new SetValRecastLangageCmd());
            lSearchFieldBoseAddr.UpdateCommands.Add(new SetValBoseAddrCmd());/*
            lSearchFieldPlaylistOne.UpdateCommands.Add(SetValPlaylistOneCmd.Create());
            lSearchFieldPlaylistTwo.UpdateCommands.Add(SetValPlaylistTwoCmd.Create());
            lSearchFieldPlaylistThree.UpdateCommands.Add(SetValPlaylistThreeCmd.Create());
            lSearchFieldPlaylistFour.UpdateCommands.Add(SetValPlaylistFourCmd.Create());
            lSearchFieldPlaylistFive.UpdateCommands.Add(SetValPlaylistFiveCmd.Create());
            lSearchFieldPlaylistSix.UpdateCommands.Add(SetValPlaylistSixCmd.Create());*/
            lQuitButton.ClickCommands.Add(new HomeCmd());
        }

        /* Labelize method will be called when the language will be changed. Current values are hard-coded */
        public override void Labelize()
        {
            GetWidget<SearchField>(FIRST_LINE).Label.text = "Recast Token";
            GetWidget<SearchField>(SECOND_LINE).Label.text = "Langage";
            GetWidget<SearchField>(THIRD_LINE).Label.text = "Bose Addr";/*
            GetWidget<SearchField>(FOURTH_LINE).Label.text = "Playlist 1";
            GetWidget<SearchField>(FIFTH_LINE).Label.text = "Playlist 2";
            GetWidget<SearchField>(SIXTH_LINE).Label.text = "Playlist 3";
            GetWidget<SearchField>(SEVENTH_LINE).Label.text = "Playlist 4";
            GetWidget<SearchField>(EIGHTH_LINE).Label.text = "Playlist 5";
            GetWidget<SearchField>(NINTH_LINE).Label.text = "Playlist 6";*/
            GetWidget<Button>(TENTH_LINE).Label.text = "QUIT APPLICATION";
        }
    }
}