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

            ListReminders.Add(lRdata);

            RemindersData = ListReminders[0];
        }

    }
}
