using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.RLGL
{
    public class RLGLData : AAppData
    {
        public enum Level : int
        {
            DEFAULT_LEVEL = 2,
            LEVEL_EASY = 1,
            LEVEL_MEDIUM = 2,
            LEVEL_HARD = 3,
            LEVEL_IMPOSSIBLE = 4
              
        }

        public Level Difficulty { get; set; }
        
        public static RLGLData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RLGLData>();
                return sInstance as RLGLData;
            }
        }
    }

}
