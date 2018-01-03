using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        [SerializeField]
        private GameObject goToMenu;

        [SerializeField]
        private GameObject replay;

        public UnityEngine.UI.Button GoToMenu { get { return goToMenu.GetComponent<UnityEngine.UI.Button>(); } }

        public UnityEngine.UI.Button Replay { get { return replay.GetComponent<UnityEngine.UI.Button>(); } }

        private PlayerList mPlayerList;

        private List<GameObject> mScoreElements;

        private int mLastRank=0;

        // Use this for initialization
        void Start()
        {
            mScoreElements = new List<GameObject>();
            string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            mPlayerList = Utils.UnserializeXML<PlayerList>(lDirectoryPath);

            if (mPlayerList == null)
            {
                mPlayerList = new PlayerList();
            }
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
            Debug.Log("add player");
            PlayerScore lPlayer = new PlayerScore();
            lPlayer.Score = iScore;
            lPlayer.Name = BYOS.Instance.DataBase.GetCurrentUser().FirstName;
            mPlayerList.List.Add(lPlayer);
            string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            //Directory.CreateDirectory(lDirectoryPath);
            //Utils.SerializeCSV(lDirectoryPath, "truc", "machin");
            ClearRanking();
            UpdateRanking();
            mLastRank = mPlayerList.List.IndexOf(lPlayer);
            
            Utils.SerializeXML<PlayerList>(mPlayerList, lDirectoryPath);
        }

        public void ShowRanking()
        {
            BYOS.Instance.Toaster.Display<BackgroundToast>().With();
            bestScore.GetComponent<Animator>().SetTrigger("open");
            ClearRanking();
            UpdateRanking();
            int lRank = mLastRank + 1;
            switch(mLastRank)
            {
                case 0:
                    title.GetComponent<Text>().text = BYOS.Instance.Dictionary.GetString("nicescore") + lRank + " " + BYOS.Instance.Dictionary.GetString("first");
                    break;
                case 1:
                    title.GetComponent<Text>().text = BYOS.Instance.Dictionary.GetString("nicescore") + lRank + " " + BYOS.Instance.Dictionary.GetString("second");
                    break;
                case 2:
                    title.GetComponent<Text>().text = BYOS.Instance.Dictionary.GetString("nicescore") + lRank + " " + BYOS.Instance.Dictionary.GetString("third");
                    break;
                default:
                    title.GetComponent<Text>().text = BYOS.Instance.Dictionary.GetString("nicescore") + lRank + " " + BYOS.Instance.Dictionary.GetString("th");
                    break;
            }
            
            //for(int i=0; i<mPlayerList.List.Count; i++)
            //{
            //    if(i==mLastRank)
            //        mScoreElements[i].GetComponent<RankItem>().HighLight = true;
            //    else
            //        mScoreElements[i].GetComponent<RankItem>().HighLight = false;
            //}
        }

        public void HideRanking()
        {
            BYOS.Instance.Toaster.Hide();
            bestScore.GetComponent<Animator>().SetTrigger("close");
        }

        public void ResetRanking()
        {
            mPlayerList.List.Clear();
            ClearRanking();
            UpdateRanking();
            Debug.Log("reset: " + mPlayerList.List.Count);
            string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            Utils.SerializeXML<PlayerList>(mPlayerList, lDirectoryPath);
        }

        private void UpdateRanking()
        {
            mPlayerList.List.Sort((x, y) => x.Score.CompareTo(y.Score));
            mPlayerList.List.Reverse();
            for (int i=0; i<mPlayerList.List.Count; i++)
            {
                PlayerScore lScore = mPlayerList.List[i];
                GameObject lItem = null;//Instantiate(otherUser);
                switch(i)
                {
                    case 0:
                        lItem = Instantiate(firstUser);
                        break;
                    case 1:
                        lItem = Instantiate(secondUser);
                        break;
                    case 2:
                        lItem = Instantiate(thirdUser);
                        break;
                    default:
                        lItem = Instantiate(otherUser);
                        break;
                }
                lItem.transform.parent = rankPlaceholder.transform;
                mScoreElements.Add(lItem);
                RankItem lRank = lItem.GetComponent<RankItem>();
                lRank.Name = lScore.Name;
                lRank.Score = lScore.Score;
                lRank.Place = (i + 1);
                if (i == mLastRank)
                    lRank.HighLight = true;
                else
                    lRank.HighLight = false;
            }
        }

        private void ClearRanking()
        {
            foreach(GameObject score in mScoreElements)
            {
                Destroy(score);
            }

            mScoreElements.Clear();
        }
    }
}