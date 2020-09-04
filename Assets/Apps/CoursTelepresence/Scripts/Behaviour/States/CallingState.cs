using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

using IEnumerator = System.Collections.IEnumerator;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallingState : AStateMachineBehaviour
    {
        private RTMManager mRTMManager;
        private float mTimeState;
        private bool mCallRefusedDisplayed;

        override public void Start()
        {
            mCallRefusedDisplayed = false;
            mRTMManager = GetComponent<RTMManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CoursTelepresenceData.Instance.CurrentState = CoursTelepresenceData.States.CALLING_STATE;
            mTimeState = Time.time;

            mRTMManager.OncallRequestAnswer = (lCallAnswer) => {
                if (lCallAnswer)
                    Trigger("CALL");
                else if(!mCallRefusedDisplayed)
                {
                    mCallRefusedDisplayed = true;
                    Buddy.GUI.Dialoger.Display<IconToast>("Appel refusé").
                    With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                    () => {
                        Buddy.GUI.Dialoger.Hide();
                        Trigger("IDLE");
                    },
                    () => {
                        StartCoroutine(RefusedCall());
                    },
                    () => {
                    }
                    );
                }
            };
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Time.time - mTimeState > 60F && !Buddy.GUI.Dialoger.IsBusy)
            {
                Buddy.GUI.Dialoger.Display<IconToast>("Echec de connexion").
                With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                () =>
                {
                    Trigger("IDLE");
                    Buddy.GUI.Dialoger.Hide();
                },
                () =>
                {
                    StartCoroutine(HideUI());
                },
                () =>
                {
                    Trigger("IDLE");
                    Buddy.GUI.Dialoger.Hide();
                }
                );
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRTMManager.OncallRequestAnswer = null;
            mCallRefusedDisplayed = false;
        }

        private IEnumerator RefusedCall()
        {
            yield return new WaitForSeconds(2);
            Buddy.GUI.Dialoger.Hide();
            Trigger("IDLE");
        }

        private IEnumerator HideUI()
        {
            yield return new WaitForSeconds(2F);
            Buddy.GUI.Dialoger.Hide();
            Trigger("IDLE");
        }
    }

}