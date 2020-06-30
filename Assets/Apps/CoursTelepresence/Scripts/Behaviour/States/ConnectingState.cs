using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class ConnectingState : AStateMachineBehaviour
    {
        [SerializeField]
        private Animator ConnectingScreenAnimator;
        private bool mListDone;
        private RTMManager mRTMManager;
        private List<GameObject> mUsers;
        private List<float> mPingTime;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
            Debug.Log("Connecting state");
            mRTMManager = GetComponent<RTMManager>();
            mRTMManager.OnPingWithId = OnPingId;
            mListDone = false;
            mUsers = new List<GameObject>();
            mPingTime = new List<float>();
            //TODO check DB and stuff

            if (Buddy.Behaviour.Mood != Mood.NEUTRAL)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            //Debug.LogWarning("Peering " + DBManager.Instance.Peering + " info " + DBManager.Instance.InfoRequestedDone);
            if (DBManager.Instance.Peering && DBManager.Instance.InfoRequestedDone && !mListDone)
            {
                if (DBManager.Instance.ListUIDTablet.Count == 1)
                {
                    mListDone = true;
                    ButtonClick(0);
                }
                else
                {
                    GetGameObject(17).SetActive(true);
                    for (int i = 0; i < DBManager.Instance.ListUIDTablet.Count; ++i)
                    {
                        GameObject lButtonUser = GameObject.Instantiate(GetGameObject(15));
                        lButtonUser.transform.parent = GetGameObject(16).transform;
                        //Name 
                        //Debug.LogError("<color=green>" + GetGameObject(16).transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).ToString() + "</color>");
                        GetGameObject(16).transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = DBManager.Instance.ListUserStudent[i].Nom + " - " + DBManager.Instance.ListUserStudent[i].Prenom;
                        GetGameObject(16).transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = DBManager.Instance.ListUserStudent[i].Organisme;
                        int lIndex = i;
                        lButtonUser.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { ButtonClick(lIndex); });
                        mUsers.Add(lButtonUser);
                        //mRTMManager.AskAvailable(DBManager.Instance.ListUIDTablet[i]);
                        mRTMManager.Ping(DBManager.Instance.ListUIDTablet[i], i);
                        mPingTime.Add(Time.time);
                    }
                    mListDone = true;
                }
            }

            for(int i=0; i<mUsers.Count; i++)
            {
                float lTime = (Time.time - mPingTime[i]) * 1000F;
                if (lTime > 300)
                    UpdateListUsers(i);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRTMManager.OnPingWithId = null;
            Debug.Log("Connecting state exit");
        }

        private void ButtonClick(int iIndexList)
        {
            DBManager.Instance.FillPlanningStart(DBManager.Instance.ListUserStudent[iIndexList].Nom);
            mRTMManager.SetTabletId(DBManager.Instance.ListUIDTablet[iIndexList]);
            GetGameObject(17).SetActive(false);
            Trigger("IDLE");
        }

        private void OnPingId(int iId)
        {
            Debug.LogWarning("PING WITH " + iId);
        }

        private void UpdateListUsers(int iUserId)
        {
            float lTime = (Time.time - mPingTime[iUserId])*1000F;
            mUsers[iUserId].transform.GetChild(4).GetComponent<Image>().sprite = SpriteNetwork((int)lTime);
            mUsers[iUserId].transform.GetChild(3).GetComponent<Text>().text = "" + lTime + "ms";
            if (lTime > 300)
                mUsers[iUserId].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
            else 
                mUsers[iUserId].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(0, 200, 200); 


            mRTMManager.Ping(DBManager.Instance.ListUIDTablet[iUserId], iUserId);
            mPingTime[iUserId] = Time.time;
        }

        private Sprite SpriteNetwork(int lValue)
        {
            string mNetworkLevel = "00";
            Sprite lSprite;

            // Update icon
            if (lValue < 60)
            {
                    mNetworkLevel = "04";
                    lSprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }
            else if (lValue < 100)
            {
                    mNetworkLevel = "03";
                    lSprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }
            else if (lValue < 150)
            {
                    mNetworkLevel = "02";
                    lSprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }
            else if (lValue < 200)
            {
                    mNetworkLevel = "01";
                    lSprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }
            else
            {
                mNetworkLevel = "00";
                lSprite = Buddy.Resources.Get<Sprite>("Atlas_Education_IconSignal" + mNetworkLevel, Context.APP);
            }

            return lSprite;
        }
    }

}