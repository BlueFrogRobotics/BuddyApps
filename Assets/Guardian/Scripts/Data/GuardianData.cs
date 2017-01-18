﻿using UnityEngine;
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

        public enum Contact : int
        {
            NOBODY,
            RODOLPHE,
            J2M,
            MAUD,
            FRANCK,
            MARC,
            BENOIT,
            WALID
        }

        /*
         * Data getters / setters
         */
        public bool FireDetectionIsActive { get; set; }
        public bool MovementDetectionIsActive { get; set; }
        public bool SoundDetectionIsActive { get; set; }
        public bool KidnappingDetectionIsActive { get; set; }
        public bool TurnHeadIsActive { get; set; }
        public int MovementSensibility { get; set; }
        public int SoundSensibility { get; set; }
        public Contact Recever { get; set; }

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