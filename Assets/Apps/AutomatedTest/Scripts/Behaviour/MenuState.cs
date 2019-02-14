using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class MenuState : AStateMachineBehaviour
    {
        //  Display each available module in test

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        public override void Start()
        {
            if (mButtonContent.Count == 0)
            {
                //mButtonContent.Add("Motions", "MotionTrigger");
                //mButtonContent.Add("Vocal", "VocalTrigger");
                mButtonContent.Add("Camera", "CameraTrigger");
                //mButtonContent.Add("GUI", "GUITrigger");
                mButtonContent.Add("Full Test", "FullTrigger");
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle("Automated Test");
            
            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {
                foreach (KeyValuePair<string, string> lButtonContent in mButtonContent)
                {
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    lBox.OnClick.Add(() => { Debug.Log("Click " + lButtonContent.Key + " TRIGGER : " + lButtonContent.Value); Trigger(lButtonContent.Value); });
                    lBox.SetLabel(lButtonContent.Key);
                    lBox.LeftButton.Hide();
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                }
            });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

