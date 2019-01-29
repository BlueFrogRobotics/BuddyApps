using BlueQuark;

namespace BuddyApp.TacheAskUserKORIAN
{
    /* Data are stored in xml file for persistent data purpose */
    public class TacheAskUserKORIANData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static TacheAskUserKORIANData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TacheAskUserKORIANData>();
                return sInstance as TacheAskUserKORIANData;
            }
        }
    }
}
