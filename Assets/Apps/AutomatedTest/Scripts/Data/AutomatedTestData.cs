using BlueQuark;
using System.Collections.Generic;

namespace BuddyApp.AutomatedTest
{
    /* Data are stored in xml file for persistent data purpose */
    public class AutomatedTestData : AAppData
    {
        /*
         *  All Modules available for test
         */
        public enum MODULES
        {
            E_MOTION,
            E_CAMERA,
            //E_VOCAL,
            //E_GUI,
            E_NB_MODULE,
        }

        public Dictionary<MODULES, AModuleTest> Modules = null;

        /*
         * Data singleton access
         */
        public static AutomatedTestData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<AutomatedTestData>();
                return sInstance as AutomatedTestData;
            }
        }
    }
}
