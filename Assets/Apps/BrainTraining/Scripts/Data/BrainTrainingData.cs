using BlueQuark;

namespace BuddyApp.BrainTraining
{
    public enum QuizzType
    {
        QUESTION,
        IMAGE,
        MUSIC
    };

    /* Data are stored in xml file for persistent data purpose */
    public class BrainTrainingData : AAppData
    {        
        /*
         * Number of questions
         */
        public int NbQuestions { get; set; }

        /*
         * Number of categories
         */
        public int NbCategories { get; set; }

        /*
         * Data singleton access
         */
        public static BrainTrainingData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BrainTrainingData>();
                return sInstance as BrainTrainingData;
            }
        }
    }
}
