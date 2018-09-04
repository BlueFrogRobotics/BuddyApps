using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Tutorial
{
    /// <summary>
    /// In this state we show you how to leave an application with a message displayed in a toast + the vocal to do that.
    /// It's important for the user to have the choice between the touch screen and the vocal so try to put both on your application (You don't have to but it's a good practice)
    /// But you also can quit the app when you press the button Quit without another state with text displayed ect.
    /// </summary>
    public class QuitState : AStateMachineBehaviour
    {
        private int mNumberListen;
        //The field 
        [SerializeField]
        private int mMaxNumberOfListen;
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //We need Buddy to listen at least one time, if the developer forgot to enter the number of listen we initialize it at minimum 1.
            if (mMaxNumberOfListen == 0)
                mMaxNumberOfListen = 1;
            mNumberListen = 0;
            
            //ParameterToast can display many of our widgets and you also can display two buttons and set an action when you click on it. 
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                //First you put the widget you want to use, there is a lot of widget option, they will be listed on our website.
                iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("quit"));
            },
                //Then you can add Action to the two buttons and label on those buttons.
                () => {
                     Debug.Log("Click no");
                     Buddy.GUI.Toaster.Hide();
                     Trigger("MenuTrigger");
                },  Buddy.Resources.GetString("no"),

                () => {
                     Debug.Log("Click yes");
                     Buddy.GUI.Toaster.Hide();
                     QuitApp();
                },  Buddy.Resources.GetString("yes")

            );
            Buddy.Vocal.Say(Buddy.Resources.GetString("quit"));
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //We check if the vocal part of Buddy is busy so we know if we can lauch a vocal command.
            if(!Buddy.Vocal.IsBusy)
            {
                if (mNumberListen < mMaxNumberOfListen)
                {
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("quit"), null, (iInput) => {

                        if (Buddy.Vocal.LastHeardInput.Utterance == Buddy.Resources.GetString("yes").ToLower())
                        {
                            Buddy.GUI.Toaster.Hide();
                            QuitApp();
                        }
                        else if (Buddy.Vocal.LastHeardInput.Utterance == Buddy.Resources.GetString("no").ToLower())
                        {
                            Buddy.GUI.Toaster.Hide();
                            Trigger("MenuTrigger");
                        }
                    });
                    mNumberListen++;
                }
                else
                    QuitApp();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}

