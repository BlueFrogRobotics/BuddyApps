using Buddy;

namespace BuddyApp.Recipe
{
    /* Data are stored in xml file for persistent data purpose */
    public class RecipeData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static RecipeData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RecipeData>();
                return sInstance as RecipeData;
            }
        }
    }
}
