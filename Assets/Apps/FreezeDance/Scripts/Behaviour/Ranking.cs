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
        //    [SerializeField]
        //    private Animator anim; 

        //    [SerializeField]
        //    private GameObject firstUser;

        //    [SerializeField]
        //    private GameObject secondUser;

        //    [SerializeField]
        //    private GameObject thirdUser;

        //    [SerializeField]
        //    private GameObject otherUser;

        //    [SerializeField]
        //    private GameObject rankPlaceholder;

        //    [SerializeField]
        //    private GameObject title;

        //    [SerializeField]
        //    private GameObject bestScore;

        //    [SerializeField]
        //    private GameObject goToMenu;

        //    [SerializeField]
        //    private GameObject replay;

        //    public UnityEngine.UI.Button GoToMenu { get { return goToMenu.GetComponent<UnityEngine.UI.Button>(); } }

        //    public UnityEngine.UI.Button Replay { get { return replay.GetComponent<UnityEngine.UI.Button>(); } }

        private PlayerList mPlayerList;

        public PlayerList GetPlayerList()
        {
            return mPlayerList;
        }

        //private List<GameObject> mScoreElements;

        private int mLastRank = 0;

        // Use this for initialization
        void Start()
        {
            //mScoreElements = new List<GameObject>();
            string lDirectoryPath = Buddy.Resources.AppRawDataPath + "scores.xml";
            mPlayerList = Utils.UnserializeXML<PlayerList>(lDirectoryPath);

            if (mPlayerList == null)
            {
                mPlayerList = new PlayerList();
            }
            
            //string lDirectoryPath = BYOS.Instance.Resources.GetPathToRaw("scores.xml");
            //Debug.Log(lDirectoryPath);
            //Utils.SerializeCSV(lDirectoryPath, "truc", "machin");
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
            
            //Directory.CreateDirectory(lDirectoryPath);
            //Utils.SerializeCSV(lDirectoryPath, "truc", "machin");
            //ClearRanking();
            //UpdateRanking();
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

        //public void ShowRanking()
        //{
        //    //BYOS.Instance.Toaster.Display<BackgroundToast>().With();
        //    //bestScore.GetComponent<Animator>().SetTrigger("open");
        //    Buddy.GUI.Header.DisplayLightTitle("Résultats");
        //    //ClearRanking();
        //    UpdateRanking();
            //int lRank = mLastRank + 1;

            //if (anim.GetBool("ScoreBool"))
            //{
            //    switch (mLastRank)
            //    {
            //        case 0:
            //            title.GetComponent<Text>().text = Buddy.Resources.GetString("nicescore") + lRank + " " + Buddy.Resources.GetString("first");
            //            break;
            //        case 1:
            //            title.GetComponent<Text>().text = Buddy.Resources.GetString("nicescore") + lRank + " " + Buddy.Resources.GetString("second");
            //            break;
            //        case 2:
            //            title.GetComponent<Text>().text = Buddy.Resources.GetString("nicescore") + lRank + " " + Buddy.Resources.GetString("third");
            //            break;
            //        default:
            //            title.GetComponent<Text>().text = Buddy.Resources.GetString("nicescore") + lRank + " " + Buddy.Resources.GetString("th");
            //            break;
            //    }
            //}
            //else
            //{
            //    title.GetComponent<Text>().text = "Score";
            //}
            //for(int i=0; i<mPlayerList.List.Count; i++)
            //{
            //    if(i==mLastRank)
            //        mScoreElements[i].GetComponent<RankItem>().HighLight = true;
            //    else
            //        mScoreElements[i].GetComponent<RankItem>().HighLight = false;
            //}
        //}

        //public void HideRanking()
        //{
        //    Buddy.GUI.Toaster.Hide();
        //    bestScore.GetComponent<Animator>().SetTrigger("close");
        //}

        public void ResetRanking()
        {
            mPlayerList.List.Clear();
            //ClearRanking();
            //UpdateRanking();
            Debug.Log("reset: " + mPlayerList.List.Count);
            SaveScores();
        }

        //private void UpdateRanking()
        //{
        //    mPlayerList.List.Sort((x, y) => x.Score.CompareTo(y.Score));
        //    mPlayerList.List.Reverse();
        //    Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
        //    {
        //        int i = 1;
        //        foreach (PlayerScore lScore in mPlayerList.List)
        //        {
        //            if (!string.IsNullOrEmpty(lScore.Name))
        //            {
        //                TVerticalListBox lBox = iBuilder.CreateBox();
        //                lBox.SetLabel(lScore.Name.ToUpper());
        //                Sprite sprite = Buddy.Resources.Get<Sprite>("os_grey_star", Context.OS);
        //                lBox.LeftButton.SetIcon(sprite);
        //                lBox.LeftButton.SetLabel((i).ToString());
        //                lBox.SetCenteredLabel(false);

        //                TRightSideButton scoreButton = lBox.CreateRightButton();
        //                scoreButton.SetLabel(lScore.Score.ToString());
        //                scoreButton.SetIconColor(Color.red);
        //                i++;
        //            }
        //        }
        //    });
            //for (int i=0; i<mPlayerList.List.Count; i++)
            //{
            //    PlayerScore lScore = mPlayerList.List[i];
            //    GameObject lItem = null;//Instantiate(otherUser);
            //    switch(i)
            //    {
            //        case 0:
            //            lItem = Instantiate(firstUser);
            //            break;
            //        case 1:
            //            lItem = Instantiate(secondUser);
            //            break;
            //        case 2:
            //            lItem = Instantiate(thirdUser);
            //            break;
            //        default:
            //            lItem = Instantiate(otherUser);
            //            break;
            //    }
            //    lItem.transform.parent = rankPlaceholder.transform;
            //    mScoreElements.Add(lItem);
            //    RankItem lRank = lItem.GetComponent<RankItem>();
            //    lRank.Name = lScore.Name;
            //    lRank.Score = lScore.Score;
            //    lRank.Place = (i + 1);
            //    if (i == mLastRank)
            //        lRank.HighLight = true;
            //    else
            //        lRank.HighLight = false;
            //}
        //}

        //private void ClearRanking()
        //{
        //    foreach(GameObject score in mScoreElements)
        //    {
        //        Destroy(score);
        //    }

        //    mScoreElements.Clear();
        //}
    }
}