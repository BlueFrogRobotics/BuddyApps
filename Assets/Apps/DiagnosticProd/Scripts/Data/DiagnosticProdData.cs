using BlueQuark;

namespace BuddyApp.DiagnosticProd
{
    /* Data are stored in xml file for persistent data purpose */
    public class DiagnosticProdData : AAppData
    {

        /*
         * Data singleton access.
         */
        public static DiagnosticProdData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<DiagnosticProdData>();
                return sInstance as DiagnosticProdData;
            }
        }
    }
}
