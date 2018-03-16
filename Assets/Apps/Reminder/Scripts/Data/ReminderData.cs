using Buddy;

namespace BuddyApp.Reminder
{
    /* Data are stored in xml file for persistent data purpose */
    public class ReminderData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        public string VocalRequest { get; set; }

        public bool GiveReminder { get; set; }

        public int SenderID { get; set; }

        public string Date;

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
