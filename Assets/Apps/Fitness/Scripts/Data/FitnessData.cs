using BlueQuark;

namespace BuddyApp.Fitness
{
    /* Data are stored in xml file for persistent data purpose */
    public class FitnessData : AAppData
    {
        /*
         * Data getters / setters
         */
        public string Exercise { get; set; }

        /*
         * Data singleton access
         */
        public static FitnessData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<FitnessData>();
                return sInstance as FitnessData;
            }
        }
	}
}
