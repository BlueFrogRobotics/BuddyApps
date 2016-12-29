using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.HideAndSeek
{
    public class Players : MonoBehaviour
    {

        [SerializeField]
        private InputField inputNumPlayer;

        [SerializeField]
        private Button buttonValidate;

        public Button ButtonValidate { get { return buttonValidate; } }
        public int NumPlayer { get { return mNumPlayer; } set { mNumPlayer = value; } }
        public List<string> NamesPlayers { get { return mNamesPlayers; } }
        public List<bool> PlayersFoundState { get { return mPlayersFoundState; } }
        

        private List<string> mNamesPlayers;
        private List<bool> mPlayersFoundState;
        private List<Player> mListPlayer;
        private int mNumPlayer = 0;

        // Use this for initialization
        void Start()
        {
            mNamesPlayers = new List<string>();
            mPlayersFoundState = new List<bool>();
            mListPlayer = new List<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ValidateNumPlayer()
        {
            int.TryParse(inputNumPlayer.text, out mNumPlayer); 
        }

        public void AddPlayer(Player iPlayer)
        {
            mListPlayer.Add(iPlayer);
            mPlayersFoundState.Add(false);
            mNumPlayer++;
        }

        public Player GetPlayer(int iIndex)
        {
            if (iIndex >= 0 && iIndex < mListPlayer.Count)
            {
                return mListPlayer[iIndex];
            }
            else
                return null;
        }

        public void AddOnePlayer(string iName)
        {
            mNamesPlayers.Add(iName);
            mPlayersFoundState.Add(false);
            mNumPlayer++;
        }

        public bool DeleteOnePlayer(int iNumPlayer)
        {
            Debug.Log("num player found ou pas: " + mPlayersFoundState.Count);
            bool lHasAlreadyFound = false;
            if (!mPlayersFoundState[iNumPlayer])
            {
                mPlayersFoundState[iNumPlayer] = true;
                mNumPlayer--;
            }
            else
                lHasAlreadyFound = true;
            return lHasAlreadyFound;

        }
    }
}