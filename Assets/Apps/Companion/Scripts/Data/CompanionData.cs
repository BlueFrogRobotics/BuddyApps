using Buddy;

namespace BuddyApp.Companion
{
    /* Data are stored in xml file for persistent data purpose */
    public class CompanionData : AAppData
    {
        /*
         * Data getters / setters
         */
		public bool CanMoveBody { get; set; }
		public bool CanMoveHead { get; set; }
		public bool CanTrigger { get; set; }
		public bool CanTriggerWander { get; set; }
		public bool UseCamera { get; set; }
		public bool CanSetHeadPos { get; set; }
		public bool Debug { get; set; }
		//public bool ShowQRCode { get; set; }
		public float HeadPosition { get; set; }
		public int Bored { get; set; }
		public int MovingDesire { get; set; }
		public int InteractDesire { get; set; }
		public int LearnDesire { get; set; }
		public int TeachDesire { get; set; }
		public int HelpDesire { get; set; }

		public bool ChargeAsked { get; set; }
		public string LastApp { get; set; }
		public float LastAppTime { get; set; }

		/*
         * Data singleton access
         */
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
