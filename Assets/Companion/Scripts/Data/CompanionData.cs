using BuddyOS.App;

namespace BuddyApp.Companion
{
    public class CompanionData : AAppData
    {
        public bool CanMoveBody { get; set; }
        public bool CanMoveHead { get; set; }
        public bool UseCamera { get; set; }

        public static CompanionData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CompanionData>();
                return sInstance as CompanionData;
            }
        }
    }
}
