using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

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
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddPlayer(int iScore)
        {
            PlayerScore lPlayer = new PlayerScore();
            lPlayer.Score = iScore;
            lPlayer.Name = BYOS.Instance.DataBase.GetUsers()[0].FirstName;
            mPlayerList.List.Add(lPlayer);
            Utils.SerializeXML<PlayerList>(mPlayerList, "");
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