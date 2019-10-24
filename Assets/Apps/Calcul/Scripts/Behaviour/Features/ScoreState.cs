using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.Calcul
{
    public class ScoreState : AStateMachineBehaviour
    {
        private Score mScore;
        private const int TIMEOUT = 20; // Quit app time out in seconds

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mScore = GetComponent<CalculBehaviour>().Score;

            // Display replay button
            FButton lButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_redo", Context.OS));
            lButton.OnClick.Add(() => {
                Trigger("StartGame");
            });

            DisplayScore();

            StartCoroutine(QuitOnTimeOut());
        }

        private void DisplayScore()
        {
            string key = "badscorespeech";
            if (mScore.SuccessPercent() == 1.0)
            {
                key = "perfectscore";
            }
            else if (mScore.SuccessPercent() >= 0.75)
            {
                key = "greatscorespeech";
            }
            else if (mScore.SuccessPercent() >= 0.5)
            {
                key = "goodscorespeech";
            }
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("results"));
            Buddy.Vocal.SayKey(key);

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                int i = 1;
                foreach (Result lResult in mScore.Results)
                {
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    lBox.SetLabel("   <size=60>" + lResult.Equation + " = " + lResult.CorrectAnswer + "</size>");
                    string iconPath = lResult.isCorrect() ? "os_icon_check" : "os_icon_close";
                    Sprite sprite = Buddy.Resources.Get<Sprite>(iconPath, Context.OS);
                    lBox.LeftButton.SetIcon(sprite);
                    Color iconColor = lResult.isCorrect() ? new Color(0, 212, 209) : Color.red;
                    //Color iconColor = lResult.isCorrect() ? Color.green : Color.red;
                    lBox.LeftButton.SetBackgroundColor(iconColor);
                    lBox.SetCenteredLabel(false);
                    i++;
                }
            });
        }

        private IEnumerator QuitOnTimeOut()
        {
            yield return new WaitForSeconds(TIMEOUT);

            QuitApp();

            //if (mScore.IsPerfect() /*&& !User.Instance.HasCurrentCertificate()*/)
            //{
            //    Buddy.Vocal.SayKey("takephotospeech", (iOutput) =>
            //    {
            //        Trigger("TakePhoto");
            //    });
            //}
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StopAllCoroutines();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Footer.Hide();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
