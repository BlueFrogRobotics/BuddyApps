using BlueQuark;

namespace BuddyApp.Calcul
{
    /* Data are stored in xml file for persistent data purpose */
    public class CalculData : AAppData
    {
        /*
         * Data getters / setters
         */


        /*
         * Data singleton access
         */
        public static CalculData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CalculData>();
                return sInstance as CalculData;
            }
        }
    }
}
