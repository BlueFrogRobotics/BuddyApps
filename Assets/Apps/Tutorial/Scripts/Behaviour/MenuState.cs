using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Tutorial
{
    /// <summary>
    /// State for the menu : we display a menu with a vertical list toast. The vertical list toast allows the user to display a list of box vertically.
    /// And you have access to many parameters for these box like icons / actions / color of the writing...
    /// You will learn also how to use the speech to text so Buddy understand what you said.
    /// </summary>
    public class MenuState : AStateMachineBehaviour
    {
        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mButtonContent.Clear();
            mButtonContent.Add(Buddy.Resources.GetString("menumotion"), "MotionTrigger");
            mButtonContent.Add(Buddy.Resources.GetString("menuphoto"), "TakePhotoTrigger");
            mButtonContent.Add(Buddy.Resources.GetString("menuwidget"), "WidgetTrigger");
            mButtonContent.Add(Buddy.Resources.GetString("menuquit"), "QuitTrigger");

            //We have differents parts of the UI : Header - Toaster - footer.
            //Each of them control one part of the Buddy's screen, header controls the top, toaster will be all the widgets/UI in the middle and the footer controls the bottom.
            //That's why if you want a menu you need to use a toaster and if you want a title you need to use the header.
            //We have a button parameter always visible in the top right, but you can hide it if you don't need parameters in your application. We will have an example of use case on our website.
            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle("Menu");

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {
                foreach (KeyValuePair<string, string> lButtonContent in mButtonContent)
                {
                    //We create the container
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    //We create en event OnClick so we can trigger en event when we click on the box
                    lBox.OnClick.Add(() => { Debug.Log("Click " + lButtonContent.Key); Trigger(lButtonContent.Value); });
                    //We label our button with our informations in the dictionary
                    lBox.SetLabel(lButtonContent.Key);
                    //You can set a left button if you need to add en event or an icon at the left
                    lBox.LeftButton.Hide();
                    //We place the text of the button in the center of the box
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                }
            });

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

