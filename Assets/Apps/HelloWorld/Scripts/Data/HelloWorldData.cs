using BlueQuark;

namespace BuddyApp.HelloWorld
{
    /* Data are stored in xml file for persistent data purpose */
    public class HelloWorldData : AAppData
    {
        /*
         * Data getters / setters
         */
        public int MyValue { get; set; }

        /*
         * Data singleton access
         */
        public static HelloWorldData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<HelloWorldData>();
                return sInstance as HelloWorldData;
            }
        }
    }
}
