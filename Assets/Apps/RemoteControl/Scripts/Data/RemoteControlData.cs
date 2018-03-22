using Buddy;

namespace BuddyApp.RemoteControl
{
    /* Data are stored in xml file for persistent data purpose */
    public class RemoteControlData : AAppData
    {
        /*
         * Data getters / setters
         */
        public bool DiscreteMode{ get; set; }

        /*
         * Data singleton access
         */
        public static RemoteControlData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RemoteControlData>();
                return sInstance as RemoteControlData;
            }
        }
    }
}
