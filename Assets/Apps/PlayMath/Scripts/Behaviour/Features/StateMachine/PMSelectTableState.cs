using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;

namespace BuddyApp.PlayMath{
    public class PMSelectTableState : AnimatorSyncState {

        private Animator mSetTableAnimator;

        private bool mIsOpen;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            mSetTableAnimator = GameObject.Find("UI/Set_Table").GetComponent<Animator>();

            mPreviousStateBehaviours.Add(GameObject.Find("UI/Menu").GetComponent<MainMenuBehaviour>());

            mIsOpen = false;

            mSetTableAnimator.gameObject.GetComponent<SelectTableBehaviour>().InitState();
            SetTablesStarProgress();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mSetTableAnimator.SetTrigger("open");
                mIsOpen = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            mSetTableAnimator.SetTrigger("close");
            mIsOpen = false;
        }

        private void SetTablesStarProgress()
        {
            Text[] childs = GameObject.Find("UI/Set_Table/Middle_UI").GetComponentsInChildren<Text>();
            foreach(Text child in childs)
            {
                int table = int.Parse(Regex.Match(child.text,@"\d+").Value);
                GameObject StarProgress = child.gameObject.transform.parent.Find("Star_Progress").gameObject;
                StarProgress.transform.Find("Mask_10").gameObject.SetActive(User.Instance.HasTableCertificate(table));
            }
        }
    }
}