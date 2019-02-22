using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BuddyApp.Calendar
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CalendarBehaviour : MonoBehaviour
    {

        private readonly string STR_ASK_FOR_VALIDATION = "deletevalidation";
        private readonly string STR_CONFIRM_DELETION = "reminderdeleted";

        private readonly string STR_CONFIRM = "accept";
        private readonly string STR_CANCEL = "refuse";

        private readonly string STR_YES = "yes";
        private readonly string STR_NO = "no";

        //private readonly float F_MAX_TIME_LISTENING = 10.0f;

        //private float mFTimeListening;

        void Start()
        {

            // Remove parameter button
            Buddy.GUI.Header.DisplayParametersButton(false);

            // Display a title
            if (Buddy.Platform.Calendar.PlannedEvents.Count > 0)
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("myreminders"));
            else
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("noreminders"));

            DisplayList();

            FButton lButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus", Context.OS));
            lButton.OnClick.Add(() => {
                Buddy.Platform.Application.StartApp("Reminder", "Calendar");
            });

        }

        void DisplayList()
        {
            //List<AScheduledReminder> lReminders = new List<AScheduledReminder>();
            //lReminders.AddRange(Buddy.Platform.Calendar.PlannedEvents);
            //lReminders.AddRange(Buddy.Platform.Calendar.PlannedAlarms);

            //lReminders = lReminders.OrderBy(o => o.ReminderTime).ToList();

            // Display menu toaster with following parameters
            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                // For each element of the previously build dictionnary,
                // we will create the corresponding button
                foreach (PlannedEventReminder lReminder in Buddy.Platform.Calendar.PlannedEvents) {
                    // We create the container
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    // We create an event OnClick to be triggered when the user click on the box
                    lBox.OnClick.Add(() => {
                        Debug.Log("Click " + lReminder.ReminderContent);
                        Buddy.Vocal.Say(lReminder.ReminderContent);
                        //Buddy.Vocal.StopListening();

                    });

                    TRightSideButton lPlayButton = lBox.CreateRightButton();
                    lPlayButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_play", Context.OS));
                    lPlayButton.OnClick.Add(() => {
                        Buddy.Vocal.Say(lReminder.ReminderContent);
                    });

                    TRightSideButton lTrashButton = lBox.CreateRightButton();
                    lTrashButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_trash", Context.OS));
                    lTrashButton.SetIconColor(Color.red);
                    lTrashButton.OnClick.Add(() => {
                        Debug.Log("Click " + lReminder.ReminderContent);
                        //iBuilder.Remove(lBox);
                        //Buddy.Platform.Calendar.Remove(lReminder);

                        //Buddy.GUI.Toaster.Hide();
                        //DisplayList();

                        Buddy.GUI.Dialoger.Display<ParameterToast>().With(
                            (jBuilder) => {
                                jBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString(STR_ASK_FOR_VALIDATION));
                            },
                            () => { Buddy.GUI.Dialoger.Hide(); }, Buddy.Resources.GetString(STR_CANCEL),
                            () => {
                                iBuilder.Remove(lBox);
                                Buddy.Platform.Calendar.Remove(lReminder);
                                Buddy.GUI.Dialoger.Hide();
                            },
                            Buddy.Resources.GetString(STR_CONFIRM)
                        );
                    });


                    TRightSideButton lAlarmButton = lBox.CreateRightButton();

                    if (lReminder.ReminderState == ReminderState.VALIDATED)
                        lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_eye_on", Context.OS));
                    else {
                        if (lReminder.NotifyUser)
                            lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock", Context.OS));
                        else
                            lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_ban", Context.OS));
                    }

                    lAlarmButton.SetIconColor(Color.red);
                    lAlarmButton.OnClick.Add(() => {
                        if (lReminder.ReminderState == ReminderState.VALIDATED)
                            lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_eye_on", Context.OS));
                        else {
                            if (lReminder.NotifyUser) {
                                lReminder.NotifyUser = false;
                                lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_ban", Context.OS));
                            } else {
                                lReminder.NotifyUser = true;
                                lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock", Context.OS));
                            }
                        }
                    });


                    lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_agenda_check", Context.OS));


                    // We label our button with the informations in the dictionary
                    lBox.SetLabel(lReminder.ReminderContent, lReminder.EventTime.ToShortDateString() + " - " + lReminder.EventTime.ToShortTimeString());

                    // We place the text of the button in the center of the box
                    lBox.SetCenteredLabel(true);
                }
            });
        }

    }
}
