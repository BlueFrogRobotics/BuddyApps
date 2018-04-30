using Buddy;

namespace BuddyApp.SandboxApp
{
    /* Data are stored in xml file for persistent data purpose */
    public class SandboxAppData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static SandboxAppData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<SandboxAppData>();
                return sInstance as SandboxAppData;
            }
        }
    }
}
