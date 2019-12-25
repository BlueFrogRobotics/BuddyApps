using System;
using BlueQuark;

namespace BuddyApp.Scheduler
{
    /* Data are stored in xml file for persistent data purpose */
    public class SchedulerData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static SchedulerData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<SchedulerData>();
                return sInstance as SchedulerData;
            }
        }
    }
}
