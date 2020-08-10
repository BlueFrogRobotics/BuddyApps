using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class ConnectingState : AStateMachineBehaviour
    {
        private bool mListDone;
        private RTMManager mRTMManager;
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
            Debug.LogError("<color=red>00 - connecting state  : </color>");
            mRTMManager = GetComponent<RTMManager>();
            mInputFilter = GetGameObject(19).GetComponent<InputField>();
            GetGameObject(21).GetComponent<Button>().onClick.AddListener(() => { Trigger("CONNECTING"); });
            mUsers = new List<GameObject>();
            mPingTime = new List<float>();
            mWaitPing = new List<int>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            for (int i = 0; i < mUsers.Count; i++)
            {
                Destroy(mUsers[i]);
            }
            mRTMManager.OnPingWithId = null;
            mListDone = false;
            Debug.LogError("<color=red>01 - connecting state  : </color>");
            DBManager.Instance.IsCheckPlanning = false;
            mTimerRefreshPing = 0F;

            mPingStarted = 0F;
            mIsTimerStarted = false;
            Debug.LogError("<color=red>02 - connecting state  : </color>");
            CoursTelepresenceData.Instance.ConnectivityProblem = ConnectivityProblem.LaunchDatabase;
            GetGameObject(20).SetActive(false);
            Debug.LogError("<color=red>03 - connecting state  : </color>");
            GameObject NameStudent = GetGameObject(14).transform.GetChild(0).GetChild(0).gameObject;
            GameObject FirstNameStudent = GetGameObject(14).transform.GetChild(0).GetChild(1).gameObject;
            GameObject ClassStudent = GetGameObject(14).transform.GetChild(1).GetChild(0).gameObject;
            Debug.LogError("<color=red>04 - connecting state  : </color>");
            NameStudent.GetComponent<Text>().text = " ";
            FirstNameStudent.GetComponent<Text>().text = " ";
            ClassStudent.GetComponent<Text>().text = " ";
            Debug.LogError("<color=red>05 - connecting state  : </color>");
            GetGameObject(21).SetActive(false);
            mDisplayList = false;
            Debug.LogError("<color=red>06 - connecting state  : </color>");
            //Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => { 
            //    TText lText = iBuilder.CreateWidget<TText>();
            //    lText.SetLabel(Buddy.Resources.GetString("educonnectingdb")); 
            //});
            //GetGameObject(17).SetActive(true);


            mUsers.Clear();
            mPingTime.Clear();
            Debug.LogError("<color=red>07 - connecting state  : </color>");
            //mInputFilter.onValueChanged.RemoveAllListeners();
            mInputFilter.onValueChanged.AddListener(delegate { OnInputChanged(); });
            Buddy.GUI.Header.DisplayParametersButton(true);
            Debug.LogError("<color=red>08 - connecting state  : </color>");
            
            mRTMManager.OnPingWithId = (lId) =>
            {
                
                if (mTimerRefreshPing > 5F)
                {
                    //Debug.LogError("TIMER REFRESH : " + mTimerRefreshPing);
                    UpdateListUsers(lId);
                    mTimerRefreshPing = 0F;

                    
                }
                Debug.LogError("<color=red>09 - connecting state  : </color>");
            };
            Debug.LogError("Connecting state");
            //mRTMManager.OnPingWithId = UpdateListUsers;//OnPingId;

            //for (int i = 0; i < mUsers.Count; i++)
            //{
            //    Destroy(mUsers[i]);
            //}
            

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
            //Debug.LogError("CONNECTING STATE : Peering - " + DBManager.Instance.Peering + " | info - " + DBManager.Instance.InfoRequestedDone + " | mListDone - " + mListDone + " | CanStartCourse - " + DBManager.Instance.CanStartCourse);
            if (((DBManager.Instance.Peering && DBManager.Instance.InfoRequestedDone ) || DBManager.Instance.CanStartCourse) && CoursTelepresenceData.Instance.InitializeDone && !mListDone )
            {
                Debug.LogError("<color=red>00 - IF CONNECTING STATE </color>");

                Debug.LogError("<color=red>1 - IF CONNECTING STATE LISTUIDTABLET COUNT  : " + DBManager.Instance.ListUIDTablet.Count + "</color>");
                //Buddy.GUI.Toaster.Hide();
                if (DBManager.Instance.ListUIDTablet.Count == 1)
                {
                    Debug.LogError("<color=red>LIST COUNT EGAL 1</color>");
                    mListDone = true;
                    ButtonClick(0);
                }
                else
                {
                    Debug.LogError("<color=red>LIST COUNT PLUS GRAND QUE 1</color>");
                    if (!mDisplayList)
                    {
                        mDisplayList = true;
                        GetGameObject(17).SetActive(true);

                    }
                    Debug.LogError("<color=red>CONNECTING STATE : " + DBManager.Instance.ListUIDTablet.Count + "</color>");
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
                        lButtonUser.transform.GetChild(3).GetComponent<Text>().text = "";
                        lButtonUser.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                        lButtonUser.transform.GetChild(2).GetComponent<Image>().color = new Color(200, 0, 0);
                        lButtonUser.transform.GetChild(4).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        //mRTMManager.AskAvailable(DBManager.Instance.ListUIDTablet[i]);
                        Debug.LogWarning("tablette: " + DBManager.Instance.ListUIDTablet[i]);
                        mRTMManager.Ping(DBManager.Instance.ListUIDTablet[i], i);
                        Debug.LogError("LIST USER TIMER " + " index : " + i + " " + Time.time);
                        mPingTime.Add(Time.time);
                        mWaitPing.Add(0);
                    }
                    mListDone = true;
                }
            }

            if (mListDone)
            {
                //if (mTimerRefreshPing > 5F)
                //{
                //    mTimerRefreshPing = 0F;
                    //Debug.LogError("REFRESH PING-----------------");
                    for (int i = 0; i < mUsers.Count; i++)
                    {
                        float lTime = (Time.time - mPingTime[i]) /** 1000F*/;
                        
                        if (lTime > 5F)
                        {
                        //Debug.LogError("*************LTIME BEFORE UPDATE LIST USER " + lTime + " DBMANAGER  LIST UID TABLET : " + DBManager.Instance.ListUIDTablet[i]);
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
                            //Debug.LogError("MPINGTIME " + mPingTime[i]);
                            //UpdateListUsers(i);
                        }

                    }
                //}
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
            //Buddy.GUI.Toaster.Hide();
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
            Debug.Log("Connecting state exit");
        }

        private void ButtonClick(int iIndexList)
        {
            DBManager.Instance.FillPlanningStart(DBManager.Instance.ListUserStudent[iIndexList].Nom, DBManager.Instance.ListUserStudent[iIndexList].Prenom);
            mRTMManager.SetTabletId(DBManager.Instance.ListUIDTablet[iIndexList]);
            mRTMManager.IndexTablet = iIndexList;
            DBManager.Instance.IndexPlanning = iIndexList;
            DBManager.Instance.IsCheckPlanning = true;
            Debug.LogError("<color=green> FILLPLANING START : " + DBManager.Instance.ListUserStudent[iIndexList].Nom + " LIST UID TABLET : " + DBManager.Instance.ListUIDTablet[iIndexList] + " index list : " + iIndexList + "</color>");
            GetGameObject(17).SetActive(false);
            Debug.LogError("*************TRIGGER IDLE DANS CONNECTING STATE");
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
            //Debug.LogError("PING WITH " + iUserId + " muser count : " + mUsers.Count + " >= userid : " + iUserId + " TIME.TIME : " + Time.time);
            if (mPingTime == null || mUsers == null || mUsers.Count <= iUserId)
                return;
            float lTime = (Time.time - mPingTime[iUserId])*1000F;
            Debug.LogError("lTIME : " + lTime);
            //if (mWaitPing[iUserId] == 0)
            //{
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
            //}
            //else
            //    mWaitPing[iUserId]--;

            mRTMManager.Ping(DBManager.Instance.ListUIDTablet[iUserId], iUserId);
            mPingTime[iUserId] = Time.time;
        }

        private Sprite SpriteNetwork(int lValue)
        {
            string mNetworkLevel = "00";
            Sprite lSprite = null;

            // Update icon
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