using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneData : AAppData
    {
        public int Timer { get; set; }
        public bool TimerIsActive { get; set; }
        public string SongToPlay { get; set; }
        //sauvegarde des paramètres YES/NO ? 
        //choix du contact
        //berceuse ON/OFF
        //choix de la berceuse 
        //le volume pour la berceuse 
        //visage ON/OFF
        //choix des "visages" à jouer
        //le niveau de la luminosité de l'écran pour le jeu dus visages
        //le temps "d'endormissement", le temps de jouer la berceuse
        //le temps avant contact = combien de temps, si on détecte les pleurs, avant d'en informer le contact?

        public static BabyPhoneData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<BabyPhoneData>();
                return sInstance as BabyPhoneData;
            }
        }
    }
}
