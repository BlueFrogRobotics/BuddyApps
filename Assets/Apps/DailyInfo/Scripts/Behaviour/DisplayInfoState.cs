using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.DailyInfo
{

    public class DisplayInfoState : AStateMachineBehaviour
    {
        private DailyInfoBehaviour mBehaviour;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBehaviour = GetComponent<DailyInfoBehaviour>();
            Buddy.GUI.Header.DisplayParametersButton(false);

            if (mBehaviour.InfosData.Count == 0)
            {
                // No info to display
                Buddy.Vocal.Say(Buddy.Resources.GetRandomString("nodata"), (iOutput) =>
                {
                    QuitApp();
                });
            }
            else
            {
                StartCoroutine(DisplayInfo());
            }

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
            Buddy.GUI.Header.HideTitle();
        }


        /// <summary>Displays the information resquested.</summary>
        /// <returns></returns>
        private IEnumerator DisplayInfo()
        {
            foreach (InfoData data in mBehaviour.InfosData)
            {
                if (!Buddy.Behaviour.Interpreter.IsBusy)
                {
                    int lRand = Random.Range(1, 6);
                    Buddy.Behaviour.Interpreter.Run("Idle0" + lRand);
                }

                string title = !string.IsNullOrEmpty(mBehaviour.Title) ? mBehaviour.Title : "INFO";

                if (!string.IsNullOrEmpty(data.DayPart))
                    title += " - " + data.DayPart;

                Buddy.GUI.Header.DisplayComplexTitle("<b>" + title.ToUpper() + "</b>",
                    "<b>" + mBehaviour.RequestedDate.ToString("m", CultureInfo.CreateSpecificCulture(mBehaviour.Lang)).ToUpper() + "</b>");

                Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
                {
                    int i = 0;
                    foreach (string item in data.Items)
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        
                        string[] items = item.Split(';');
                        if (items.Length > 1)
                            lBox.SetLabel(items[0].ToUpper(), "<size=35>" + items[1].ToUpper() + "</size>");
                        else
                            lBox.SetLabel(item.ToUpper());

                        if (mBehaviour.Icons != null && mBehaviour.Icons.Count > i)
                        {
                            Sprite sprite = Buddy.Resources.GetJIT<Sprite>(Buddy.Resources.AppSpritesPath + mBehaviour.Icons[i]);
                            lBox.LeftButton.SetIcon(sprite);
                        }
                        else
                        {
                            lBox.LeftButton.SetLabel(">");
                        }

                        lBox.SetCenteredLabel(false);
                        lBox.LeftButton.SetBackgroundColor(new Color(0.172f, 0.172f, 0.172f, 1F));
                        i++;
                    }
                });

                string txt = mBehaviour.DateStr + " ";
                if (!string.IsNullOrEmpty(data.DayPart))
                    txt +=  Buddy.Resources.GetRandomString("for") + " " + data.DayPart + " ";
                txt += Buddy.Resources.GetRandomString("therewillbe") + " [12]";
                for (int i = 0; i < data.Items.Count; i++)
                {
                    txt += data.Items[i].Replace(";", "[20]") + " [30] ";
                }
                Debug.Log("DisplayInfo Buddy say " + txt);
                Buddy.Vocal.Say(txt);

                // Display info at least 10 seconds
                yield return new WaitForSeconds(10);

                yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);

                Buddy.GUI.Toaster.Hide();
            }

            QuitApp();
        }
    }
}
