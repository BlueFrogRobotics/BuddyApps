using BuddyOS.App;

namespace BuddyApp.RemoteControl
{
    public class RemoteControlData : AAppData
    {
        public string ContactName { get; set; }
        public float Volume { get; set; }
        public bool IsVolumeOn { get; set; }
        public string LullabyToPlay { get; set; }
        public float ScreenLightLevelForAnimation { get; set; }
        public bool IsAnimationOn { get; set; }
        public string AnimationToPlay { get; set; }
        public float MicrophoneSensitivity { get; set; }
        public int  TimeBeforSartListening { get; set; }
        public float ScreenSaver { get; set; }
        public bool IsScreanSaverOn { get; set; }
        public bool IsMobilityOn { get; set; }
        public bool DoSaveSetting { get; set; }



        public int Timer { get; set; }
        public bool TimerIsActive { get; set; }

        
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

        public static RemoteControlData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RemoteControlData>();
                return sInstance as RemoteControlData;
            }
        }
    }
}
