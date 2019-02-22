using BlueQuark;

namespace BuddyApp.Calendar
{
    /* Data are stored in xml file for persistent data purpose */
    public class CalendarData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static CalendarData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CalendarData>();
                return sInstance as CalendarData;
            }
        }
    }
}
