using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class RecipeData : AAppData
    {
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