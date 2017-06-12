using Buddy;

namespace BuddyApp.Diagnostic
{
    /* Data are stored in xml file for persistent data purpose */
    public class DiagnosticData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access.
         */
        public static DiagnosticData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<DiagnosticData>();
                return sInstance as DiagnosticData;
            }
        }
    }
}
