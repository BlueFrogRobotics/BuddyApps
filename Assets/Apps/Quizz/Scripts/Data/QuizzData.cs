using Buddy;

namespace BuddyApp.Quizz
{
    /* Data are stored in xml file for persistent data purpose */
    public class QuizzData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static QuizzData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<QuizzData>();
                return sInstance as QuizzData;
            }
        }
    }
}
