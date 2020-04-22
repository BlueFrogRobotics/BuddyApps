using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{

    public sealed class CallingState : AStateMachineBehaviour
    {
        private RTMCom mRTMCom;

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMCom = GetComponent<RTMCom>();
            mRTMCom.OncallRequestAnswer = (lCallAnswer) => {
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


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("calling state");
            Debug.Log("sprite " + Buddy.Resources.Get<Sprite>("Calling"));
            Buddy.GUI.Toaster.Display<PictureToast>().With(Buddy.Resources.Get<Sprite>("Calling"));



            mRTMCom.OncallRequestAnswer = (lCallAnswer) => {
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

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("calling state exit");
            mRTMCom.OncallRequestAnswer = null;
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
        }
    }

}