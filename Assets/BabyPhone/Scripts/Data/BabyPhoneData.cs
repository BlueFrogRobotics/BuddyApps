using BuddyOS.App;

namespace BuddyApp.BabyPhone
{
    public class BabyPhoneData : AAppData
    {
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
