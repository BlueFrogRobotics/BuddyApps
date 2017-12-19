using Buddy;

namespace BuddyApp.ExperienceCenter
{
    /* Data are stored in xml file for persistent data purpose */
    public class ExperienceCenterData : AAppData
    {
        /*
         * Data singleton access
         */
        public static ExperienceCenterData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ExperienceCenterData>();
                return sInstance as ExperienceCenterData;
            }
        }
    }
}
