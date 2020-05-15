using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallingState : AStateMachineBehaviour
    {
        private RTMManager mRTMManager;
        private float mTimeState;

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMManager = GetComponent<RTMManager>();
            //mRTMManager.OncallRequestAnswer = (lCallAnswer) => {
            //    if (lCallAnswer)
            //        Trigger("CALL");
            //    else
            //        Buddy.GUI.Dialoger.Display<IconToast>("Appel refusé").
            //        With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
            //            () => {
            //                Trigger("IDLE");
            //                Buddy.GUI.Dialoger.Hide();
            //            },
            //            null,
            //            () => {
            //                Trigger("IDLE");
            //                Buddy.GUI.Dialoger.Hide();
            //            }
            //            );
            //};
        }


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.LogError("calling state");

            mTimeState = Time.time;

            mRTMManager.OncallRequestAnswer = (lCallAnswer) => {
                if (lCallAnswer)
                    Trigger("CALL");
                else
                    Buddy.GUI.Dialoger.Display<IconToast>("Appel refusé").
                    With(Buddy.Resources.Get<Sprite>("os_icon_phoneoff_big"),
                        () => {
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        },
                        null,
                        () => {
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        }
                        );
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
                        null,
                        () =>
                        {
                            Trigger("IDLE");
                            Buddy.GUI.Dialoger.Hide();
                        }
                        );
                Debug.LogError("temps: "+ (Time.time - mTimeState));
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("calling state exit");
            mRTMManager.OncallRequestAnswer = null;
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
        }
    }

}