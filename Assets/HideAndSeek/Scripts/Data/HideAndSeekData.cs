using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.HideAndSeek
{
    public class HideAndSeekData : AAppData
    {

        public enum ObjectsLinked : int
        {
           WINDOW_LINKER,
           FACE_RECO,
           LOADING
        }

        /*
         * Data singleton access
         */
        public static HideAndSeekData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<HideAndSeekData>();
                return sInstance as HideAndSeekData;
            }
        }
    }
}