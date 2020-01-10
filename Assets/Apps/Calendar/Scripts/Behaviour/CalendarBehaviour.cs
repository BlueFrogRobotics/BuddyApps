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
        private readonly string STR_CONFIRM_DELETION_REMINDER = "reminderdeleted";
        private readonly string STR_CONFIRM_DELETION_TASK = "taskdeleted";

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

            InitReminder();

        }

        void InitReminder()
        {
            // Reset GUI
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Footer.Hide();

            // Display a title
            if (Buddy.Platform.Calendar.PlannedEvents.Count > 0)
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("myreminders"));
            else
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("noreminders"));

            DisplayListReminder();


            // Button to Add a new reminder
            FButton lButton = Buddy.GUI.Footer.CreateOnMiddle<FButton>();
            lButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus", Context.OS));
            lButton.OnClick.Add(() => {
                Buddy.Platform.Application.StartApp("Reminder", "Calendar");
            });

            FButton lButton2 = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lButton2.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_right", Context.OS));
            lButton2.OnClick.Add(() => {
                InitTask();
            });
        }

        void InitTask()
        {
            // Reset GUI
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Footer.Hide();

            if (Buddy.Platform.Calendar.ScheduledTasks.Count > 0)
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("mytasks"));
            else
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("notasks"));

            DisplayListTask();

            // Button to Add a new task
            FButton lButton = Buddy.GUI.Footer.CreateOnMiddle<FButton>();
            lButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus", Context.OS));
            lButton.OnClick.Add(() => {
                Buddy.Platform.Application.StartApp("Scheduler", "Calendar");
            });

            FButton lButton2 = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lButton2.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left", Context.OS));
            lButton2.OnClick.Add(() => {
                InitReminder();
            });

        }


        void DisplayListReminder()
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

        void DisplayListTask()
        {

            // Display menu toaster with following parameters
            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) => {

                // For each element of the previously build dictionnary,
                // we will create the corresponding button
                foreach (ScheduledTask lTask in Buddy.Platform.Calendar.ScheduledTasks) {
                    // We create the container
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    // We create an event OnClick to be triggered when the user click on the box
                    lBox.OnClick.Add(() => {
                        Debug.Log("Click " + lTask.ReminderContent);
                        Buddy.Vocal.Say(lTask.ReminderContent);
                        //Buddy.Vocal.StopListening();

                    });

                    TRightSideButton lPlayButton = lBox.CreateRightButton();
                    lPlayButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_play", Context.OS));
                    lPlayButton.OnClick.Add(() => {
                        Buddy.Vocal.Say(lTask.ReminderContent);
                    });

                    TRightSideButton lTrashButton = lBox.CreateRightButton();
                    lTrashButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_trash", Context.OS));
                    lTrashButton.SetIconColor(Color.red);
                    lTrashButton.OnClick.Add(() => {
                        Debug.Log("Click " + lTask.ReminderContent);
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
                                Buddy.Platform.Calendar.Remove(lTask);
                                Buddy.GUI.Dialoger.Hide();
                            },
                            Buddy.Resources.GetString(STR_CONFIRM)
                        );
                    });


                    TRightSideButton lAlarmButton = lBox.CreateRightButton();

                    if (lTask.ReminderState == ReminderState.VALIDATED)
                        lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_eye_on", Context.OS));
                   

                    lAlarmButton.SetIconColor(Color.red);
                    lAlarmButton.OnClick.Add(() => {
                        if (lTask.ReminderState == ReminderState.VALIDATED)
                            lAlarmButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_eye_on", Context.OS));                        
                    });


                    lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_agenda_check", Context.OS));


                    // We label our button with the informations in the dictionary
                    lBox.SetLabel(lTask.Rule, lTask.ReminderTime.ToShortDateString() + " - " + lTask.ReminderTime.ToShortTimeString());

                    // We place the text of the button in the center of the box
                    lBox.SetCenteredLabel(true);
                }
            });
        }

    }
}
