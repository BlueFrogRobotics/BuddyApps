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
                for(int i = 0; i < DBManager.Instance.ListUIDTablet.Count; ++i)
                {
                    GameObject lButtonUser = GameObject.Instantiate(GetGameObject(15));
                    lButtonUser.transform.parent = GetGameObject(16).transform;
                    //Name 
                    GetGameObject(17).GetComponent<Text>().text = DBManager.Instance.UserStudent.Nom + " - " + DBManager.Instance.UserStudent.Prenom;
                    GetGameObject(18).GetComponent<Text>().text = DBManager.Instance.UserStudent.Organisme;
                    Debug.LogError("CONNECTING STATE : BUTTON N° " + i);
                    lButtonUser.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { ButtonClick(i); });
                }
                mListDone = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Connecting state exit");
        }

        private void ButtonClick(int iIndexList)
        {
            mRTMManager.SetTabletId(DBManager.Instance.ListUIDTablet[iIndexList]);
            Trigger("IDLE");
        }
    }

}