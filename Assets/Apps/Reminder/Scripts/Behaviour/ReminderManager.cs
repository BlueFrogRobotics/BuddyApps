using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System.IO;

namespace BuddyApp.Reminder
{
    public class ReminderManager : MonoBehaviour
    {
        public RemindersData RemindersData { get; private set; }

        private List<RemindersData> ListReminders;

        void Awake()
        {
            RemindersData = new RemindersData();
            ListReminders = new List<RemindersData>();

            string[] lReminderfile = Directory.GetFiles(BYOS.Instance.Resources.GetPathToRaw("Reminders"));

            RemindersData lRdata = Utils.UnserializeXML<RemindersData>(lReminderfile[0]);

            if (lRdata != null)
            {
                Debug.Log("remindr data pas nul");
                ListReminders.Add(lRdata);

                RemindersData = ListReminders[0];
            }
            else
            {
                Debug.Log("remindr data nul");
                RemindersData.Reminders = new List<Reminderkey>();
            }
        }

    }
}
