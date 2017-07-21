using Buddy;

namespace BuddyApp.MemoryGame
{
    /* Data are stored in xml file for persistent data purpose */
    public class MemoryGameData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int Difficulty { get; set; }

		/*
         * Data getters / setters
         */
		public bool FullBody { get; set; }

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
