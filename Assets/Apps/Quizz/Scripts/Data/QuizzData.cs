using BlueQuark;

namespace BuddyApp.Quizz
{
    /* Data are stored in xml file for persistent data purpose */
    public class QuizzData : AAppData
    {
        /*
         * If false Buddy asks the number of players and give them name
         * If true Buddy starts directly with the questions
         */
        public bool OnePlayerGame { get; set; }
        /*
         * If true the questions are displayed on the screen and users can answer by touching the screen
         */
        public bool DisplayQuestions { get; set; }
        /*
         * Name of the file containing the questions of the quizz
         */
        public string QuizzFileName { get; set; }
        /*
         * Number of questions
         */
        public int NbQuestions { get; set; }


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
