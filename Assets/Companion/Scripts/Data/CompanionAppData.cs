using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyOS;

namespace BuddyApp.Companion
{
    internal class CompanionAppData : AAppData
    {
        public static CompanionAppData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CompanionAppData>();
                return sInstance as CompanionAppData;
            }
        }
    }
}
