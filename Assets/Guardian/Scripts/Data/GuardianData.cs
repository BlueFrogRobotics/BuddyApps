using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.App;


#if SIMULATION
namespace BuddyApp.Guardian
{
    public class GuardianData : AAppData
    {

        /*
         * Data getters / setters
         */
        public bool FireDetectionIsActive { get; set; }
        public bool MovementDetectionIsActive { get; set; }
        public bool SoundDetectionIsActive { get; set; }
        public bool KidnappingDetectionIsActive { get; set; }

        /*
         * Data singleton access
         */
        public static GuardianData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new GuardianData();
                return sInstance as GuardianData;
            }
        }

    }
}

#else
namespace BuddyApp.Guardian
{
    public class GuardianData : AAppData
    {
        /*
         * Data getters / setters
         */
        public bool FireDetectionIsActive { get; set; }
        public bool MovementDetectionIsActive { get; set; }
        public bool SoundDetectionIsActive { get; set; }
        public bool KidnappingDetectionIsActive { get; set; }

        /*
         * Data singleton access
         */
        public static GuardianData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<GuardianData>();
                return sInstance as GuardianData;
            }
        }

    }
}
#endif