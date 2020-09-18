using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.TeleBuddyQuatreDeux
{

    public sealed class ConnectingState : AStateMachineBehaviour
    {
        private bool mListDone;
        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private List<GameObject> mUsers;
        private List<float> mPingTime;
        private List<int> mWaitPing;

        private InputField mInputFilter;
        private bool mDisplayList;

        private float mTimerRefreshPing;

        private bool mIsTimerStarted;
        private float mPingStarted;

        override public void Start()
        {
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();
            mInputFilter = GetGameObject(19).GetComponent<InputField>();
            GetGameObject(21).GetComponent<Button>().onClick.AddListener(() => { Trigger("CONNECTING"); });
            mUsers = new List<GameObject>();
            mPingTime = new List<float>();
            mWaitPing = new List<int>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRTCManager.Leave();
            TeleBuddyQuatreDeuxData.Instance.CurrentState = TeleBuddyQuatreDeuxData.States.CONNECTING_STATE;
            for (int i = 0; i < mUsers.Count; i++)
            {
                Destroy(mUsers[i]);
            }
            mRTMManager.OnPingWithId = null;
            mListDone = false;
            DBManager.Instance.IsCheckPlanning = false;
            mTimerRefreshPing = 0F;

            mPingStarted = 0F;
            mIsTimerStarted = false;
            TeleBuddyQuatreDeuxData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
            GetGameObject(20).SetActive(false);
            GameObject NameStudent = GetGameObject(14).transform.GetChild(0).GetChild(0).gameObject;
            GameObject FirstNameStudent = GetGameObject(14).transform.GetChild(0).GetChild(1).gameObject;
            GameObject ClassStudent = GetGameObject(14).transform.GetChild(1).GetChild(0).gameObject;
            NameStudent.GetComponent<Text>().text = " ";
            FirstNameStudent.GetComponent<Text>().text = " ";
            ClassStudent.GetComponent<Text>().text = " ";
            GetGameObject(21).SetActive(false);
            mDisplayList = false;
            mUsers.Clear();
            mPingTime.Clear();
            mInputFilter.onValueChanged.AddListener(delegate { OnInputChanged(); });
            Buddy.GUI.Header.DisplayParametersButton(true);
            
            mRTMManager.OnPingWithId = (lId) =>
            {
                if (mTimerRefreshPing > 5F)
                {
                    UpdateListUsers(lId);
                    mTimerRefreshPing = 0F;
                }
            };

            if (Buddy.Behaviour.Mood != Mood.NEUTRAL)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mIsTimerStarted)
            {
                mPingStarted += Time.deltaTime;
            }
            mTimerRefreshPing += Time.deltaTime;
            if (((DBManager.Instance.Peering && DBManager.Instance.InfoRequestedDone ) || DBManager.Instance.CanStartCourse) && TeleBuddyQuatreDeuxData.Instance.InitializeDone && !mListDone )
            {
                if (DBManager.Instance.ListUIDTablet.Count == 1)
                {
                    mListDone = true;
                    ButtonClick(0);
                }
                else
                {
                    if (!mDisplayList)
                    {
                        mDisplayList = true;
                        GetGameObject(17).SetActive(true);

                    }
                    for (int i = 0; i < DBManager.Instance.ListUIDTablet.Count; ++i)
                    {
                        GameObject lButtonUser = GameObject.Instantiate(GetGameObject(15));
                        lButtonUser.transform.parent = GetGameObject(16).transform;
                        GetGameObject(16).transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = DBManager.Instance.ListUserStudent[i].Nom + " - " + DBManager.Instance.ListUserStudent[i].Prenom;
                        GetGameObject(16).transform.GetChild(i).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = DBManager.Instance.ListUserStudent[i].Organisme;
                        int lIndex = i;
                        lButtonUser.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { ButtonClick(lIndex); });
                        mUsers.Add(lButtonUser);
                        lButtonUser.transform.GetChild(3).GetComponent<Text>().text = "";
                        lButtonUser.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                        lButtonUser.transform.GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                        lButtonUser.transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        mRTMManager.Ping(DBManager.Instance.ListUIDTablet[i], i);
                        mPingTime.Add(Time.time);
                        mWaitPing.Add(0);
                    }
                    mListDone = true;
                }
            }

            if (mListDone)
            {
                for (int i = 0; i < mUsers.Count; i++)
                {
                    float lTime = (Time.time - mPingTime[i]) /** 1000F*/;
                        
                    if (lTime > 5F)
                    {
                    mRTMManager.Ping(DBManager.Instance.ListUIDTablet[i], i);
                        mPingTime[i] = Time.time;
                        if (mPingStarted > 6F)
                        {
                            mIsTimerStarted = false;
                            mUsers[i].transform.GetChild(3).GetComponent<Text>().text = "";
                            mUsers[i].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                            mUsers[i].transform.GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                            mUsers[i].transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        }
                    }
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(DBManager.Instance.ListUIDTablet.Count > 1)
            {
                GetGameObject(21).SetActive(true);
            }

            mListDone = false;
            mDisplayList = false;
            mRTMManager.OnPingWithId = null;
            for(int i=0; i<mUsers.Count; i++)
            {
                Destroy(mUsers[i]);
            }
            mUsers.Clear();
            mPingTime.Clear();
            mInputFilter.onValueChanged.RemoveAllListeners();
            ResetTriggerAnim("CONNECTING");
            ResetTriggerAnim("INCOMING CALL");
        }

        private void ButtonClick(int iIndexList)
        {
            DBManager.Instance.FillPlanningStart(DBManager.Instance.ListUserStudent[iIndexList].Nom, DBManager.Instance.ListUserStudent[iIndexList].Prenom);
            mRTMManager.SetTabletId(DBManager.Instance.ListUIDTablet[iIndexList]);
            mRTMManager.IndexTablet = iIndexList;
            DBManager.Instance.IndexPlanning = iIndexList;
            DBManager.Instance.IsCheckPlanning = true;
            GetGameObject(17).SetActive(false);
            Trigger("IDLE");
        }

        private void OnPingId(int iId)
        {
            Debug.LogWarning("PING WITH " + iId);
        }

        private void UpdateListUsers(int iUserId)
        {
            mPingStarted = 0F;
            mIsTimerStarted = true;
            if (mPingTime == null || mUsers == null || mUsers.Count <= iUserId)
                return;
            float lTime = (Time.time - mPingTime[iUserId])*1000F;
            mUsers[iUserId].transform.GetChild(4).GetComponent<Image>().sprite = SpriteNetwork((int)lTime);
            if(lTime == 0)
            {
                mUsers[iUserId].transform.GetChild(3).GetComponent<Text>().text = "";
                mUsers[iUserId].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                mUsers[iUserId].transform.GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                mUsers[iUserId].transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            else if (lTime >= 500)
            {
                mUsers[iUserId].transform.GetChild(3).GetComponent<Text>().text = "";
                mUsers[iUserId].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                mUsers[iUserId].transform.GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                mUsers[iUserId].transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            else
            {
                mUsers[iUserId].transform.GetChild(3).GetComponent<Text>().text = "" + (int)lTime + "ms";
                mUsers[iUserId].transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(0, 200, 200);
                mUsers[iUserId].transform.GetChild(2).GetComponent<Image>().color = new Color(0, 200, 200);
                mUsers[iUserId].transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            mWaitPing[iUserId] = 4;
            mRTMManager.Ping(DBManager.Instance.ListUIDTablet[iUserId], iUserId);
            mPingTime[iUserId] = Time.time;
        }

        private Sprite SpriteNetwork(int lValue)
        {
            string mNetworkLevel = "00";
            Sprite lSprite = null;

            if(lValue == 0)
            {
                mNetworkLevel = "00";
                lSprite = null;
            }
            else if (lValue < 60)
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

        private void OnInputChanged()
        {
            for(int i=0; i< mUsers.Count; i++)
            {
                if(DBManager.Instance.ListUserStudent[i].Nom.ToLower().Contains(mInputFilter.text.ToLower()) || DBManager.Instance.ListUserStudent[i].Prenom.ToLower().Contains(mInputFilter.text.ToLower()))
                {
                    mUsers[i].SetActive(true);
                }
                else
                    mUsers[i].SetActive(false);
            }
        }
    }
}