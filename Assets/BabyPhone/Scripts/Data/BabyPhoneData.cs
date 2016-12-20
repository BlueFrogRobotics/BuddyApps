using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneData : AAppData
    {

        public enum Contact : int
        {
            NOONE = 0 ,
            RODOLPHE = 1,
            J2M = 2,
            MAUD =3
        }

        //public enum Lullabies : int
        //{
        //    DEFAULT = 0,
        //    LULL1 = 1,
        //    LULL2 = 2,
        //    LULL3 = 3
        //}

        public Contact Recever { get; set; } 
        public int LullabyVolume { get; set; }
        public bool IsVolumeOn { get; set; }
        public string LullabyToPlay { get; set; }
        public int AnimationLight { get; set; }
        public bool IsAnimationOn { get; set; }
        public string AnimationToPlay { get; set; }
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
