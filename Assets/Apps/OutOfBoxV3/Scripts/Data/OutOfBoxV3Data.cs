using BlueQuark;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.OutOfBoxV3
{
    /* Data are stored in xml file for persistent data purpose */
    public class OutOfBoxV3Data : AAppData
    {
        /*
         * Data getters / setters
         */
        //private List<String> nameOfPhotoTaken;
        public List<String> NameOfPhotoTaken { get; set; }


        /*
         * Data singleton access
         */
        public static OutOfBoxV3Data Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<OutOfBoxV3Data>();
                return sInstance as OutOfBoxV3Data;
            }
        }
    }
}
