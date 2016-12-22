using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneData : AAppData
    {

        public enum Contact : int
        {
            DEFAULT = 0 ,
            RODOLPHE = 1,
            J2M = 2,
            MAUD =3
        }

        public enum Lullaby : int
        {
            DEFAULT_LULL = 0,
            LULL_1 = 1,
            LULL_2 = 2,
            LULL_3 = 3,
            LULL_4 = 4
        }

        public enum Animation : int
        {
            OWL = 0,
            CHRISTMAS = 1
        }

        public enum Action : int
        {
            DEFAULT_ACTION = 0,
            REPLAY_LULLABY = 1,
            REPLAY_ANIMATION = 2,
            REPLAY_BOTH = 3
        }

        public Contact Recever { get; set; } 
        public Lullaby LullabyToPlay { get; set; }
        public Animation AnimationToPlay { get; set; }
        public Action ActionWhenBabyCries { get; set; }

        public int LullabyVolume { get; set; }
        public bool IsVolumeOn { get; set; }
        public int AnimationLight { get; set; }
        public bool IsAnimationOn { get; set; }

        public int MicrophoneSensitivity { get; set; }
        public int  TimeBeforContact { get; set; }
        public int ScreenSaverLight { get; set; }
        public bool IsScreanSaverOn { get; set; }
        public bool IsMotionOn { get; set; }
        public bool DoSaveSetting { get; set; }

        public int Timer { get; set; }
        public bool TimerIsActive { get; set; }
       
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
