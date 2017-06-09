using Buddy;

namespace BuddyApp.MemoryGame
{
    /* Data are stored in xml file for persistent data purpose */
    public class MemoryGameData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static MemoryGameData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<MemoryGameData>();
                return sInstance as MemoryGameData;
            }
        }
    }
}
