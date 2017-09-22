using Buddy;

namespace BuddyApp.ShoppingList
{
    /* Data are stored in xml file for persistent data purpose */
    public class ShoppingListData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static ShoppingListData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<ShoppingListData>();
                return sInstance as ShoppingListData;
            }
        }
    }
}
