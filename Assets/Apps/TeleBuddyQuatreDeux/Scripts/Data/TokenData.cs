using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    [Serializable]
    public class TokenData
    {
        public string Access_Token;
        public string Refresh_Token;
        public string Api_Domain;
        public string Token_Type;
        public int Expires_In;
    }

}
