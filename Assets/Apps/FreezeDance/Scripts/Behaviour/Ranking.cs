using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System.IO;

namespace BuddyApp.FreezeDance
{
    public class Ranking : MonoBehaviour
    {

        [SerializeField]
        private GameObject firstUser;

        [SerializeField]
        private GameObject secondUser;

        [SerializeField]
        private GameObject thirdUser;

        [SerializeField]
        private GameObject otherUser;

        [SerializeField]
        private GameObject rankPlaceholder;

        [SerializeField]
        private GameObject title;

        [SerializeField]
        private GameObject bestScore;

        private PlayerList mPlayerList;

        // Use this for initialization
        void Start()
        {
            mPlayerList = new PlayerList();
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            //Debug.Log(lDirectoryPath);
            //Utils.SerializeCSV(lDirectoryPath, "truc", "machin");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddPlayer(int iScore)
        {
            PlayerScore lPlayer = new PlayerScore();
            lPlayer.Score = iScore;
            lPlayer.Name = BYOS.Instance.DataBase.GetCurrentUser().FirstName;
            mPlayerList.List.Add(lPlayer);
            string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            //Directory.CreateDirectory(lDirectoryPath);
            //Utils.SerializeCSV(lDirectoryPath, "truc", "machin");
            int a = 1;
            Utils.SerializeXML<PlayerList>(mPlayerList, lDirectoryPath);
        }

        public void ShowRanking()
        {
            BYOS.Instance.Toaster.Display<BackgroundToast>().With();
            bestScore.GetComponent<Animator>().SetTrigger("open");
        }

        public void HideRanking()
        {
            BYOS.Instance.Toaster.Hide();
            bestScore.GetComponent<Animator>().SetTrigger("close");
        }
    }
}