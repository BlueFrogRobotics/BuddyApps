using BlueQuark;

namespace BuddyApp.Radio
{
    /* Data are stored in xml file for persistent data purpose */
    public class RadioData : AAppData
    {
        private const string DEFAULT_RADIO_STATION = "nrj";

        /*
         * Data getters / setters
         */
        public string DefaultRadio { get; set; }

        public bool StopEchoCancellation { get; set; }

        /*
         * Data singleton access
         */
        public static RadioData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RadioData>();
                return sInstance as RadioData;
            }
        }
        /*
        * Set current radio name from vocal request
        */
        public void SetRadio(string vocalRequest)
        {
            string lRadio = ExtractRadio(vocalRequest);

            if (lRadio != "")
                DefaultRadio = lRadio;
            // If DefaultRadio has not been defined by a previous call use default
            else if (string.IsNullOrEmpty(DefaultRadio))
                DefaultRadio = DEFAULT_RADIO_STATION;
        }

        /*
		* Get radio name from vocal request
		*/
        private string ExtractRadio(string iText)
        {
            string[] lWords = iText.Split(' ');
            if (lWords == null)
                return "";

            string lResult = "";
            for (int i = 0; i < lWords.Length; i++)
            {
                if (lWords[i].Contains("radio") && i < lWords.Length - 1)
                {
                    for (int j = i + 1; j < lWords.Length; j++)
                        lResult += (lWords[j] + " ");
                }
            }
            return lResult;
        }
    }
}
