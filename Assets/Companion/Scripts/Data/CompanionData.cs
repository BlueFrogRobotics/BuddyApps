using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyOS;

namespace BuddyApp.Companion
{
    internal class CompanionData : AAppData
    {
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
