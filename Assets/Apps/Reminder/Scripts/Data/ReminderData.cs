using System;
using BlueQuark;

namespace BuddyApp.Reminder
{
    /* Data are stored in xml file for persistent data purpose */
    public class ReminderData : AAppData
    {
        /*
         * Data getters / setters
         */

        public int AppStepNumbers { get { return 3; } }
        public int AppState { get; set; }
        public bool DateSaved { get; set; }
        public bool HourSaved { get; set; }
        public DateTime ReminderDate { get; set; }

        /*
         * Data singleton access
         */
        public static ReminderData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ReminderData>();
                return sInstance as ReminderData;
            }
        }
    }
}
