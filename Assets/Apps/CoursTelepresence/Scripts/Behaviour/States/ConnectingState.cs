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

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
            Debug.Log("Connecting state");
            mRTMManager = GetComponent<RTMManager>();
            mRTMManager.OnPingWithId = OnPingId;
            mListDone = false;
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
                        //mRTMManager.AskAvailable(DBManager.Instance.ListUIDTablet[i]);
                        mRTMManager.Ping(DBManager.Instance.ListUIDTablet[i], 0);
                    }
                    mListDone = true;
                }
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
    }

}