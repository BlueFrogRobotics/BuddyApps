using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

using System.IO;

namespace BuddyApp.FreezeDance
{
    public class Ranking : MonoBehaviour
    {
        private PlayerList mPlayerList;

        public PlayerList GetPlayerList()
        {
            return mPlayerList;
        }

        private int mLastRank = 0;

        // Use this for initialization
        void Start()
        {
            string lDirectoryPath = Buddy.Resources.AppRawDataPath + "scores.xml";
            mPlayerList = Utils.UnserializeXML<PlayerList>(lDirectoryPath);

            if (mPlayerList == null)
            {
                mPlayerList = new PlayerList();
            }
        }

        public void AddPlayer(int iScore)
        {
            AddPlayer(iScore, "");
        }

        public void AddPlayer(int iScore, string name)
        {
            PlayerScore lPlayer = new PlayerScore();
            lPlayer.Score = iScore;
            lPlayer.Name = name;
            mPlayerList.List.Add(lPlayer);

            mPlayerList.List.Sort((x, y) => x.Score.CompareTo(y.Score));
            mPlayerList.List.Reverse();

            mLastRank = mPlayerList.List.IndexOf(lPlayer);
            SaveScores();
        }

        public void RemovePlayer(PlayerScore lPlayer)
        {
            mPlayerList.List.Remove(lPlayer);
            SaveScores();
        }

        private void SaveScores()
        {
            string lDirectoryPath = Buddy.Resources.AppRawDataPath + "scores.xml";
            Utils.SerializeXML<PlayerList>(mPlayerList, lDirectoryPath);
        }

        public void ResetRanking()
        {
            mPlayerList.List.Clear();
            Debug.Log("reset: " + mPlayerList.List.Count);
            SaveScores();
        }

    }
}